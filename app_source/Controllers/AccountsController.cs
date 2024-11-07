using System;
using System.Net;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Web;
using App.BLL.Interfaces;
using App.Entity.DTOs.Employee;
using FS.BaseAPI;
using FS.BaseModels;
using FS.BaseModels.Enums;
using FS.BaseModels.IdentityModels;
using FS.BLL.Services.Interfaces;
using FS.Commons;
using FS.Commons.Models;
using FS.Commons.Models.DTOs;
using FS.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace App.API.Controllers
{
    // [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : BaseAPIController
    {
        private readonly IIdentityBizLogic _identityBizLogic;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmployeeBizLogic _employeeBizLogic;

        public AccountsController(IIdentityBizLogic identityBizLogic, IConfiguration configuration,
                                    IEmailService emailService, SignInManager<ApplicationUser> signInManager,
                                    UserManager<ApplicationUser> userManager,
                                    IEmployeeBizLogic employeeBizLogic)
        {
            this._identityBizLogic = identityBizLogic;
            this._configuration = configuration;
            this._emailService = emailService;
            this._signInManager = signInManager;
            this._userManager = userManager;
            _employeeBizLogic = employeeBizLogic;
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

                if (!dto.IsValidUserType())
                {
                    ModelState.AddModelError("UserType", "Loại người dùng không hợp lệ");
                    return ModelInvalid();
                }

                if (!dto.CheckValidDateOfBirth().Equals("VALID"))
                {
                    ModelState.AddModelError("DateOfBirth", dto.CheckValidDateOfBirth());
                    return ModelInvalid();
                }

                if (!dto.CheckValidIdentityCard().Equals("VALID"))
                {
                    ModelState.AddModelError("IdentityCard", dto.CheckValidIdentityCard());
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
                    Gender = dto.Gender.ToString() ?? Gender.None.ToString(),
                    DateOfBirth = dto.DateOfBirth,
                    IdentityCard = dto.IdentityCard,
                    Status = Status.Online.ToString()
                };

                var result = await _identityBizLogic.AddUserAsync(user, dto.Password);
                if (result < 0) return Error(Constants.SomeThingWentWrong);

                var addRole = await _identityBizLogic.AddRoleByNameAsync(user.Id.ToString(), dto.UserType.ToString());
                if (!addRole) return Error(Constants.SomeThingWentWrong);
                
                //<===Add to employee===>
                if (dto.UserType == UserType.Employee)
                {
                    var empRequestDTO = new EmployeeRequestDTO
                    {
                        DepartmentName = dto.DepartmentName
                    };
                    var tryAddEmp = await _employeeBizLogic.CreateUpdateEmployee(empRequestDTO, UserId);
                    if (!tryAddEmp.IsSuccess) return SaveError(tryAddEmp);
                }

                var sendMail = await SendEmailConfirm(user);
                if (!sendMail) return Error("Đã xảy ra lỗi trong quá trình gửi mail xác thực. Vui lòng đăng nhập lại để nhận một mail mới");
                var userRoles = await _identityBizLogic.GetRolesAsync(user.Id);
                var userData = new UserViewDTO(user, userRoles.ToList());
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
        public async Task<IActionResult> VerifyEmail(string token, string email)
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
                var genAccesToken = await _identityBizLogic.GenerateJwtToken(user, dto.IsRemember, roles.Contains(SystemRoleConstants.ADMIN),
                                                                                                            roles.Contains(SystemRoleConstants.MANAGER),
                                                                                                                    roles.Contains(SystemRoleConstants.EMPLOYEE));
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


        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPassDTO dto)
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
                    return GetNotFound(Constants.GetNotFound);
                var resetPassToken = await _identityBizLogic.GeneratePasswordResetTokenAsync(user);
                if (resetPassToken == null) return Error("Không thể xác thực mã để làm mới mật khẩu. Vui lòng thử lại sau ít phút.");
                var encodedToken = WebUtility.UrlEncode(resetPassToken);
                var forgotUrl = Url.Action(
                                            action: "ResetPasswordView",
                                            controller: "Accounts",
                                            values: new { token = encodedToken, userId = user.Id },
                                            protocol: Request.Scheme,
                                            host: _configuration["AppSettings:HomeUrl"].TrimEnd('/')
                                            );
                var message = new EmailDTO
                (
                    new string[] { user.Email! },
                    "Confirmation Email Link!",
                    $@"
<p>- Hệ thống nhận thấy bạn vừa gửi yêu cầu đổi mật khẩu với Email: <b>{user.Email}<b>.</p>
<p>- Vui lòng truy cập vào link này để tiếp tục quá trình thay đổi mật khẩu: <b>{forgotUrl!}<b></p>"
                );
                var sendMail = await _emailService.SendEmailAsync(message);
                if (!sendMail) return Error("Đã xảy ra lỗi trong quá trình gửi mail xác thực. Vui lòng gửi lại 1 yêu cầu khác.");
                return Success(sendMail, $"Chúng tôi đã gửi một yêu cầu thay đổi mật khẩu đến {dto.Email}");
            }
            catch (Exception ex)
            {
                ConsoleLog.WriteExceptionToConsoleLog(ex);
                return Error(Constants.SomeThingWentWrong);
            }
        }

        [HttpGet]
        [Route("reset-password-view")]
        public IActionResult ResetPasswordView(long userId, string token)
        {
            if (userId <= 0 || string.IsNullOrEmpty(token)) return GetError("Nhận token xác thực thất bại.");
            var model = new ResetPasswordDTO { UserId = userId, Token = token };
            return GetSuccess(model);
        }


        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) return ModelInvalid();
                var result = await _identityBizLogic.ResetPasswordAsync(dto.UserId.ToString(), dto.Token, dto.NewPassword);
                if (!result) return Error("Thay đổi mật khẩu không thành công.");
                return Success(result, "Thay đổi mật khẩu thành công. Hãy đăng nhập bằng mật khẩu mới của bạn để trải nghiệm dịch vụ.");
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
                var isInvoked = await IsTokenInvoked();
                if (isInvoked) return GetUnAuthorized(Constants.GetUnAuthorized);
                if (!ModelState.IsValid) return ModelInvalid();

                var user = await _identityBizLogic.GetByIdAsync(UserId);
                if (user == null)
                {
                    return GetNotFound(Constants.GetNotFound);
                }

                dto.userId = UserId;
                var checkToken = await _identityBizLogic.ValidateAndVerifyToken(dto);
                if (!checkToken.IsSuccess)
                {
                    return Error(checkToken.Message);
                }

                //use token
                if (checkToken.Data == null) return Error(Constants.SomeThingWentWrong);
                checkToken.Data.IsUsed = true;
                var useToken = await _identityBizLogic.UpdateTokenAsync(checkToken.Data);
                if (!useToken) return SaveError("Sử dụng refresh token không thành công.");

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


        [Authorize]
        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> LogOutAsync(LogOutDTO dto)
        {
            try
            {
                var isInvoked = await IsTokenInvoked();
                if (isInvoked) return GetUnAuthorized(Constants.GetUnAuthorized);
                if (!ModelState.IsValid) return ModelInvalid();
                var user = await _identityBizLogic.GetByIdAsync(UserId);
                if (user == null)
                {
                    return GetUnAuthorized(Constants.GetUnAuthorized);
                }

                dto.userId = UserId;
                var checkToken = await _identityBizLogic.ValidateAndVerifyToken(dto);
                if (!checkToken.IsSuccess)
                {
                    return Error(checkToken.Message);
                }

                if (checkToken.Data == null) return Error(Constants.SomeThingWentWrong);
                checkToken.Data.IsRevoked = true;
                var useToken = await _identityBizLogic.UpdateTokenAsync(checkToken.Data);
                if (!useToken) return SaveError("Thu hồi refresh token không thành công.");
                await _signInManager.SignOutAsync();
                return SaveSuccess("Đăng xuất thành công.");
            }
            catch (Exception ex)
            {
                ConsoleLog.WriteExceptionToConsoleLog(ex);
                return Error(Constants.SomeThingWentWrong);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO dto)
        {
            try
            {
                var isInvoked = await IsTokenInvoked();
                if (isInvoked) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (!ModelState.IsValid) return ModelInvalid();
                var user = await _identityBizLogic.GetByIdAsync(UserId);
                if (user == null) return GetUnAuthorized(Constants.GetUnAuthorized);
                var rightPassword = await _identityBizLogic.CheckPasswordAsync(user, dto.Password);
                if (!rightPassword)
                {
                    ModelState.AddModelError("Password", "Mật khẩu hiện tại không đúng!");
                    return ModelInvalid();
                }

                var token = await _identityBizLogic.GeneratePasswordResetTokenAsync(user);
                if (string.IsNullOrEmpty(token)) return Error("Lỗi tạo mã thay đổi mật khẩu.");
                var tryResetPasss = await _identityBizLogic.ResetPasswordAsync(UserId.ToString(), token, dto.NewPassword);
                if (!tryResetPasss) return Error("Thay đổi mật khẩu không thành công.");

                //Generate new token:
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
                if (newToken == null || newRefreshToken == null) return Error("Lỗi phát sinh mã đăng nhập.");
                var response = new LoginResponseDTO
                {
                    AccessToken = newToken.AccessToken,
                    RefreshToken = newRefreshToken,
                    Redirect = _configuration["AppSettings:HomeUrl"]
                };
                return Success(response, "Thay dổi mật khẩu thành công.");
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
                var confirmationLink = Url.Action(
                                            action: "VerifyEmail",
                                            controller: "Accounts",
                                            values: new { token = encodedToken, email = user.Email },
                                            protocol: Request.Scheme,
                                            host: _configuration["AppSettings:HomeUrl"].TrimEnd('/')
                                            );
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Url: {confirmationLink}");
                Console.ResetColor();
                var message = new EmailDTO
                (
                    new string[] { user.Email! },
                    "Confirmation Email Link!",
                    $@"
<p>- Hệ thống nhận thấy bạn đã đăng kí với Email: {user.Email}.</p>
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
        #endregion
    }
}
