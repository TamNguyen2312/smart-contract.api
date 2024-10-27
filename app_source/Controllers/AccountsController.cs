using System.Web;
using FS.BaseAPI;
using FS.BaseModels;
using FS.BaseModels.IdentityModels;
using FS.BLL.Services.Interfaces;
using FS.Commons;
using FS.Commons.Models;
using FS.Commons.Models.DTOs;
using FS.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : BaseAPIController
    {
        private readonly IIdentityBizLogic _identityBizLogic;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountsController(IIdentityBizLogic identityBizLogic, IConfiguration configuration,
                                    IEmailService emailService, SignInManager<ApplicationUser> signInManager,
                                    UserManager<ApplicationUser> userManager)
        {
            this._identityBizLogic = identityBizLogic;
            this._configuration = configuration;
            this._emailService = emailService;
            this._signInManager = signInManager;
            this._userManager = userManager;
        }

        #region COMMON

        [HttpPost]
        [Route("sign-up-account")]
        public async Task<IActionResult> SignUpAsync(RegisterDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ModelInvalid();
                }

                if (!Helpers.IsValidEmail(dto.Email.Trim()))
                {
                    ModelState.AddModelError("Email", Constants.EmailAddressFormatError);
                    return ModelInvalid();
                }

                if (dto.Password != dto.ConfirmPassword)
                {
                    ModelState.AddModelError("Password", Constants.ConfirmPasswordError);
                    return ModelInvalid();
                }

                if (!dto.IsValidGender())
                {
                    ModelState.AddModelError("Gender", "Giới tính không hợp lệ");
                    return ModelInvalid();
                }

                var userEmail = await _identityBizLogic.GetByEmailAsync(dto.Email);
                if (userEmail != null)
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại!");
                    return ModelInvalid();
                }

                var user = new ApplicationUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    EmailConfirmed = false,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    PhoneNumber = dto.PhoneNumber,
                    Avatar = Constants.DefaultAvatar,
                    Gender = dto.Gender.ToString() ?? Gender.None.ToString()
                };

                var result = await _identityBizLogic.AddUserAsync(user, dto.Password);
                if (result < 0) return Error(Constants.SomeThingWentWrong);

                var addRole = await _identityBizLogic.AddRoleByNameAsync(user.Id.ToString(), UserType.Customer.ToString());
                if (!addRole) return Error(Constants.SomeThingWentWrong);

                var sendMail = await SendEmailConfirm(user);
                if (!sendMail) return Error("Đã xảy ra lỗi trong quá trình gửi mail xác thực. Vui lòng đăng nhập lại để nhận một mail mới");
                var userData = await GetUserView(user);
                return SaveSuccess(userData);
            }
            catch (Exception ex)
            {
                ConsoleLog.WriteExceptionToConsoleLog(ex);
                return Error(Constants.SomeThingWentWrong);
            }
        }

        [HttpGet]
        [Route("verify-email")]
        public async Task<IActionResult> VeryfiEmailAsync(string token, string email)
        {
            var user = await _identityBizLogic.GetByEmailAsync(email);
            if (user == null)
            {
                return GetNotFound("Không tìm thấy Email người dùng trong hệ thống.");
            }
            var response = await _identityBizLogic.VerifyEmailAsync(user, token);

            if (!response)
            {
                return Error("Xác thực KHÔNG thành công.");
            }

            return Success(response, "Xác thực tài khoản thành công.");
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) return ModelInvalid();

                if (!Helpers.IsValidEmail(dto.Email.Trim()))
                {
                    ModelState.AddModelError("Email", Constants.EmailAddressFormatError);
                    return ModelInvalid();
                }

                var user = await _identityBizLogic.GetByEmailAsync(dto.Email);
                if (user == null)
                {
                    ModelState.AddModelError("Email", "Email không tồn tại.");
                    return ModelInvalid();
                }

                var rightPassword = await _identityBizLogic.CheckPasswordAsync(user, dto.Password);
                if (!rightPassword)
                {
                    ModelState.AddModelError("Password", Constants.PasswordIsInCorrect);
                    return ModelInvalid();
                }

                //check email confirm
                if (!user.EmailConfirmed)
                {
                    var sendMail = await SendEmailConfirm(user);
                    if (!sendMail)
                    {
                        return Error($"Email quý khách chưa đưọc xác thực. {Constants.SomeThingWentWrong}");
                    }
                    else
                    {
                        return GetUnAuthorized("Chúng tôi đã gửi email xác thực đến tài khoản của bạn. Hãy kiểm tra và thực hiện xác thực");
                    }
                }

                //check 2fa otp
                //....

                //check account is locked
                var signedInAccount = await _signInManager.PasswordSignInAsync(user, dto.Password, true, true);
                if (signedInAccount.IsLockedOut)
                {
                    return GetUnAuthorized($"Tài khoản của bạn đã bị khóa vì đăng nhập sai nhiều lần. Khóa đến: {user.LockoutEnd.Value.LocalDateTime}.");
                }
                await _userManager.ResetAccessFailedCountAsync(user);

                //gen token
                var roles = await _userManager.GetRolesAsync(user);
                var genAccesToken = await _identityBizLogic.GenerateJwtToken(user, dto.IsRemember, roles.Contains(SystemRoleConstants.ADMIN));
                string refreshToken = await _identityBizLogic.GenerateRefreshToken(user, genAccesToken.JwtToken, dto.IsRemember);

                var response = new LoginResponseDTO
                {
                    AccessToken = genAccesToken.AccessToken,
                    RefreshToken = refreshToken,
                    Redirect = string.IsNullOrEmpty(dto.Redirect) ? "/" : dto.Redirect
                };
                return response != null ? SaveSuccess(response) : Error(Constants.SomeThingWentWrong);
            }
            catch (Exception ex)
            {
                ConsoleLog.WriteExceptionToConsoleLog(ex);
                return Error(Constants.SomeThingWentWrong);
            }
        }

        #endregion

        #region USER

        [Authorize]
        [HttpPost]
        [Route("renew-token")]
        public async Task<IActionResult> RenewTokenAsync(RenewTokenDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) return ModelInvalid();

                var user = await _identityBizLogic.GetByIdAsync(UserId);
                if (user == null)
                {
                    return GetNotFound("Không tìm thấy người dùng.");
                }

                var checkToken = await _identityBizLogic.CheckToRenewToken(dto, user);
                if (!checkToken.Success)
                {
                    return Error(checkToken.Message);
                }

                var newToken = await _identityBizLogic.GenerateJwtToken
                (
                    user: user,
                    isRemember: IsRemember,
                    isAdmin: IsAdmin,
                    isEmployee: IsEmployee,
                    isManager: IsManager
                );
                var newRefreshToken = await _identityBizLogic.GenerateRefreshToken
                (
                    user: user,
                    jwtToken: newToken.JwtToken,
                    isRemember: IsRemember
                );
                if (newToken == null || newRefreshToken == null)
                {
                    return Error("Tạo mã đăng nhập mới không thành công. Vui lòng thử lại sau ít phút.");
                }

                return SaveSuccess(new LoginResponseDTO { AccessToken = newToken.AccessToken, RefreshToken = newRefreshToken });
            }
            catch (Exception ex)
            {
                ConsoleLog.WriteExceptionToConsoleLog(ex);
                return Error(Constants.SomeThingWentWrong);
            }
        }

        #endregion

        #region PRIVATE
        private async Task<bool> SendEmailConfirm(ApplicationUser user)
        {
            try
            {
                var emailToken = await _identityBizLogic.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = HttpUtility.UrlEncode(emailToken);
                var configVerifyUrl = _configuration.GetSection("Authentication").GetValue<string>("VerifyEmail");
                var confirmationLink = $"{configVerifyUrl}token={encodedToken}&email={user.Email}";
                var message = new EmailDTO
                (
                    new string[] { user.Email! },
                    "Confirmation Email Link!",
                    $@"
<p>- Hệ thống nhận thấy bạn vừa đăng kí với Email: {user.Email}.</p>
<p>- Vui lòng truy cập vào link này để xác thực tài khoản: {confirmationLink!}</p>"
                );
                var sendMail = await _emailService.SendEmailAsync(message);
                if (!sendMail) return false;
                return true;
            }
            catch (Exception ex)
            {
                ConsoleLog.WriteExceptionToConsoleLog(ex);
                throw;
            }
        }
        private async Task<UserViewDTO> GetUserView(ApplicationUser user)
        {
            return new UserViewDTO
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = $"{user.FirstName} {user.LastName}",
                Avatar = user.Avatar,
                Gender = Enum.TryParse<Gender>(user.Gender, out var gender) ? gender : default
            };
        }
        #endregion
    }
}
