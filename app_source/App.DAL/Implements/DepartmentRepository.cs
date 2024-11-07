using App.DAL.Interfaces;
using App.Entity.DTOs.Department;
using App.Entity.Entities;
using FS.Commons;
using FS.Commons.Models;
using FS.DAL.Interfaces;
using FS.DAL.Queries;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace App.DAL.Implements;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly IFSUnitOfWork<AppDbContext> _unitOfWork;

    public DepartmentRepository(IFSUnitOfWork<AppDbContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse> CreateUpdateDepartment(Department department, long userId)
    {
        var baseRepo = _unitOfWork.GetRepository<Department>();
        var any = await baseRepo.AnyAsync(new QueryBuilder<Department>()
            .WithPredicate(x => x.Id == department.Id)
            .Build());
        if (any)
        {
            var existed = await baseRepo.GetSingleAsync(new QueryBuilder<Department>()
                .WithPredicate(x => x.Id == department.Id && x.IsDelete == false)
                .Build());
            if (existed == null) return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy phòng ban." };
            if (!existed.CreatedBy.Equals(userId))
                return new BaseResponse { IsSuccess = false, Message = Constants.UserNotSame };
            existed.Name = department.Name;
            existed.Description = department.Description;
            existed.ModifiedBy = userId.ToString();
            existed.ModifiedDate = DateTime.Now;

            await baseRepo.UpdateAsync(existed);
        }
        else
        {
            var newDepartment = new Department
            {
                Name = department.Name,
                Description = department.Description,
                CreatedDate = DateTime.Now,
                CreatedBy = userId.ToString(),
                IsDelete = false
            };
            await baseRepo.CreateAsync(newDepartment);
        }

        var saver = await _unitOfWork.SaveAsync();
        if (!saver) return new BaseResponse { IsSuccess = false, Message = Constants.SaveDataFailed};
        return new BaseResponse { IsSuccess = true, Message = Constants.SaveDataSuccess};
    }

    public async Task<List<Department>> GetDropDownDepartment()
    {
        var baseRepo = _unitOfWork.GetRepository<Department>();
        var response = await baseRepo.GetAllAsync(new QueryBuilder<Department>()
            .WithPredicate(x => x.IsDelete == false)
            .Build());
        return response.ToList();
    }
}