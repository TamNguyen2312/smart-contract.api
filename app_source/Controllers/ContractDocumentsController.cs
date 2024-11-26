using App.API.Filter;
using App.BLL.Interfaces;
using App.Entity.DTOs.Contract;
using App.Entity.DTOs.ContractDocument;
using FS.BaseAPI;
using FS.Commons;
using FS.Commons.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{
    // [Route("api/[controller]")]
    [ApiController]
    public class ContractDocumentsController : BaseAPIController
    {
        private readonly IContractDocumentBizLogic _contractDocumentBizLogic;
        private readonly IContractBizLogic _contractBizLogic;
        private readonly ILogger<ContractDocumentsController> _logger;

        public ContractDocumentsController(IContractDocumentBizLogic contractDocumentBizLogic,
                                            IContractBizLogic contractBizLogic,
                                            ILogger<ContractDocumentsController> logger)
        {
            _contractDocumentBizLogic = contractDocumentBizLogic;
            _contractBizLogic = contractBizLogic;
            _logger = logger;
        }
        
        [FSAuthorize(Policy = "ManagerEmployeePolicy")]
        [HttpPost]
        [Route("create-update-contract-document")]
        public async Task<IActionResult> CreateUpdateContractDocument([FromBody] ContractDocumentRequestDTO dto)
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

                var response = await _contractDocumentBizLogic.CreateUpdateContractDocument(dto, UserId);
                if (!response.IsSuccess) return SaveError(response.Message);
                return SaveSuccess(response.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("CreateUpdateContractDocument {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        [FSAuthorize(Policy = "ManagerEmployeePolicy")]
        [HttpPost]
        [Route("get-contract-documents-by-contract/{contractId}")]
        public async Task<IActionResult> GetContractDocumentsByContract([FromBody] ContractDocumentGetListDTO dto, [FromRoute] long contractId)
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
                

                var data = await _contractDocumentBizLogic.GetContractDocumentsByContract(dto, contractId);
                var response = new PagingDataModel<ContractDocumentViewDTO>(data, dto);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetContractDocumentsByContract {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        [FSAuthorize(Policy = "ManagerEmployeePolicy")]
        [HttpGet]
        [Route("get-contract-document/{contractId}/{contractDocumentId}")]
        public async Task<IActionResult> GetContractDocument([FromRoute] long contractId, [FromRoute] long contractDocumentId)
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
                
                var response = await _contractDocumentBizLogic.GetContractDocument(contractId, contractDocumentId);
                if (response == null) return GetNotFound(Constants.GetNotFound);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetContractDocument {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
    }
}
