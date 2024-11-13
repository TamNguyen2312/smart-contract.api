using App.API.Filter;
using App.BLL.Interfaces;
using App.Entity.DTOs.Department;
using FS.BaseAPI;
using FS.Commons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{
    // [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : BaseAPIController
    {
        private readonly IDepartmentBizLogic _departmentBizLogic;

        public DepartmentsController(IDepartmentBizLogic departmentBizLogic)
        {
            _departmentBizLogic = departmentBizLogic;
        }

        [FSAuthorize(Policy = "AdminRolePolicy")]
        [HttpPost]
        [Route("create-update-department")]
        public async Task<IActionResult> CreateUpdateDepartment(DepartmentRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return ModelInvalid();
                
                var isInvoke = await IsTokenInvoked();
                if (isInvoke) return GetUnAuthorized(Constants.GetUnAuthorized);

                var response = await _departmentBizLogic.CreateUpdateDepartment(dto, UserId);
                if (!response.IsSuccess) return SaveError(response.Message);
                return SaveSuccess(response);
            }
            catch (Exception e)
            {
                ConsoleLog.WriteExceptionToConsoleLog(e);
                return Error(Constants.SomeThingWentWrong);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("get-dropdown-department")]
        public async Task<IActionResult> GetDropdownDepartment()
        {
            try
            {
                var data = await _departmentBizLogic.GetDropDownDepartment();
                return GetSuccess(data);
            }
            catch (Exception e)
            {
                ConsoleLog.WriteExceptionToConsoleLog(e);
                return Error(Constants.SomeThingWentWrong);
            }   
        }
    }
}