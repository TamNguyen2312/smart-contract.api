using App.BLL.Interfaces;
using App.Entity.DTOs.Customer;
using FS.BaseAPI;
using FS.Commons;
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
        public async Task<IActionResult> CreateUpdateCustomer(CustomerRequestDTO dto)
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
    }
}
