using FS.BaseAPI;
using FS.BLL.Services.Interfaces;
using FS.Commons;
using FS.Commons.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{
    // [Route("api/[controller]")]
    [ApiController]
    public class ProfilesController : BaseAPIController
    {
        private readonly IIdentityBizLogic _identityBizLogic;

        public ProfilesController(IIdentityBizLogic identityBizLogic)
        {
            _identityBizLogic = identityBizLogic;
        }

        #region COMMON

        [Authorize]
        [HttpGet]
        [Route("get-personal-profile")]
        public async Task<IActionResult> GetPersonalProfile()
        {
            try
            {
                var isInvoked = await IsTokenInvoked();
                if (isInvoked) return GetUnAuthorized(Constants.GetUnAuthorized);

                var user = await _identityBizLogic.GetByIdAsync(UserId);
                if (user == null) return GetNotFound(Constants.GetNotFound);
                var userRoles = await _identityBizLogic.GetRolesAsync(UserId);
                var userView = new UserViewDTO(user, userRoles.ToList());
                return userView == null ? GetNotFound(Constants.GetNotFound) : GetSuccess(userView);
            }
            catch(Exception ex)
            {
                ConsoleLog.WriteExceptionToConsoleLog(ex);
                return Error(Constants.SomeThingWentWrong);
            }
        }

        #endregion
    }
}
