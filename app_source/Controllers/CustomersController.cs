using App.BLL.Interfaces;
using App.Entity.DTOs.Customer;
using App.Entity.Entities;
using FS.BaseAPI;
using FS.Commons;
using FS.Commons.Models.DTOs;
using FS.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{
    // [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : BaseAPIController
    {
        private readonly ICustomerBizLogic _customerBizLogic;

        public CustomersController(ICustomerBizLogic customerBizLogic)
        {
            _customerBizLogic = customerBizLogic;
        }

        [Authorize]
        [HttpPost]
        [Route("create-update-customer")]
        public async Task<IActionResult> CreateUpdateCustomer(CustomerRequestDto dto)
        {
            try
            {
                var isInvoked = await IsTokenInvoked();
                if (isInvoked) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (!ModelState.IsValid) return ModelInvalid();
                
                if (!Helpers.IsValidEmail(dto.Email.Trim()))
                {
                    ModelState.AddModelError("Email", Constants.EmailAddressFormatError);
                    return ModelInvalid();
                }

                var responnse = await _customerBizLogic.CreateUpdateCustomer(dto, UserId);
                if (!responnse.IsSuccess) return SaveError(responnse.Message);
                return SaveSuccess(responnse);
            }
            catch (Exception e)
            {
                ConsoleLog.WriteExceptionToConsoleLog(e);
                return Error(Constants.SomeThingWentWrong);
            }   
        }

        [Authorize]
        [HttpPost]
        [Route("get-list-customers")]
        public async Task<IActionResult> GetAllCustomers([FromBody]CustomerGetListDTO dto)
        {
            try
            {
                var isInvoked = await IsTokenInvoked();
                if (isInvoked) return GetUnAuthorized(Constants.GetUnAuthorized);
                
                if (!ModelState.IsValid) return ModelInvalid();
                var data = await _customerBizLogic.GetAllCustomers(dto, UserId);
                var response = new PagingDataModel<CustomerViewDTO>(data, dto);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                ConsoleLog.WriteExceptionToConsoleLog(ex);
                return Error(Constants.SomeThingWentWrong);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("get-customer/{customerId}")]
        public async Task<IActionResult> GetCustomer([FromRoute]long customerId)

        {
            try
            {
                var isInvoked = await IsTokenInvoked();
                if (isInvoked) return GetUnAuthorized(Constants.GetUnAuthorized);
                
                var data = await _customerBizLogic.GetCustomer(customerId, UserId);
                if (data == null) return GetNotFound(Constants.GetNotFound);
                return GetSuccess(data);
            }
            catch (Exception ex)
            {
                ConsoleLog.WriteExceptionToConsoleLog(ex);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        
        [Authorize]
        [HttpPost]
        [Route("delete-customer/{customerId}")]
        public async Task<IActionResult> DeleteCustomer([FromRoute]long customerId)

        {
            try
            {
                var isInvoked = await IsTokenInvoked();
                if (isInvoked) return GetUnAuthorized(Constants.GetUnAuthorized);
                
                var response = await _customerBizLogic.DeleteCustomer(customerId, UserId);
                if (!response.IsSuccess) return SaveError(response.Message);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                ConsoleLog.WriteExceptionToConsoleLog(ex);
                return Error(Constants.SomeThingWentWrong);
            }
        }
    }
}
