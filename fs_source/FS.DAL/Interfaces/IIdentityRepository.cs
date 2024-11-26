using System;
using System.IdentityModel.Tokens.Jwt;
using FS.BaseModels;
using FS.BaseModels.IdentityModels;
using FS.Commons;
using FS.Commons.Models;
using FS.Commons.Models.DTOs;
using Microsoft.AspNetCore.Identity;

namespace FS.DAL.Interfaces;

public interface IIdentityRepository
{
    Task<ApplicationUser> GetByEmailAsync(string email);
    Task<long> AddUserAsync(ApplicationUser dto, string password);
    Task<bool> UpdateAsync(ApplicationUser dto);
    Task<ApplicationUser> GetByIdAsync(long id);
    Task<ApplicationUser> GetByExternalIdAsync(string id);
    Task<bool> CheckPasswordAsync(ApplicationUser dto, string password);
    Task<bool> HasPasswordAsync(ApplicationUser dto);
    Task<IdentityResult> AddPasswordAsync(ApplicationUser dto, string password);
    /// <summary>
    /// lấy thông tin user theo điện thoại
    /// </summary>
    /// <param name="phoneNumber">số điện thoại</param>
    /// <returns></returns>
    Task<ApplicationUser> GetByPhoneAsync(string phoneNumber);
    /// <summary>
    /// xác thực email
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    Task<bool> ConfirmEmailAsync(string userId, string code);
    /// <summary>
    /// Verify Email with Get http method
    /// </summary>
    /// <param name="user"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<bool> VerifyEmailAsync(ApplicationUser user, string token);
    /// <summary>
    /// thay đổi mật khẩu
    /// </summary>
    /// <param name="userId">id user cần thay đổi mật khẩu</param>
    /// <param name="passwordNew">mật khẩu mới</param>
    /// <returns></returns>
    Task<bool> ChangePassword(string userId, string passwordNew);

    /// <summary>
    /// thêm role cho ứng dụng
    /// </summary>
    /// <param name="roleName"></param>
    /// <param name="isAdmin"></param>
    /// <returns></returns>
    Task<bool> CreateUpdateRoleAsync(string roleName, bool isAdmin);

    /// <summary>
    /// thêm role cho người dùng theo tên role
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    Task<bool> AddRoleByNameAsync(string userId, string roleName);
    /// <summary>
    /// generate email confirm token
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
    Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
    Task<bool> ResetPasswordAsync(string userId, string token, string newPassword);
    /// <summary>
    /// Is user in role
    /// </summary>
    /// <param name="user"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    Task<bool> IsUserInRole(ApplicationUser user, string role);
    /// <summary>
    /// xóa quyền của user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<bool> DeleteRoleByUser(long userId);

    /// <summary>
    /// DeleteListRole
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    Task<bool> DeleteListRole(long[] ids);
    /// <summary>
    /// Get list roles by userId
    /// </summary>
    /// <param name="userId">The user identified.</param>
    /// <returns></returns>
    Task<string[]> GetRolesAsync(long userId);
    /// <summary>
    /// Verified user can be access to the function.
    /// </summary>
    Task<bool> VerifyPermission(long userId, string claim);

    /// <summary>
    /// lấy toàn bộ role quản trị
    /// </summary>
    /// <returns></returns>
    Task<List<Role>> GetRolesAdmin();
    Task<JwtSecurityTokenDTO> GenerateJwtToken(ApplicationUser user, bool isRemember, bool isAdmin, bool isManager = false, bool isEmployee = false);
    Task<string> GenerateRefreshToken(ApplicationUser user, JwtSecurityToken jwtToken, bool isRemeber);
    /// <summary>
    /// This is used to authen and check valiation of access and refresh token
    /// </summary>
    /// <param name="tokenModel"></param>
    /// <returns></returns>
    Task<BaseResponse<RefreshToken>> ValidateAndVerifyToken(BaseTokenModel tokenModel);
    Task<bool> UpdateTokenAsync(RefreshToken refreshToken);
    Task<bool> IsTokenInvoked(string jti, long userId);
    Task<List<ApplicationUser>> GetAll(AccountGetListDTO dto);
    Task<ApplicationUser> GetByUserName(string username);
    Task<ApplicationUser> GetByEmailOrUserNameAsync(string input);
}
