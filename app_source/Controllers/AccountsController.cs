using System.Web;
using App.API.Email;
using FS.BaseAPI;
using FS.BaseModels.IdentityModels;
using FS.BLL.Services.Interfaces;
using FS.Commons;
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
        [Route("register-account")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) return ModelInvalid();
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
                };

                var result = await _identityBizLogic.AddUserAsync(user, dto.Password);
                if (result > 0)
                {
                    // Gửi email xác nhận tài khoản
                    await SendEmailConfirm(user, dto.Email);

                    //trả về thông tin
                    var userData = new UserViewDTO()
                    {
                        Id = user.Id,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Username = user.UserName
                    };
                    return SaveSuccess(userData);
                }
                return Error("Có lỗi xảy ra trong quá trình thực hiện. Vui lòng thử lại sau ít phút!");
            }
            catch (System.Exception ex)
            {
                ConsoleLog.WriteExceptionToConsoleLog(ex);
                return Error("Có lỗi xảy ra trong quá trình thực hiện. Vui lòng thử lại sau ít phút!");
            }
        }


        [HttpPost]
        [Route("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDTO dto)
        {
            try
            {
                var result = await _identityBizLogic.ConfirmEmailAsync(dto.UserId.ToString(), dto.Token);
                if (result) return SaveSuccess(result);
                return SaveError("Link hiện tại đã hết thời gian. Xin vui lòng đăng ký lại");
            }
            catch (Exception ex)
            {
                ConsoleLog.WriteExceptionToConsoleLog(ex);
                return SaveError();
            }
        }

        #region PRIVATE
        private async Task<bool> SendEmailConfirm(ApplicationUser user, string email)
        {
            var code = await _identityBizLogic.GenerateEmailConfirmationTokenAsync(user);
            var homeUrl = _configuration.GetSection("AppSettings:HomeUrl").Value;
            var callbackUrl = string.Format("{0}/confirm-email?userId={1}&token={2}", homeUrl, user.Id.ToString(), HttpUtility.UrlEncode(code));
            var htmlPath = PathConstant.GetFilePath(PathConstant.CustomerRegister);
            var html = System.IO.File.ReadAllText(htmlPath);
            var contentBuilder = new ContentBuilder(html);
            contentBuilder.BuildCallback(new System.Collections.Generic.List<ObjectReplace>() { new ObjectReplace() { Name = "__calback_url__", Value = callbackUrl } });
            var content = contentBuilder.GetContent() ?? $"Vui lòng xác nhận tài khoản đăng ký bằng cách nhấn vào: <a href='{callbackUrl}'>Xác nhận</a>";
            var emailDto = new EmailDTO
            (
                new string[] { email! },
                "Email xác thực tài khoản.",
                content
            );
            await _emailService.SendEmailAsync(emailDto);
            return true;
        }

        #endregion
    }
}
