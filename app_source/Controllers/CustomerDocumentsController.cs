using App.API.Filter;
using App.BLL.Interfaces;
using App.Entity.DTOs.Customer;
using App.Entity.DTOs.CustomerDocument;
using FS.BaseAPI;
using FS.Commons;
using FS.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{
    // [Route("api/[controller]")]
    [ApiController]
    public class CustomerDocumentsController : BaseAPIController
    {
        private readonly ICustomerDocumentBizLogic _customerDocumentBizLogic;
        private readonly ILogger<CustomerDocumentsController> _logger;
        private readonly ICustomerBizLogic _customerBizLogic;

        public CustomerDocumentsController(ICustomerDocumentBizLogic customerDocumentBizLogic, ILogger<CustomerDocumentsController> logger, ICustomerBizLogic customerBizLogic)
        {
            _customerDocumentBizLogic = customerDocumentBizLogic;
            _logger = logger;
            _customerBizLogic = customerBizLogic;
        }
        
        [FSAuthorize(Policy = "AdminRolePolicy")]
        [HttpPost]
        [Route("create-update-customer-document")]
        public async Task<IActionResult> CreateUpdateCustomerDocument(CustomerDocumentRequestDTO dto)
        {
            try
            {
                var isInvoked = await IsTokenInvoked();
                if (isInvoked) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (!ModelState.IsValid) return ModelInvalid();

                var existedCustomer = await _customerBizLogic.GetCustomer(dto.CustomerId);
                if (existedCustomer == null)
                {
                    return SaveError("Khách hàng không tồn tại.");
                }
                var responnse = await _customerDocumentBizLogic.CreateUpdateCustomerDocument(dto, UserId);
                if (!responnse.IsSuccess) return SaveError(responnse.Message);
                return SaveSuccess(responnse);
            }
            catch (Exception e)
            {
                _logger.LogError("CreateUpdateCustomerDocument {0} {1}", e.Message, e.StackTrace); 
                return Error(Constants.SomeThingWentWrong);
            }   
        }
        
        [FSAuthorize(Policy = "AdminManagerPolicy")]
        [HttpPost]
        [Route("get-customer-document-by-id/{id}")]
        public async Task<IActionResult> GetCustomerDocument([FromRoute]long id)

        {
            try
            {
                var isInvoked = await IsTokenInvoked();
                if (isInvoked) return GetUnAuthorized(Constants.GetUnAuthorized);

                //...author manager
                
                var data = await _customerDocumentBizLogic.GetCustomerDocument(id);
                if (data == null) return GetNotFound(Constants.GetNotFound);
                return GetSuccess(data);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetCustomerDocument {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
    }
}
