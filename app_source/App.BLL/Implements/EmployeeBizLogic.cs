using App.BLL.Interfaces;
using App.DAL.Interfaces;
using App.Entity.DTOs.Employee;
using FS.Commons.Models;

namespace App.BLL.Implements;

public class EmployeeBizLogic : IEmployeeBizLogic
{
    private readonly IEmployeeRepository _repository;

    public EmployeeBizLogic(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    public async Task<BaseResponse> CreateUpdateEmployee(EmployeeRequestDTO dto, long userId)
    {
        var entity = dto.GetEntity();
        var response = await _repository.CreateUpdateEmployee(entity, userId);
        return response;
    }
}