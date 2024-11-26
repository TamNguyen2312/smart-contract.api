using App.API.Filter;
using App.BLL.Interfaces;
using App.Entity.DTOs.Contract;
using FS.BaseAPI;
using FS.Commons;
using FS.Commons.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{
    // [Route("api/[controller]")]
    [ApiController]
    public class ContractsController : BaseAPIController
    {
        private readonly IContractBizLogic _contractBizLogic;
        private readonly ILogger<ContractsController> _logger;

        public ContractsController(IContractBizLogic contractBizLogic, ILogger<ContractsController> logger)
        {
            _contractBizLogic = contractBizLogic;
            _logger = logger;
        }

        [FSAuthorize(Policy = "EmployeeRolePolicy")]
        [HttpPost]
        [Route("create-contract")]
        public async Task<IActionResult> CreateContract([FromBody] ContractRequestDTO dto)
        {
            try
            {
                if (await IsTokenInvoked()) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (!ModelState.IsValid) return ModelInvalid();

                var response = await _contractBizLogic.CreateContract(dto, UserId);
                if (!response.IsSuccess) return SaveError(response.Message);
                return SaveSuccess(response.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("CreateContract {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        [FSAuthorize(Policy = "AdminManagerPolicy")]
        [HttpPut]
        [Route("update-contract")]
        public async Task<IActionResult> UpdateContract([FromBody] ContractUpdateDTO dto)
        {
            try
            {
                if (await IsTokenInvoked()) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (!ModelState.IsValid) return ModelInvalid();
                
                if (IsManager)
                {
                    var hasAccess = await _contractBizLogic.HasManagerAccessToContract(ManagerOrEmpId, dto.Id);
                    if (!hasAccess) return GetForbidden();
                }

                var response = await _contractBizLogic.UpdateContract(dto, UserId);
                if (!response.IsSuccess) return SaveError(response.Message);
                return SaveSuccess(response.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("UpdateContract {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        [FSAuthorize(Policy = "AdminRolePolicy")]
        [HttpPost]
        [Route("assign-contract-to-department")]
        public async Task<IActionResult> AssignContractToDepartment ([FromBody] ContractAssignRequestDTO dto)
        {
            try
            {
                if (await IsTokenInvoked()) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (!ModelState.IsValid) return ModelInvalid();
                
                var response = await _contractBizLogic.AssignContractToDepartment(dto, UserId);
                if (!response.IsSuccess) return SaveError(response.Message);
                return SaveSuccess(response.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("AssignContractToDepartment {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        [FSAuthorize(Policy = "AdminRolePolicy")]
        [HttpPut]
        [Route("update-contract-department-assign")]
        public async Task<IActionResult> UpdateContractDepartmentAssign ([FromBody] ContractAssignUpdateDTO dto)
        {
            try
            {
                if (await IsTokenInvoked()) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (!ModelState.IsValid) return ModelInvalid();
                
                var response = await _contractBizLogic.UpdateContractDepartmentAssign(dto, UserId);
                if (!response.IsSuccess) return SaveError(response.Message);
                return SaveSuccess(response.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("UpdateContractDepartmentAssign {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        [FSAuthorize(Policy = "ManagerRolePolicy")]
        [HttpPost]
        [Route("get-contracts-department-assign-by-manager")]
        public async Task<IActionResult> GetContractDepartmentAssignsByManager([FromBody] ContractDepartmentAssignGetListDTO dto)
        {
            try
            {
                if (await IsTokenInvoked()) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (!ModelState.IsValid) return ModelInvalid();

                if (!dto.IsValidOrderDate())
                {
                    ModelState.AddModelError("OrderDate", "OrderDate không hợp lệ");
                    return ModelInvalid();
                }

                var data = await _contractBizLogic.GetContractDepartmentAssignsByManager(dto, ManagerOrEmpId);
                var response = new PagingDataModel<ContractDepartmentAssignViewDTO>(data, dto);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetContractDepartmentAssignsByManager {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        [FSAuthorize(Policy = "AdminRolePolicy")]
        [HttpPost]
        [Route("get-contracts-department-assign-by-admin")]
        public async Task<IActionResult> GetContractDepartmentAssignsByAdmin([FromBody] ContractDepartmentAssignGetListDTO dto)
        {
            try
            {
                if (await IsTokenInvoked()) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (!ModelState.IsValid) return ModelInvalid();

                if (!dto.IsValidOrderDate())
                {
                    ModelState.AddModelError("OrderDate", "OrderDate không hợp lệ");
                    return ModelInvalid();
                }

                var data = await _contractBizLogic.GetContractDepartmentAssignsByAdmin(dto);
                var response = new PagingDataModel<ContractDepartmentAssignViewDTO>(data, dto);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetContractDepartmentAssignsByAdmin {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        
        [FSAuthorize(Policy = "AdminManagerPolicy")]
        [HttpGet]
        [Route("get-contract-department-assign/{contractId}/{departmentId}")]
        public async Task<IActionResult> GetContractDepartmentAssign([FromRoute] long contractId, [FromRoute]long departmentId)
        {
            try
            {
                if (await IsTokenInvoked()) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (IsManager)
                {
                    var hasAccess =
                        await _contractBizLogic.HasManagerAccessToContractDepartmetnAssign(contractId, departmentId,
                            ManagerOrEmpId);
                    if (!hasAccess) return GetForbidden();
                }

                var response = await _contractBizLogic.GetContractDepartmentAssign(contractId, departmentId);
                if (response == null) return GetNotFound(Constants.GetNotFound);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetContractDepartmentAssign {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        
        [FSAuthorize(Policy = "ManagerRolePolicy")]
        [HttpPost]
        [Route("assign-contract-to-employee")]
        public async Task<IActionResult> AssignContractToEmployee ([FromBody] EmpContractRequestDTO dto)
        {
            try
            {
                if (await IsTokenInvoked()) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (!ModelState.IsValid) return ModelInvalid();
                
                var response = await _contractBizLogic.AssignContractToEmployee(dto, UserId);
                if (!response.IsSuccess) return SaveError(response.Message);
                return SaveSuccess(response.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("AssignContractToEmployee {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        
        [FSAuthorize(Policy = "EmployeeRolePolicy")]
        [HttpPost]
        [Route("get-contract-employee-assigns-by-employee")]
        public async Task<IActionResult> GetContractEmployeeAssignsByEmployee([FromBody] EmpContractGetListDTO dto)
        {
            try
            {
                if (await IsTokenInvoked()) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (!ModelState.IsValid) return ModelInvalid();

                if (!dto.IsValidOrderDate())
                {
                    ModelState.AddModelError("OrderDate", "OrderDate không hợp lệ");
                    return ModelInvalid();
                }

                var data = await _contractBizLogic.GetEmpContractsByEmployee(dto, ManagerOrEmpId);
                var response = new PagingDataModel<EmpContractViewDTO>(data, dto);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetContractEmployeeAssignsByEmployee {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        
        [FSAuthorize(Policy = "ManagerRolePolicy")]
        [HttpPost]
        [Route("get-contract-employee-assigns-by-manager")]
        public async Task<IActionResult> GetContractEmployeeAssignsByManager([FromBody] EmpContractGetListDTO dto)
        {
            try
            {
                if (await IsTokenInvoked()) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (!ModelState.IsValid) return ModelInvalid();

                if (!dto.IsValidOrderDate())
                {
                    ModelState.AddModelError("OrderDate", "OrderDate không hợp lệ");
                    return ModelInvalid();
                }

                var data = await _contractBizLogic.GetEmpContractsByManager(dto, ManagerOrEmpId);
                var response = new PagingDataModel<EmpContractViewDTO>(data, dto);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetContractEmployeeAssignsByManager {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        
        [FSAuthorize(Policy = "ManagerEmployeePolicy")]
        [HttpPost]
        [Route("get-contract-employee-assign/{employeeId}/{contractId}")]
        public async Task<IActionResult> GetContractEmployeeAssign([FromRoute] string employeeId, [FromRoute]long contractId)
        {
            try
            {
                if (await IsTokenInvoked()) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (IsManager)
                {
                    var managerAccess =
                        await _contractBizLogic.HasManagerAccessToEmpContract(employeeId, contractId, ManagerOrEmpId);
                    if (!managerAccess) return GetForbidden();
                }
                
                if (IsEmployee)
                {
                    var employeeAccess =
                        await _contractBizLogic.HasEmployeeAccessToEmpContract(employeeId, contractId, ManagerOrEmpId);
                    if (!employeeAccess) return GetForbidden();
                }

                var response = await _contractBizLogic.GetEmpContract(employeeId, contractId);
                if (response == null) return GetNotFound(Constants.GetNotFound);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetContractEmployeeAssign {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        
        [FSAuthorize(Policy = "ManagerRolePolicy")]
        [HttpPost]
        [Route("get-contracts-by-manager")]
        public async Task<IActionResult> GetContractsByManager([FromBody] ContractGetListDTO dto)
        {
            try
            {
                if (await IsTokenInvoked()) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (!ModelState.IsValid) return ModelInvalid();

                if (!dto.IsValidOrderDate())
                {
                    ModelState.AddModelError("OrderDate", "OrderDate không hợp lệ");
                    return ModelInvalid();
                }

                var data = await _contractBizLogic.GetContractsByManager(dto, ManagerOrEmpId);
                var response = new PagingDataModel<ContractViewDTO>(data, dto);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetContractsByManager {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        [FSAuthorize(Policy = "EmployeeRolePolicy")]
        [HttpPost]
        [Route("get-contracts-by-employee")]
        public async Task<IActionResult> GetContractsByEmployee([FromBody] ContractGetListDTO dto)
        {
            try
            {
                if (await IsTokenInvoked()) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (!ModelState.IsValid) return ModelInvalid();

                if (!dto.IsValidOrderDate())
                {
                    ModelState.AddModelError("OrderDate", "OrderDate không hợp lệ");
                    return ModelInvalid();
                }

                var data = await _contractBizLogic.GetContractsByEmployee(dto, ManagerOrEmpId);
                var response = new PagingDataModel<ContractViewDTO>(data, dto);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetContractsByEmployee {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        [FSAuthorize]
        [HttpGet]
        [Route("get-contract-by-id/{id}")]
        public async Task<IActionResult> GetContract([FromRoute] long id)
        {
            try
            {
                if (await IsTokenInvoked()) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (IsManager)
                {
                    var hasAccess = await _contractBizLogic.HasManagerAccessToContract(ManagerOrEmpId, id);
                    if (!hasAccess) return GetForbidden();
                }
                
                if (IsEmployee)
                {
                    var hasAccess = await _contractBizLogic.HasEmployeeAccessToContract(ManagerOrEmpId, id);
                    if (!hasAccess) return GetForbidden();
                }
                
                var response = await _contractBizLogic.GetContract(id);
                if (response == null) return GetNotFound(Constants.GetNotFound);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetContract {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        [FSAuthorize(Policy = "AdminRolePolicy")]
        [HttpPost]
        [Route("get-contracts-by-admin")]
        public async Task<IActionResult> GetContractsByAdmin([FromBody] ContractGetListDTO dto)
        {
            try
            {
                if (await IsTokenInvoked()) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (!ModelState.IsValid) return ModelInvalid();

                if (!dto.IsValidOrderDate())
                {
                    ModelState.AddModelError("OrderDate", "OrderDate không hợp lệ");
                    return ModelInvalid();
                }

                var data = await _contractBizLogic.GetContractsByAdmin(dto);
                var response = new PagingDataModel<ContractViewDTO>(data, dto);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetContractsByAdmin {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
    }
}
