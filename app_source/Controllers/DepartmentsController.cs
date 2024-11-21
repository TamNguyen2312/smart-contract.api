using App.API.Filter;
using App.BLL.Interfaces;
using App.Entity.DTOs.Department;
using FS.BaseAPI;
using FS.Commons;
using FS.Commons.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{
    // [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : BaseAPIController
    {
        private readonly IDepartmentBizLogic _departmentBizLogic;
        private readonly ILogger<DepartmentsController> _logger;

        public DepartmentsController(IDepartmentBizLogic departmentBizLogic, ILogger<DepartmentsController> logger)
        {
            _departmentBizLogic = departmentBizLogic;
            _logger = logger;
        }

        [FSAuthorize(Policy = "AdminRolePolicy")]
        [HttpPost]
        [Route("create-update-department")]
        public async Task<IActionResult> CreateUpdateDepartment(DepartmentRequestDto dto)
        {
            try
            {
                var isInvoke = await IsTokenInvoked();
                if (isInvoke) return GetUnAuthorized(Constants.GetUnAuthorized);
                
                if (!ModelState.IsValid) return ModelInvalid();

                var response = await _departmentBizLogic.CreateUpdateDepartment(dto, UserId);
                if (!response.IsSuccess) return SaveError(response.Message);
                return SaveSuccess(response);
            }
            catch (Exception e)
            {
                _logger.LogError("CreateUpdateDepartment {0} {1}", e.Message, e.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }

        [FSAuthorize(Policy = "AdminRolePolicy")]
        [HttpGet]
        [Route("get-dropdown-department")]
        public async Task<IActionResult> GetDropdownDepartment()
        {
            try
            {
                var isInvoke = await IsTokenInvoked();
                if (isInvoke) return GetUnAuthorized(Constants.GetUnAuthorized);
                
                var data = await _departmentBizLogic.GetDropDownDepartment();
                return GetSuccess(data);
            }
            catch (Exception e)
            {
                _logger.LogError("GetDropdownDepartment {0} {1}", e.Message, e.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }   
        }
        
        
        [FSAuthorize(Policy = "AdminRolePolicy")]
        [HttpPost]
        [Route("get-all-departments")]
        public async Task<IActionResult> GetAllDepartments(DepartmentGetListDTO dto)
        {
            try
            {
                var isInvoke = await IsTokenInvoked();
                if (isInvoke) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (!ModelState.IsValid) return ModelInvalid();

                if (!dto.IsValidOrderDate())
                {
                    ModelState.AddModelError("OrderDate", "OrderDate không hợp lệ.");
                    return ModelInvalid();
                }
                
                var data = await _departmentBizLogic.GetAllDepartments(dto);
                var response = new PagingDataModel<DepartmentViewDTO>(data, dto);
                return GetSuccess(response);
            }
            catch (Exception e)
            {
                _logger.LogError("GetAllDepartments {0} {1}", e.Message, e.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }   
        }
        
        [FSAuthorize(Policy = "AdminRolePolicy")]
        [HttpGet]
        [Route("get-department-by-id/{id}")]
        public async Task<IActionResult> GetDepartment([FromRoute]long id)
        {
            try
            {
                var isInvoke = await IsTokenInvoked();
                if (isInvoke) return GetUnAuthorized(Constants.GetUnAuthorized);
                
                var response = await _departmentBizLogic.GetDepartment(id);
                if (response == null) return GetNotFound(Constants.GetNotFound);
                return GetSuccess(response);
            }
            catch (Exception e)
            {
                _logger.LogError("GetDepartment {0} {1}", e.Message, e.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }   
        }
    }
}