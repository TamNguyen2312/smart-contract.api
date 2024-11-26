using App.API.Filter;
using App.BLL.Interfaces;
using App.Entity.DTOs.ContractDocument;
using App.Entity.DTOs.ContractTerm;
using FS.BaseAPI;
using FS.Commons;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{
    // [Route("api/[controller]")]
    [ApiController]
    public class ContractTermsController : BaseAPIController
    {
        private readonly IContractTermBizLogic _contractTermBizLogic;
        private readonly IContractBizLogic _contractBizLogic;
        private readonly ILogger<ContractTermsController> _logger;

        public ContractTermsController(IContractTermBizLogic contractTermBizLogic, IContractBizLogic contractBizLogic,
            ILogger<ContractTermsController> logger)
        {
            _contractTermBizLogic = contractTermBizLogic;
            _contractBizLogic = contractBizLogic;
            _logger = logger;
        }
        
        [FSAuthorize(Policy = "ManagerEmployeePolicy")]
        [HttpPost]
        [Route("create-update-contract-term")]
        public async Task<IActionResult> CreateUpdateContractTerm([FromBody] ContractTermRequestDTO dto)
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

                var response = await _contractTermBizLogic.CreateUpdateContractTerm(dto, UserId);
                if (!response.IsSuccess) return SaveError(response.Message);
                return SaveSuccess(response.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("CreateUpdateContractTerm {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
    }
}