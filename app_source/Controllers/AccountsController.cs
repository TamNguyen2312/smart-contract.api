using System.Web;
using App.API.Email;
using FS.BaseAPI;
using FS.BaseModels.IdentityModels;
using FS.BLL.Services.Interfaces;
using FS.Commons;
using FS.Commons.Models;
using FS.Commons.Models.DTOs;
using FS.Utility;
using Microsoft.AspNetCore.Http;
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
                    ModelState.AddModelError("Password", "Mật khẩu xác thực không đúng!");
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
                return GetError("Không tìm thấy Email người dùng trong hệ thống.");
            }
            var response = await _identityBizLogic.VerifyEmailAsync(user, token);

            if (!response)
            {
                return Error("Xác thực KHÔNG thành công.");
            }

            return Success(response, "Xác thực tài khoản thành công.");
        }

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
