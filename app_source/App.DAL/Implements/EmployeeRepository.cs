using App.DAL.Interfaces;
using App.Entity.DTOs.Employee;
using App.Entity.Entities;
using App.Entity.Mappers;
using AutoMapper;
using FS.BaseModels.IdentityModels;
using FS.Commons;
using FS.Commons.Extensions;
using FS.Commons.Models;
using FS.Commons.Models.DTOs;
using FS.DAL.Interfaces;
using FS.DAL.Queries;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.Implements;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly IFSUnitOfWork<AppDbContext> _unitOfWork;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;


    public EmployeeRepository(IFSUnitOfWork<AppDbContext> unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<BaseResponse> CreateUpdateEmployee(Employee emp, ApplicationUser user)
    {
        try
        {
            var empRepoBase = _unitOfWork.GetRepository<Employee>();
            await _unitOfWork.BeginTransactionAsync();
            var any = await empRepoBase.AnyAsync(new QueryBuilder<Employee>()
                .WithPredicate(x => x.Id == emp.Id && x.DepartmentId == emp.DepartmentId)
                .Build());
            if (any)
            {
                var existedEmp = await empRepoBase.GetSingleAsync(new QueryBuilder<Employee>()
                    .WithPredicate(x => x.Id == emp.Id
                                        && x.DepartmentId == emp.DepartmentId
                                        && x.IsDelete == false)
                    .Build());
                if (existedEmp == null) return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy nhân viên." };
                if (!existedEmp.CreatedBy.Equals(user.UserName))
                    return new BaseResponse { IsSuccess = false, Message = Constants.UserNotSame };
                existedEmp.DepartmentId = emp.DepartmentId;
                existedEmp.ModifiedDate = DateTime.Now;
                existedEmp.ModifiedBy = user.UserName;

                await empRepoBase.UpdateAsync(existedEmp);
            }
            else
            {
                var empCreate = new Employee
                {
                    Id = emp.Id,
                    DepartmentId = emp.DepartmentId,
                    CreatedDate = DateTime.Now,
                    CreatedBy = user.UserName
                };
                await empRepoBase.CreateAsync(empCreate);
            }

            var saver = await _unitOfWork.SaveAsync();
            await _unitOfWork.CommitTransactionAsync();
            if (!saver) return new BaseResponse { IsSuccess = false, Message = Constants.SaveDataFailed };
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
            .WithPredicate(x => x.Id.Equals(userId.ToString()) && x.IsDelete == false)
            .Build());
        return emp;
    }

    public Task<Employee> GetEmployeeForAdmin(long empId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// get all user, cần lưu ý điều chỉnh lại nếu muốn cải thiện hiệu năng
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<List<UserEmpDTO>> GetAllEmployee(AccountGetListDTO dto)
    {
        var baseEmpRepo = _unitOfWork.GetRepository<Employee>();
        var empQuery = baseEmpRepo.Get(new QueryBuilder<Employee>()
            .WithPredicate(x => x.IsDelete == false)
            .Build());

        if (dto.DepartmentId.HasValue)
        {
            empQuery = empQuery.Where(x => x.DepartmentId == dto.DepartmentId.Value);
        }
        
        empQuery = empQuery.ApplyOrderDate(dto.OrderDate);
        
        var employeeIds = await empQuery.Select(emp => emp.Id).ToListAsync();
        
        var userQuery = _userManager.Users.Where(user => employeeIds.Contains(user.Id.ToString()));

        if (!string.IsNullOrEmpty(dto.Keyword))
        {
            userQuery = userQuery.Where(x => x.FirstName.Contains(dto.Keyword)
                                             || x.LastName.Contains(dto.Keyword)
                                             || x.Email.Contains(dto.Keyword)
                                             || x.UserName.Contains(dto.Keyword));
        }
        
        dto.TotalRecord = await userQuery.CountAsync();
        
        var pagedUsers = await userQuery.ToPagedList(dto.PageIndex, dto.PageSize).ToListAsync();
        
        var userIds = pagedUsers.Select(u => u.Id).ToList();
        
        var employees = await empQuery
            .Where(emp => userIds.Contains(Convert.ToInt64(emp.Id)))
            .ToListAsync();
        
        var result = pagedUsers.Join(employees, 
                user => user.Id.ToString(), 
                emp => emp.Id, 
                (user, emp) => new UserEmpDTO
                {
                    User = user,
                    Employee = emp
                })
            .ToList();

        return result;
    }
}