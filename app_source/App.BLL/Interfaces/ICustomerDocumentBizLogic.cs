using App.Entity.DTOs.CustomerDocument;
using FS.Commons.Models;

namespace App.BLL.Interfaces;

public interface ICustomerDocumentBizLogic
{
    Task<BaseResponse> CreateUpdateCustomerDocument(CustomerDocumentRequestDTO dto, long userId);
}