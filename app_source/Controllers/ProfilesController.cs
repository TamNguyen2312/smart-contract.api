using App.API.Filter;
using App.BLL.Interfaces;
using App.Entity.DTOs.Employee;
using App.Entity.DTOs.Manager;
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
        private readonly ILogger<ProfilesController> _logger;

        public ProfilesController(IIdentityBizLogic identityBizLogic,
            IEmployeeBizLogic employeeBizLogic, IManagerBizLogic managerBizLogic, IProfileBizLogic profileBizLogic, ILogger<ProfilesController> logger)
        {
            _identityBizLogic = identityBizLogic;
            _employeeBizLogic = employeeBizLogic;
            _managerBizLogic = managerBizLogic;
            _profileBizLogic = profileBizLogic;
            _logger = logger;
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
                _logger.LogError("GetPersonalProfile {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }


        /// <summary>
        /// This is used to edit PERSONAL profile of the employee 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        [Route("edit-personal-profile")]
        public async Task<IActionResult> EditPersonalProfile([FromBody] PersonalProfileDto dto)
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
                _logger.LogError("EditPersonalProfile {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }

        #endregion


        #region ADMIN

        /// <summary>
        /// This is used to get all manager profiles for admin
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [FSAuthorize(Policy = "AdminRolePolicy")]
        [HttpPost]
        [Route("get-all-manager-profiles")]
        public async Task<IActionResult> GetAllManagerProfiles([FromBody] AccountGetListDTO dto)
        {
            try
            {
                var isInvoked = await IsTokenInvoked();
                if (isInvoked) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (!ModelState.IsValid) return ModelInvalid();
                
                if (!dto.IsValidOrderDate())
                {
                    ModelState.AddModelError("OrderDate", "OrderDate không hợp lệ.");
                    return ModelInvalid();
                }

                var data = await _managerBizLogic.GetAllManager(dto);
                var response = new PagingDataModel<ManagerViewDTO>(data, dto);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetAllManagerProfiles {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        [FSAuthorize(Policy = "AdminManagerPolicy")]
        [HttpPost]
        [Route("get-all-employee-profiles")]
        public async Task<IActionResult> GetAllEmployeeProfiles([FromBody] AccountGetListDTO dto)
        {
            try
            {
                var isInvoked = await IsTokenInvoked();
                if (isInvoked) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (!ModelState.IsValid) return ModelInvalid();

                if (!dto.IsValidOrderDate())
                {
                    ModelState.AddModelError("OrderDate", "OrderDate không hợp lệ.");
                    return ModelInvalid();
                }

                if (IsManager)
                {
                    var managerView = await _managerBizLogic.GetManager(UserId);
                    dto.DepartmentId = managerView.DepartmentId;
                }

                var data = await _employeeBizLogic.GetAllEmployee(dto);
                var response = new PagingDataModel<EmployeeViewDTO>(data, dto);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetAllEmployeeProfiles {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }

        /// <summary>
        /// This is used to get a single profile for admin
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [FSAuthorize(Policy = "AdminManagerPolicy")]
        [HttpGet]
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
                
                if (IsManager)
                {
                    if (!userRoles.Contains(SystemRoleConstants.EMPLOYEE)) return Forbid();
                    var loggedManager = await _managerBizLogic.GetManager(UserId);
                    var empViewForManager = await _employeeBizLogic.GetEmployee(user, userRoles.ToList(), loggedManager);
                    if (empViewForManager == null) return GetNotFound(Constants.GetNotFound);
                    return GetSuccess(empViewForManager);
                }
                
                if (userRoles.Contains(SystemRoleConstants.EMPLOYEE))
                {
                    var empView = await _employeeBizLogic.GetEmployee(user, userRoles.ToList());
                    if (empView == null) return GetNotFound(Constants.GetNotFound);
                    return GetSuccess(empView);
                }
                
                if (userRoles.Contains(SystemRoleConstants.MANAGER))
                {
                    var managerView = await _managerBizLogic.GetManager(user, userRoles.ToList());
                    if (managerView == null) return GetNotFound(Constants.GetNotFound);
                    return GetSuccess(managerView);
                }

                var userView = new UserViewDTO(user, userRoles.ToList());
                return GetSuccess(userView);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetProfileByUserId {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        
        /// <summary>
        /// This is used to edit profile of any user for admin
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [FSAuthorize(Policy = "AdminRolePolicy")]
        [HttpPut]
        [Route("edit-profile-by-userId")]
        public async Task<IActionResult> EditProfile([FromBody] ProfileUpdateDto dto)
        {
            try
            {
                if (await IsTokenInvoked()) return GetUnAuthorized(Constants.GetUnAuthorized);
                
                if (!ModelState.IsValid) return ModelInvalid();

                if (!dto.IsValidGender())
                {
                    ModelState.AddModelError("Gender", "Giới tính không hợp lệ.");
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
                    return ModelInvalid();
                }

                var response = await _profileBizLogic.EditProfile(dto);
                if (!response.IsSuccess) return SaveError(response.Message);
                return SaveSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("EditProfile {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        #endregion
    }
}