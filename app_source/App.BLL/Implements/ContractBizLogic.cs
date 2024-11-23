using System;
using App.BLL.Interfaces;
using App.DAL.Interfaces;
using App.Entity.DTOs.Contract;
using App.Entity.Entities;
using FS.Commons.Models;
using FS.DAL.Interfaces;

namespace App.BLL.Implements;

public class ContractBizLogic : IContractBizLogic
{
    private readonly IContractRepository _contractRepository;
    private readonly IIdentityRepository _identityRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IContractTypeRepository _contractTypeRepository;

    public ContractBizLogic(IContractRepository contractRepository,
        IIdentityRepository identityRepository,
        ICustomerRepository customerRepository,
        IContractTypeRepository contractTypeRepository)
    {
        _contractRepository = contractRepository;
        _identityRepository = identityRepository;
        _customerRepository = customerRepository;
        _contractTypeRepository = contractTypeRepository;
    }

    public async Task<BaseResponse> CreateContract(ContractRequestDTO dto, long userId)
    {
        var entity = dto.GetEntity();
        var user = await _identityRepository.GetByIdAsync(userId);
        var response = await _contractRepository.CreateContract(entity, user);
        return response;
    }

    public async Task<List<ContractViewDTO>> GetContractByManager(ContractGetListDTO dto, string managerId)
    {
        var data = await _contractRepository.GetContractsByManager(dto, managerId);
        var response = await GetContractViews(data);
        return response;
    }

    #region PRIVATE

    private async Task<ContractViewDTO> GetContractView(Contract contract)
    {
        var customer = await _customerRepository.GetCustomer(contract.CustomerId);
        if (customer == null) return null;

        var contractType = await _contractTypeRepository.GetContractTypeById(contract.ContractTypeId);
        if (contractType == null) return null;

        var view = new ContractViewDTO(contract, customer, contractType);
        return view;
    }

    private async Task<List<ContractViewDTO>> GetContractViews(List<Contract> contracts)
    {
        var views = new List<ContractViewDTO>();
        foreach (var contract in contracts)
        {
            var view = await GetContractView(contract);
            views.Add(view);
        }

        return views;
    }

    #endregion
}
