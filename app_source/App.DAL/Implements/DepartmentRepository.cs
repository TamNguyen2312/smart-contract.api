using App.DAL.Interfaces;
using App.Entity.DTOs.Department;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons;
using FS.Commons.Extensions;
using FS.Commons.Models;
using FS.DAL.Interfaces;
using FS.DAL.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace App.DAL.Implements;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly IFSUnitOfWork<AppDbContext> _unitOfWork;

    public DepartmentRepository(IFSUnitOfWork<AppDbContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse> CreateUpdateDepartment(Department department, ApplicationUser user)
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
            if (!existed.CreatedBy.Equals(user.UserName))
                return new BaseResponse { IsSuccess = false, Message = Constants.UserNotSame };
            
            department.UpdateNonDefaultProperties(existed);
            existed.ModifiedBy = user.UserName;
            existed.ModifiedDate = DateTime.Now;

            await baseRepo.UpdateAsync(existed);
        }
        else
        {
            var newDepartment = new Department
            {
                Name = department.Name,
                Description = department.Description,
                EmployeeQuantity = department.EmployeeQuantity,
                MornitorQuantity = 0,
                CreatedDate = DateTime.Now,
                CreatedBy = user.UserName,
                IsDelete = false,
                ModifiedBy = default,
                ModifiedDate = default
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

    public async Task<Department> GetDepartment(long id)
    {
        var baseRepo = _unitOfWork.GetRepository<Department>();
        var response = await baseRepo.GetSingleAsync(new QueryBuilder<Department>()
            .WithPredicate(x => x.IsDelete == false && x.Id == id)
            .Build());
        return response;
    }

    public async Task<List<Department>> GetAllDepartments(DepartmentGetListDTO dto)
    {
        var baseRepo = _unitOfWork.GetRepository<Department>();
        var query = baseRepo.Get(new QueryBuilder<Department>()
            .WithPredicate(x => x.IsDelete == false)
            .Build());

        if (!string.IsNullOrEmpty(dto.Keyword))
        {
            query = query.Where(x => x.Name.Contains(dto.Keyword) ||
                                     x.CreatedBy.Contains(dto.Keyword));
        }

        query = query.ApplyOrderDate(dto.OrderDate);
        dto.TotalRecord = await query.CountAsync();
        return await query.ToPagedList(dto.PageIndex, dto.PageSize).ToListAsync();
    }
}