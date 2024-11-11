using App.API.Filter;
using App.BLL.Interfaces;
using App.Entity.DTOs.Profile;
using FS.BaseAPI;
using FS.BaseModels;
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

        /// <summary>
        /// This is used to get PERSONAL profile of every user
        /// </summary>
        /// <returns></returns>
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
            catch (Exception ex)
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
            catch (Exception ex)
            {
                ConsoleLog.WriteExceptionToConsoleLog(ex);
                return Error(Constants.SomeThingWentWrong);
            }
        }

        #endregion


        #region ADMIN

        /// <summary>
        /// This is used to get all account profiles for admin
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [FSAuthorize(Policy = "AdminRolePolicy")]
        [HttpPost]
        [Route("get-all-profiles")]
        public async Task<IActionResult> GetAllProfiles([FromBody] AccountGetListDTO dto)
        {
            try
            {
                var isInvoked = await IsTokenInvoked();
                if (isInvoked) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (!ModelState.IsValid) return ModelInvalid();

                var userViewDtos = new List<object>();
                var users = await _identityBizLogic.GetAll(dto);

                foreach (var user in users)
                {
                    var userRoles = await _identityBizLogic.GetRolesAsync(user.Id);
                    if (userRoles.Contains(SystemRoleConstants.MANAGER))
                    {
                        var managerView = await _managerBizLogic.GetManager(user, userRoles.ToList());
                        userViewDtos.Add(managerView);
                    }
                    else if (userRoles.Contains(SystemRoleConstants.EMPLOYEE))
                    {
                        var empView = await _employeeBizLogic.GetEmployee(user, userRoles.ToList());
                        userViewDtos.Add(empView);
                    }
                    else
                    {
                        var userView = new UserViewDTO(user, userRoles.ToList());
                        userViewDtos.Add(userView);
                    }
                }

                var response = new PagingDataModel<object>(userViewDtos, dto);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                ConsoleLog.WriteExceptionToConsoleLog(ex);
                return Error(Constants.SomeThingWentWrong);
            }
        }

        [FSAuthorize(Policy = "AdminRolePolicy")]
        [HttpPost]
        [Route("get-profile-by-userId/{userId}")]
        public async Task<IActionResult> GetProfileByUserId(long userId)
        {
            try
            {
                var isInvoked = await IsTokenInvoked();
                if (isInvoked) return GetUnAuthorized(Constants.GetUnAuthorized);

                var user = await _identityBizLogic.GetByIdAsync(userId);
                if (user == null) return GetNotFound(Constants.GetNotFound);

                var userRoles = await _identityBizLogic.GetRolesAsync(user.Id);
                if (userRoles.Contains(SystemRoleConstants.MANAGER))
                {
                    var managerView = await _managerBizLogic.GetManager(user, userRoles.ToList());
                    return GetSuccess(managerView);
                }
                
                if (userRoles.Contains(SystemRoleConstants.EMPLOYEE))
                {
                    var empView = await _employeeBizLogic.GetEmployee(user, userRoles.ToList());
                    return GetSuccess(empView);
                }

                var userView = new UserViewDTO(user, userRoles.ToList());
                return GetSuccess(userView);
            }
            catch (Exception ex)
            {
                ConsoleLog.WriteExceptionToConsoleLog(ex);
                return Error(Constants.SomeThingWentWrong);
            }
        }

        #endregion
    }
}