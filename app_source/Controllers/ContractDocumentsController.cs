using App.API.Filter;
using App.BLL.Interfaces;
using App.Entity.DTOs.Contract;
using App.Entity.DTOs.ContractDocument;
using FS.BaseAPI;
using FS.Commons;
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
    }
}
