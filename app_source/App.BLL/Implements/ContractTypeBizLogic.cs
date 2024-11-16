using System;
using App.BLL.Interfaces;
using App.DAL.Interfaces;
using App.Entity.DTOs.ContractType;
using App.Entity.Entities;
using FS.Commons.Models;
using FS.DAL.Interfaces;

namespace App.BLL.Implements;

public class ContractTypeBizLogic : IContractTypeBizLogic
{
    private readonly IContractTypeRepository _contractTypeRepository;
    private readonly IIdentityRepository _identityRepository;

    public ContractTypeBizLogic(IContractTypeRepository contractTypeRepository, IIdentityRepository identityRepository)
    {
        this._contractTypeRepository = contractTypeRepository;
        this._identityRepository = identityRepository;
    }
    public async Task<BaseResponse> CreateUpdateContractType(ContractTypeRequestDTO dto, long userId)
    {
        var user = await _identityRepository.GetByIdAsync(userId);
        var entity = dto.GetEntity();
        var response = await _contractTypeRepository.CreateUpdateContractType(entity, user);
        return response;
    }

    public async Task<List<ContractTypeViewDTO>> GetAllContractType(ContractTypeGetListDTO dto)
    {
        var data = await _contractTypeRepository.GetAllContractType(dto);
        var response = GetContractTypeViews(data);
        return response;
    }

    public async Task<ContractTypeViewDTO> GetContractTypeById(long id)
    {
        var data = await _contractTypeRepository.GetContractTypeById(id);
        if (data == null) return null;
        var response = new ContractTypeViewDTO(data);
        return response;
    }

    public async Task<List<ContractTypeViewDTO>> GetDropdownList()
    {
        var data = await _contractTypeRepository.GetDropdownList();
        var response = GetContractTypeViews(data);
        return response;
    }

    #region PRIVATE
    /// <summary>
    /// This is used to convert a collection of contract types to collection of contract type views
    /// </summary>
    /// <param name="contractTypes"></param>
    /// <returns></returns>
    private List<ContractTypeViewDTO> GetContractTypeViews(List<ContractType> contractTypes)
    {
        var response = new List<ContractTypeViewDTO>();
        foreach (var type in contractTypes)
        {
            var view = new ContractTypeViewDTO(type);
            response.Add(view);
        }
        return response;
    }

    #endregion
}
