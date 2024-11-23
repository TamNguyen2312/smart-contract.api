using System;
using App.Entity.DTOs.Contract;
using FS.Commons.Models;

namespace App.BLL.Interfaces;

public interface IContractBizLogic
{
    Task<BaseResponse> CreateContract(ContractRequestDTO dto, long userId);
}
