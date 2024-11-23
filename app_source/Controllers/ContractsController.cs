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

                var data = await _contractBizLogic.GetContractByManager(dto, ManagerOrEmpId);
                var response = new PagingDataModel<ContractViewDTO>(data, dto);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetContractsByManager {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
    }
}
