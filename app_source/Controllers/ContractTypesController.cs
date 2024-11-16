using App.API.Filter;
using App.BLL.Interfaces;
using App.Entity.DTOs.ContractType;
using FS.BaseAPI;
using FS.Commons;
using FS.Commons.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{
    // [Route("api/[controller]")]
    [ApiController]
    public class ContractTypesController : BaseAPIController
    {
        private readonly IContractTypeBizLogic _contractTypeBizLogic;

        public ContractTypesController(IContractTypeBizLogic contractTypeBizLogic)
        {
            this._contractTypeBizLogic = contractTypeBizLogic;
        }

        [FSAuthorize]
        [HttpPost]
        [Route("create-update-contract-type")]
        public async Task<IActionResult> CreateUpdateContractType(ContractTypeRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) return ModelInvalid();

                var response = await _contractTypeBizLogic.CreateUpdateContractType(dto, UserId);
                if (!response.IsSuccess) return SaveError(response.Message);
                return SaveSuccess(response);
            }
            catch (Exception ex)
            {
                ConsoleLog.WriteExceptionToConsoleLog(ex);
                return Error(Constants.SomeThingWentWrong);
            }
        }


        [FSAuthorize]
        [HttpGet]
        [Route("get-contract-type-by-id/{id}")]
        public async Task<IActionResult> GetContractTypeById(long id)
        {
            try
            {
                var response = await _contractTypeBizLogic.GetContractTypeById(id);
                if (response == null) return GetNotFound(Constants.GetNotFound);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                ConsoleLog.WriteExceptionToConsoleLog(ex);
                return Error(Constants.SomeThingWentWrong);
            }
        }

        [FSAuthorize]
        [HttpPost]
        [Route("get-all-contract-types")]
        public async Task<IActionResult> GetAllContractTypes([FromBody] ContractTypeGetListDTO dto)
        {
            try
            {
                var data = await _contractTypeBizLogic.GetAllContractType(dto);
                var response = new PagingDataModel<ContractTypeViewDTO>(data, dto);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                ConsoleLog.WriteExceptionToConsoleLog(ex);
                return Error(Constants.SomeThingWentWrong);
            }
        }

        [FSAuthorize]
        [HttpGet]
        [Route("get-dropdown-list-contract-types")]
        public async Task<IActionResult> GetDropdownListContractTypes()
        {
            try
            {
                var data = await _contractTypeBizLogic.GetDropdownList();
                return GetSuccess(data);
            }
            catch (Exception ex)
            {
                ConsoleLog.WriteExceptionToConsoleLog(ex);
                return Error(Constants.SomeThingWentWrong);
            }
        }
    }
}
