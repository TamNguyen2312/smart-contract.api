using App.API.Filter;
using App.BLL.Interfaces;
using App.Entity.DTOs.ContractAppendix;
using App.Entity.DTOs.ContractTerm;
using FS.BaseAPI;
using FS.Commons;
using FS.Commons.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{
    // [Route("api/[controller]")]
    [ApiController]
    public class ContractAppendicesController : BaseAPIController
    {
        private readonly IContractAppendixBizLogic _contractAppendixBizLogic;
        private readonly IContractBizLogic _contractBizLogic;
        private readonly ILogger<ContractAppendicesController> _logger;

        public ContractAppendicesController(IContractAppendixBizLogic contractAppendixBizLogic,
                                            IContractBizLogic contractBizLogic,
                                            ILogger<ContractAppendicesController> logger)
        {
            _contractAppendixBizLogic = contractAppendixBizLogic;
            _contractBizLogic = contractBizLogic;
            _logger = logger;
        }
        
        [FSAuthorize(Policy = "ManagerEmployeePolicy")]
        [HttpPost]
        [Route("create-update-contract-appendix")]
        public async Task<IActionResult> CreateUpdateContractAppendix([FromBody] ContractAppendixRequestDTO dto)
        {
            try
            {
                if (await IsTokenInvoked()) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (!ModelState.IsValid) return ModelInvalid();

                if (IsManager)
                {
                    var managerAccess =
                        await _contractBizLogic.HasManagerAccessToContract(ManagerOrEmpId, dto.ContractId);
                    if (!managerAccess) return SaveError($"Bạn không đủ quyền hạn truy cập hợp đồng {dto.ContractId}");
                }
                
                if (IsEmployee)
                {
                    var employeeAccess =
                        await _contractBizLogic.HasEmployeeAccessToContract(ManagerOrEmpId, dto.ContractId);
                    if (!employeeAccess) return SaveError($"Bạn không đủ quyền hạn truy cập hợp đồng {dto.ContractId}");
                }

                var response = await _contractAppendixBizLogic.CreateUpdateContractAppendix(dto, UserId);
                if (!response.IsSuccess) return SaveError(response.Message);
                return SaveSuccess(response.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("CreateUpdateContractAppendix {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        [FSAuthorize(Policy = "ManagerEmployeePolicy")]
        [HttpPost]
        [Route("get-contract-appendices-by-contract/{contractId}")]
        public async Task<IActionResult> GetContractAppendicesByContract([FromBody] ContractAppendixGetListDTO dto, [FromRoute] long contractId)
        {
            try
            {
                if (await IsTokenInvoked()) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (IsManager)
                {
                    var managerAccess =
                        await _contractBizLogic.HasManagerAccessToContract(ManagerOrEmpId, contractId);
                    if (!managerAccess) return GetError($"Bạn không đủ quyền hạn truy cập hợp đồng {contractId}");
                }
                
                if (IsEmployee)
                {
                    var employeeAccess =
                        await _contractBizLogic.HasEmployeeAccessToContract(ManagerOrEmpId, contractId);
                    if (!employeeAccess) return GetError($"Bạn không đủ quyền hạn truy cập hợp đồng {contractId}");
                }

                if (!ModelState.IsValid) return ModelInvalid();

                if (!dto.IsValidOrderDate())
                {
                    ModelState.AddModelError("OrderDate", "OrderDate không hợp lệ");
                    return ModelInvalid();
                }
                

                var data = await _contractAppendixBizLogic.GetContractAppendicesByContract(dto, contractId);
                var response = new PagingDataModel<ContractAppendixViewDTO>(data, dto);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetContractAppendicesByContract {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        
        [FSAuthorize(Policy = "ManagerEmployeePolicy")]
        [HttpGet]
        [Route("get-contract-appendix/{contractId}/{contractAppendixId}")]
        public async Task<IActionResult> GetContractAppendix([FromRoute] long contractId, [FromRoute] long contractAppendixId)
        {
            try
            {
                if (await IsTokenInvoked()) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (IsManager)
                {
                    var managerAccess =
                        await _contractBizLogic.HasManagerAccessToContract(ManagerOrEmpId, contractId);
                    if (!managerAccess) return GetError($"Bạn không đủ quyền hạn truy cập hợp đồng {contractId}");
                }
                
                if (IsEmployee)
                {
                    var employeeAccess =
                        await _contractBizLogic.HasEmployeeAccessToContract(ManagerOrEmpId, contractId);
                    if (!employeeAccess) return GetError($"Bạn không đủ quyền hạn truy cập hợp đồng {contractId}");
                }
                

                var response = await _contractAppendixBizLogic.GetContractAppendix(contractId, contractAppendixId);
                if (response == null) return GetNotFound(Constants.GetNotFound);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetContractAppendix {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
    }
}
