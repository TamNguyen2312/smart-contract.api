using App.API.Filter;
using App.BLL.Interfaces;
using App.Entity.DTOs.AppendixDocument;
using App.Entity.DTOs.ContractAppendix;
using FS.BaseAPI;
using FS.Commons;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{
    // [Route("api/[controller]")]
    [ApiController]
    public class AppendixDocumentsController : BaseAPIController
    {
        private readonly IAppendixDocumentBizLogic _appendixDocumentBizLogic;
        private readonly IContractBizLogic _contractBizLogic;
        private readonly ILogger<AppendixDocumentsController> _logger;
        private readonly IContractAppendixBizLogic _contractAppendixBizLogic;

        public AppendixDocumentsController(IAppendixDocumentBizLogic appendixDocumentBizLogic,
            IContractBizLogic contractBizLogic, ILogger<AppendixDocumentsController> logger, IContractAppendixBizLogic contractAppendixBizLogic)
        {
            _appendixDocumentBizLogic = appendixDocumentBizLogic;
            _contractBizLogic = contractBizLogic;
            _logger = logger;
            _contractAppendixBizLogic = contractAppendixBizLogic;
        }
        
        [FSAuthorize(Policy = "ManagerEmployeePolicy")]
        [HttpPost]
        [Route("create-update-appendix-document")]
        public async Task<IActionResult> CreateUpdateAppendixDocument([FromBody] AppendixDocumentRequestDTO dto)
        {
            try
            {
                if (await IsTokenInvoked()) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (!ModelState.IsValid) return ModelInvalid();

                if (IsManager)
                {
                    var managerAccess =
                        await _contractAppendixBizLogic.HasManagerAccessToAppendix(ManagerOrEmpId, dto.AppendixId);
                    if (!managerAccess) return SaveError($"Bạn không đủ quyền hạn truy cập phụ lục {dto.AppendixId}");
                }
                
                if (IsEmployee)
                {
                    var employeeAccess =
                        await _contractAppendixBizLogic.HasEmployeeAccessToAppendix(ManagerOrEmpId, dto.AppendixId);
                    if (!employeeAccess) return SaveError($"Bạn không đủ quyền hạn truy cập hợp đồng {dto.AppendixId}");
                }

                var response = await _appendixDocumentBizLogic.CreateUpdateAppendixDocument(dto, UserId);
                if (!response.IsSuccess) return SaveError(response.Message);
                return SaveSuccess(response.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("CreateUpdateAppendixDocument {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
    }
}