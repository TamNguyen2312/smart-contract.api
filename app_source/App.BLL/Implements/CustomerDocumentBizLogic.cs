using App.BLL.Interfaces;
using App.DAL.Interfaces;
using App.Entity.DTOs.CustomerDocument;
using FS.Commons.Models;
using FS.DAL.Interfaces;

namespace App.BLL.Implements;

public class CustomerDocumentBizLogic : ICustomerDocumentBizLogic
{
    private readonly ICustomerDocumentRepository _customerDocumentRepository;
    private readonly IIdentityRepository _identityRepository;

    public CustomerDocumentBizLogic(ICustomerDocumentRepository customerDocumentRepository, IIdentityRepository identityRepository)
    {
        _customerDocumentRepository = customerDocumentRepository;
        _identityRepository = identityRepository;
    }
    public async Task<BaseResponse> CreateUpdateCustomerDocument(CustomerDocumentRequestDTO dto, long userId)
    {
        var entity = dto.GetEntity();
        var user = await _identityRepository.GetByIdAsync(userId);
        var response = await _customerDocumentRepository.CreateUpdateCustomerDocument(entity, user);
        return response;
    }
}