using App.DAL.Interfaces;
using App.Entity.DTOs.Employee;
using App.Entity.Entities;
using App.Entity.Mappers;
using AutoMapper;
using FS.Commons;
using FS.Commons.Extensions;
using FS.Commons.Models;
using FS.DAL.Interfaces;
using FS.DAL.Queries;

namespace App.DAL.Implements;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly IFSUnitOfWork<AppDbContext> _unitOfWork;
    private readonly IMapper _mapper;

    public EmployeeRepository(IFSUnitOfWork<AppDbContext> unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<BaseResponse> CreateUpdateEmployee(Employee emp, long userId)
    {
        try
        {
            var empRepoBase = _unitOfWork.GetRepository<Employee>();
            await _unitOfWork.BeginTransactionAsync();
            var any = await empRepoBase.AnyAsync(new QueryBuilder<Employee>()
                .WithPredicate(x => x.Id == emp.Id)
                .Build());
            if (any)
            {
                var existedEmp = await empRepoBase.GetSingleAsync(new QueryBuilder<Employee>()
                    .WithPredicate(x => x.Id == emp.Id)
                    .Build());
                if (existedEmp == null) return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy nhân viên." };
                if (!string.IsNullOrEmpty(emp.DepartmentName))
                    existedEmp.DepartmentName = emp.DepartmentName;
                existedEmp.ModifiedDate = DateTime.Now;
                existedEmp.ModifiedBy = userId.ToString();
                await empRepoBase.UpdateAsync(existedEmp);
            }
            else
            {
                var empCreate = new Employee
                {
                    DepartmentName = emp.DepartmentName,
                    CreatedDate = DateTime.Now,
                    CreatedBy = userId.ToString()
                };
                await empRepoBase.CreateAsync(empCreate);
            }

            var saver = await _unitOfWork.SaveAsync();
            await _unitOfWork.CommitTransactionAsync();
            if (!saver) return new BaseResponse { IsSuccess = false, Message = "Lưu nhân viên không thành công." };
            return new BaseResponse { IsSuccess = true, Message = Constants.SaveDataSuccess };
        }
        catch (Exception)
        {
            await _unitOfWork.RollBackAsync();
            throw;
        }
    }

    public async Task<Employee> GetEmployee(long userId)
    {
        var baseEmpRepo = _unitOfWork.GetRepository<Employee>();
        var emp = await baseEmpRepo.GetSingleAsync(new QueryBuilder<Employee>()
            .WithPredicate(x => x.Id == userId && x.IsDelete == false)
            .Build());
        if (emp == null) return null;
        return emp;
    }

    public Task<Employee> GetEmployeeForAdmin(long empId)
    {
        throw new NotImplementedException();
    }
}