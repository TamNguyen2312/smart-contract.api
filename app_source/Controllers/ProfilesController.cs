using App.BLL.Interfaces;
using App.Entity.DTOs.Profile;
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
        private readonly IManagerBizLogic _managerBizLogic;
        private readonly IProfileBizLogic _profileBizLogic;

        public ProfilesController(IIdentityBizLogic identityBizLogic,
            IEmployeeBizLogic employeeBizLogic, IManagerBizLogic managerBizLogic, IProfileBizLogic profileBizLogic)
        {
            _identityBizLogic = identityBizLogic;
            _employeeBizLogic = employeeBizLogic;
            _managerBizLogic = managerBizLogic;
            _profileBizLogic = profileBizLogic;
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
                
                
                if (IsManager)
                {
                    var manager = await _managerBizLogic.GetManager(UserId);
                    if (manager == null) return GetNotFound(Constants.GetNotFound);
                    return GetSuccess(manager);
                }

                var userView = await _profileBizLogic.GetPersonalProfile(UserId);
                return userView == null ? GetNotFound(Constants.GetNotFound) : GetSuccess(userView);
            }
            catch(Exception ex)
            {
                ConsoleLog.WriteExceptionToConsoleLog(ex);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        
        /// <summary>
        /// This is used to edit PERSONAL profile of the employee 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("edit-personal-profile")]
        public async Task<IActionResult> EditPersonalProfile([FromBody] PersonalProfileDTO dto)
        {
            try
            {
                var isInvoked = await IsTokenInvoked();
                if (isInvoked) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (!ModelState.IsValid) return ModelInvalid();

                if (!dto.IsValidGender())
                {
                    ModelState.AddModelError("Gender", "Giới tính không hợp lệ");
                    return ModelInvalid();
                }

                var tryEdit = await _profileBizLogic.EditPersonalProfile(dto, UserId);
                if (!tryEdit.IsSuccess) return SaveError();
                return SaveSuccess(tryEdit);
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
