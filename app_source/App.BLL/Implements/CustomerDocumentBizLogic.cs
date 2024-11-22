using App.BLL.Interfaces;
using App.DAL.Interfaces;
using App.Entity.DTOs.CustomerDocument;
using App.Entity.Entities;
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

    public async Task<CustomerDocumentViewDTO> GetCustomerDocument(long id)
    {
        var data = await _customerDocumentRepository.GetCustomerDocument(id);
        if (data == null) return null;
        var response = GetCustomerDocumentView(data);
        return response;
    }

    public async Task<List<CustomerDocumentViewDTO>> GetAllCustomerDocuments(CustomerDocumentGetListDTO dto, string userName)
    {
        var data = await _customerDocumentRepository.GetAllCustomerDocuments(dto, userName);
        var response = GetCustomerDocumentViews(data);
        return response;
    }

    /// <summary>
    /// This is used to list of customer document that manager can access
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="managerId"></param>
    /// <returns></returns>
    public async Task<List<CustomerDocumentViewDTO>> GetCustomerDocumentsByManagerAsync(CustomerDocumentGetListDTO dto,
        string managerId)
    {
        var data = await _customerDocumentRepository.GetCustomerDocumentsByManagerAsync(dto, managerId);
        var response = GetCustomerDocumentViews(data);
        return response;
    }

    #region PRIVATE

    private CustomerDocumentViewDTO GetCustomerDocumentView(CustomerDocument customerDocument)
    {
        return new CustomerDocumentViewDTO(customerDocument);
    }

    private List<CustomerDocumentViewDTO> GetCustomerDocumentViews(List<CustomerDocument> customerDocuments)
    {
        return customerDocuments.Select(x => new CustomerDocumentViewDTO(x)).ToList();
    }

    #endregion
}