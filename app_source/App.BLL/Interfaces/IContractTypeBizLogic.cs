using System;
using App.Entity.DTOs.ContractType;
using FS.Commons.Models;

namespace App.BLL.Interfaces;

public interface IContractTypeBizLogic
{
    Task<BaseResponse> CreateUpdateContractType(ContractTypeRequestDTO dto, long userId);
    Task<ContractTypeViewDTO> GetContractTypeById(long id);
    Task<List<ContractTypeViewDTO>> GetAllContractType(ContractTypeGetListDTO dto);
    Task<List<ContractTypeViewDTO>> GetDropdownList();
}
