using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Azure.Core;
using FS.BaseModels;
using FS.BaseModels.IdentityModels;
using FS.Commons;
using FS.Commons.Extensions;
using FS.Commons.Models;
using FS.Commons.Models.DTOs;
using FS.DAL.Base;
using FS.DAL.Interfaces;
using FS.DAL.Queries;
using FS.IdentityFramework;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FS.DAL.Implements;

public class IdentityRepository : BaseRepository, IIdentityRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IFSUnitOfWork<FSDbContext> _unitOfWork;
    private readonly IConfiguration _configuration;
    public IdentityRepository(IConfiguration config, UserManager<ApplicationUser> userManager,
                             RoleManager<Role> roleManager, FSDbContext dbContext,
                             IFSUnitOfWork<FSDbContext> unitOfWork) : base(config, dbContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        this._unitOfWork = unitOfWork;
        _configuration = config;
    }

    public async Task<long> AddUserAsync(ApplicationUser dto, string password)
    {
        IdentityResult result;
        if (string.IsNullOrEmpty(password))
        {
            result = await _userManager.CreateAsync(dto);
        }
        else
        {
            result = await _userManager.CreateAsync(dto, password);
        }
        if (result.Succeeded)
            return dto.Id;
        return -1;
    }
    public async Task<bool> UpdateAsync(ApplicationUser dto)
    {
        var result = await _userManager.UpdateAsync(dto);
        return result.Succeeded;
    }

    public async Task<JwtSecurityTokenDTO> GenerateJwtToken(ApplicationUser user, bool isRemember, bool isAdmin, bool isManager = false, bool isEmployee = false)
    {
        var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? String.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(Constants.FULL_NAME, user.FirstName + " " +  user.LastName ?? string.Empty),
                new Claim(Constants.POLICY_VERIFY_EMAIL, user.EmailConfirmed.ToString()),
                new Claim(Constants.AVATAR, user.Avatar ?? Constants.DefaultAvatar),
                new Claim(Constants.IS_ADMIN, isAdmin.ToString()),
                new Claim(Constants.IS_MANAGER, isManager.ToString()),
                new Claim(Constants.IS_EMPLOYEE, isEmployee.ToString()),
                new Claim(Constants.IS_REMEMBER, isRemember.ToString())

            };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var token = new JwtSecurityToken(
             _configuration["JWT:ValidAudience"],
             _configuration["JWT:ValidIssuer"],
             claims,
             expires: isRemember ? DateTime.Now.AddDays(28) : DateTime.Now.AddMinutes(1),
             signingCredentials: creds
        );
        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        return new JwtSecurityTokenDTO { AccessToken = accessToken, JwtToken = token };
    }

    /// <summary>
    /// This is used to Generate a refresh token and save it to database
    /// </summary>
    /// <param name="user"></param>
    /// <param name="jwtToken"></param>
    /// <param name="isRemember"></param>
    /// <returns></returns>
    public async Task<string> GenerateRefreshToken(ApplicationUser user, JwtSecurityToken jwtToken, bool isRemember)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var refreshTokenRepo = _unitOfWork.GetRepository<RefreshToken>();
            var any = await refreshTokenRepo.AnyAsync(new QueryBuilder<RefreshToken>()
                                                    .WithPredicate(x => x.UserId == user.Id)
                                                    .WithTracking(false)
                                                    .Build());
            var refreshtoken = GenerateRandomStringAsToken();
            if (any)
            {
                var storedToken = await refreshTokenRepo.GetSingleAsync(new QueryBuilder<RefreshToken>()
                                                                        .WithPredicate(x => x.UserId == user.Id)
                                                                        .WithTracking(false)
                                                                        .Build());
                storedToken.JwtId = jwtToken.Id;
                storedToken.Token = refreshtoken;
                storedToken.IsUsed = false;
                storedToken.IsRevoked = false;
                storedToken.IssuedAt = DateTime.Now;
                storedToken.ExpiredAt = isRemember ? DateTime.Now.AddDays(28) : DateTime.Now.AddMinutes(5);
                await refreshTokenRepo.UpdateAsync(storedToken);
            }
            else
            {
                var refreshTokenInDb = new RefreshToken
                {
                    JwtId = jwtToken.Id,
                    UserId = user.Id,
                    Token = refreshtoken,
                    IsUsed = false,
                    IsRevoked = false,
                    IssuedAt = DateTime.Now,
                    ExpiredAt = isRemember ? DateTime.Now.AddDays(28) : DateTime.Now.AddMinutes(5),
                };
                await refreshTokenRepo.CreateAsync(refreshTokenInDb);
            }
            var saver = await _unitOfWork.SaveAsync();
            await _unitOfWork.CommitTransactionAsync();
            if (!saver)
            {
                throw new Exception("Đã xảy ra lỗi trong quá trình tạo và lưu refresh token");
            }
            return refreshtoken;
        }
        catch (Exception)
        {
            await _unitOfWork.RollBackAsync();
            throw;
        }
    }

    /// <summary>
    /// This is used to authen and check validation of access and refresh token
    /// </summary>
    /// <param name="tokenModel"></param>
    /// <returns></returns>
    public async Task<BaseResponse<RefreshToken>> ValidateAndVerifyToken(BaseTokenModel tokenModel)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var secretKeyBytes = Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]);
        var tokenValidateParam = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = _configuration["JWT:ValidAudience"],
            ValidIssuer = _configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false
        };

        try
        {
            var tokenInVerification = jwtTokenHandler.ValidateToken(tokenModel.AccessToken, tokenValidateParam, out var validatedToken);
            var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            if (validatedToken is JwtSecurityToken jwtSecurityToken &&
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
            {
                return new BaseResponse<RefreshToken> { IsSuccess = false, Message = "Access token không hợp lệ" };
            }

            var utcExpireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
            if (expireDate > DateTime.UtcNow)
            {
                return new BaseResponse<RefreshToken> { IsSuccess = false, Message = "Access token chưa hết hạn." };
            }

            var refreshTokenRepo = _unitOfWork.GetRepository<RefreshToken>();
            var storedToken = await refreshTokenRepo.GetSingleAsync(new QueryBuilder<RefreshToken>()
                                                                        .WithPredicate(x => x.Token.Equals(tokenModel.RefreshToken)
                                                                                        && x.UserId == tokenModel.userId
                                                                                        && x.JwtId.Equals(jti))
                                                                        .WithTracking(false)
                                                                        .Build());
            if (storedToken == null || storedToken.IsUsed || storedToken.IsRevoked)
            {
                return new BaseResponse<RefreshToken>
                {
                    IsSuccess = false,
                    Message = storedToken == null ? "Refresh Token không tồn tại." :
                              storedToken.IsUsed ? "Refresh token đã được sử dụng." :
                                                   "Refresh token đã bị thu hồi."
                };
            }
            return new BaseResponse<RefreshToken>
            {
                IsSuccess = true,
                Message = "Token hợp lệ",
                Data = storedToken
            };
        }
        catch (Exception)
        {
            throw;
        }
    }


    /// <summary>
    /// This is used to update refresh token status
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    public async Task<bool> UpdateTokenAsync(RefreshToken refreshToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var refreshTokenRepo = _unitOfWork.GetRepository<RefreshToken>();
            await refreshTokenRepo.UpdateAsync(refreshToken);
            var saver = await _unitOfWork.SaveAsync();
            await _unitOfWork.CommitTransactionAsync();
            return saver;
        }
        catch (Exception)
        {
            await _unitOfWork.RollBackAsync();
            throw;
        }
    }


    /// <summary>
    /// This is used to check token in use ?
    /// </summary>
    /// <param name="jti"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<bool> IsTokenInvoked(string jti, long userId)
    {
        var refreshTokenRepo = _unitOfWork.GetRepository<RefreshToken>();
        var storedToken = await refreshTokenRepo.GetSingleAsync(new QueryBuilder<RefreshToken>()
                                                                        .WithPredicate(x => x.UserId == userId
                                                                                        && x.JwtId.Equals(jti))
                                                                        .WithTracking(false)
                                                                        .Build());
        if (storedToken == null) return true;
        if (storedToken.IsRevoked || storedToken.IsUsed) return true;
        return false;
    }
    
    public async Task<ApplicationUser> GetByUserName(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        return user;
    }
    
    /// <summary>
    /// This is used to find a user by Email or UserName
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<ApplicationUser> GetByEmailOrUserNameAsync(string input)
    {
        if (input.Contains("@"))
        {
            // Xử lý như email
            var user = await _userManager.FindByEmailAsync(input);
            if (user == null)
            {
                user = await _dbContext.Users.FirstOrDefaultAsync(
                    x => x.Email.Replace(".", "") == input.Replace(".", "")
                );
            }
            return user;
        }
        else
        {
            // Xử lý như tên người dùng
            return await _userManager.FindByNameAsync(input);
        }
    }
    
    public async Task<ApplicationUser> GetByEmailAsync(string email)
    {
        if (email.IndexOf('@') >= 0)
        {
            // Find by email:
            var result = await _userManager.FindByEmailAsync(email);
            if (result == null)
                result = await base._dbContext.Users.FirstOrDefaultAsync(x => x.Email.Replace(".", "") == email.Replace(".", ""));
            return result;
        }
        // Find by userName
        return await _userManager.FindByNameAsync(email);
    }

    public async Task<ApplicationUser> GetByIdAsync(long id)
    {
        return await _userManager.FindByIdAsync(id.ToString());
    }

    public async Task<ApplicationUser> GetByExternalIdAsync(string id)
    {
        return await _userManager.FindByIdAsync(id.ToString());
    }

    public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<bool> HasPasswordAsync(ApplicationUser user)
    {
        return await _userManager.HasPasswordAsync(user);
    }

    public async Task<IdentityResult> AddPasswordAsync(ApplicationUser dto, string password)
    {
        return await _userManager.AddPasswordAsync(dto, password);
    }

    /// <summary>
    /// lấy thông tin user theo điện thoại
    /// </summary>
    /// <param name="phoneNumber">số điện thoại</param>
    /// <returns></returns>
    public async Task<ApplicationUser> GetByPhoneAsync(string phoneNumber)
    {
        var tempPhone = phoneNumber.StartsWith(Constants.PhoneNumberVietNam) ? phoneNumber.Substring(3) : phoneNumber;
        if (!tempPhone.StartsWith("0"))
            tempPhone = $"0{tempPhone}";
        else
            tempPhone = tempPhone.Substring(1);
        tempPhone = Constants.PhoneNumberVietNam + tempPhone;
        return await _userManager.Users.Where(x => x.PhoneNumber.Equals(phoneNumber) || x.PhoneNumber.Equals(tempPhone)).AsNoTracking().FirstOrDefaultAsync();
    }

    /// <summary>
    /// xác thực email
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public async Task<bool> ConfirmEmailAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            var decodedToken = HttpUtility.UrlDecode(token);
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken.Replace(" ", "+"));
            if (result.Succeeded)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Verify email with Get http method
    /// </summary>
    /// <param name="user"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<bool> VerifyEmailAsync(ApplicationUser user, string token)
    {
        var decodedToken = HttpUtility.UrlDecode(token);
        var result = await _userManager.ConfirmEmailAsync(user, decodedToken.Replace(" ", "+"));
        if (result.Succeeded) return true;
        return false;
    }

    /// <summary>
    /// thay đổi mật khẩu
    /// </summary>
    /// <param name="userId">id user cần thay đổi mật khẩu</param>
    /// <param name="passwordNew">mật khẩu mới</param>
    /// <returns></returns>
    public async Task<bool> ChangePassword(string userId, string passwordNew)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            //remove password
            var remove = await _userManager.RemovePasswordAsync(user);
            if (remove.Succeeded)
            {
                var result = await _userManager.AddPasswordAsync(user, passwordNew);
                return result.Succeeded;
            }
        }
        return false;
    }

    /// <summary>
    /// thêm role cho người dùng theo tên role
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public async Task<bool> AddRoleByNameAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                var result = await _userManager.AddToRoleAsync(user, roleName);
                if (result.Succeeded)
                    return true;
            }
        }
        return false;
    }
    /// <summary>
    /// generate email confirm token
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
    {
        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
    {
        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<bool> ResetPasswordAsync(string userId, string token, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            var decodedToken = WebUtility.UrlDecode(token);
            if (decodedToken.Contains(' '))
            {
                decodedToken = decodedToken.Replace(" ", "+");
            }
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, newPassword);

            if (!user.EmailConfirmed && result.Succeeded)
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
            }

            return result.Succeeded;
        }
        return false;
    }

    public async Task<bool> CreateUpdateRoleAsync(string roleName, bool isAdmin)
    {
        var roleExists = await _roleManager.RoleExistsAsync(roleName);

        if (!roleExists)
        {
            var newRole = new Role
            {
                Name = roleName,
                IsAdmin = isAdmin
            };
            var result = await _roleManager.CreateAsync(newRole);
            if (result.Succeeded)
            {
                return true;
            }
        }
        else
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                role.IsAdmin = isAdmin;
                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return true;
                }
            }
        }
        return false;

    }

    public async Task<bool> IsUserInRole(ApplicationUser user, string role)
    {
        return await _userManager.IsInRoleAsync(user, role);
    }
    /// <summary>
    /// xóa quyền của user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<bool> DeleteRoleByUser(long userId)
    {
        var userRoles = base._dbContext.UserRoles.Where(a => a.UserId == userId);
        base._dbContext.RemoveRange(userRoles);
        var result = await base._dbContext.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// DeleteListRole
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    public async Task<bool> DeleteListRole(long[] ids)
    {
        if (ids != null && ids.Length > 0)
        {
            foreach (var id in ids)
            {
                var role = await _roleManager.FindByIdAsync(id.ToString());
                if (role != null)
                {
                    // delete RoleClaims
                    var claims = await _roleManager.GetClaimsAsync(role);
                    if (claims != null && claims.Count > 0)
                        foreach (var claim in claims)
                            await _roleManager.RemoveClaimAsync(role, claim);

                    // delete UserRoles
                    var userRoles = base._dbContext.UserRoles.Where(a => a.RoleId == id);
                    if (userRoles != null)
                        base._dbContext.RemoveRange(userRoles);

                    // delete Role
                    await _roleManager.DeleteAsync(role);
                }
            }
            return true;
        }
        return false;
    }

    public async Task<string[]> GetRolesAsync(long userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return null;
            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToArray();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> VerifyPermission(long userId, string claim)
    {
        try
        {
            var roles = base._dbContext.UserRoles.Where(m => m.UserId == userId).Select(m => m.RoleId);
            var claims = await base._dbContext.RoleClaims.Where(m => roles.Contains(m.RoleId)).Select(m => m.ClaimValue).ToArrayAsync();
            return claims.Contains(claim);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// This is used to get all accounts in db
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<List<ApplicationUser>> GetAll(AccountGetListDTO dto)
    {
        var userQuery = _userManager.Users;
        if (!string.IsNullOrEmpty(dto.Keyword))
        {
            userQuery = userQuery.Where(x => x.FirstName.Contains(dto.Keyword)
                                                                || x.LastName.Contains(dto.Keyword)
                                                                || x.Email.Contains(dto.Keyword));
        }

        dto.TotalRecord = await userQuery.CountAsync();
        var response = await userQuery.ToPagedList(dto.PageIndex, dto.PageSize).ToListAsync();
        return response;
    }

    /// <summary>
    /// lấy toàn bộ role quản trị
    /// </summary>
    /// <returns></returns>
    public async Task<List<Role>> GetRolesAdmin()
    {
        try
        {
            var roles = await base._dbContext.Roles.Where(x => x.IsAdmin).ToListAsync();
            if (roles != null && roles.Any())
            {
                foreach (var role in roles)
                    role.RoleClaims = await base._dbContext.RoleClaims.Where(x => x.RoleId == role.Id).ToListAsync();
                return roles;
            }
            return null;
        }
        catch (Exception)
        {
            throw;
        }
    }


    #region PRIVATE
    /// <summary>
    /// This is used to generate a random string for refreshtoken
    /// </summary>
    /// <returns></returns>
    private string GenerateRandomStringAsToken()
    {
        var random = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(random);

            return Convert.ToBase64String(random);
        }
    }

    /// <summary>
    /// This is used to covert Unixtime to Datetime
    /// </summary>
    /// <param name="utcExpireDate"></param>
    /// <returns></returns>
    private DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
    {
        var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();

        return dateTimeInterval;
    }
    #endregion

}
