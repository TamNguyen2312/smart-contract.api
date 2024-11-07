using App.BLL.Interfaces;
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
        private readonly IEmployeeBizLogic _employeeBizLogic;

        public ProfilesController(IIdentityBizLogic identityBizLogic, IEmployeeBizLogic employeeBizLogic)
        {
            _identityBizLogic = identityBizLogic;
            _employeeBizLogic = employeeBizLogic;
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

                if (IsEmployee)
                {
                    var employee = await _employeeBizLogic.GetEmployee(UserId);
                    if (employee == null) return GetNotFound(Constants.GetNotFound);
                    return GetSuccess(employee);
                }
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
