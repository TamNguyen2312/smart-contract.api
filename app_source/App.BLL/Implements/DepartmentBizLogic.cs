using App.BLL.Interfaces;
using App.DAL.Interfaces;
using App.Entity.DTOs.Department;
using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;
using FS.DAL.Interfaces;
using Microsoft.IdentityModel.Abstractions;

namespace App.BLL.Implements;

public class DepartmentBizLogic : IDepartmentBizLogic
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IIdentityRepository _identityRepository;

    public DepartmentBizLogic(IDepartmentRepository departmentRepository, IIdentityRepository identityRepository)
    {
        _departmentRepository = departmentRepository;
        _identityRepository = identityRepository;
    }
    
    
    public async Task<BaseResponse> CreateUpdateDepartment(DepartmentRequestDto dto, long userId)
    {
        var entity = dto.GetEntity();
        var user = await _identityRepository.GetByIdAsync(userId);
        var response = await _departmentRepository.CreateUpdateDepartment(entity, user);
        return response;
    }

    public async Task<List<DepartmentViewDTO>> GetDropDownDepartment()
    {
        var data = await _departmentRepository.GetDropDownDepartment();
        var response = await GetDepartmentViews(data);
        return response;
    }

    public async Task<DepartmentViewDTO> GetDepartment(long id, long userId)
    {
        var data = await _departmentRepository.GetDepartment(id, userId);
        var response = await GetDepartmentView(data);
        return response;
    }


    #region PRIVATE

    /// <summary>
    /// This is used to convert a department to a department view
    /// </summary>
    /// <param name="department"></param>
    /// <returns></returns>
    private async Task<DepartmentViewDTO> GetDepartmentView(Department department)
    {
        var user = await _identityRepository.GetByEmailAsync(department.CreatedBy);
        if (user == null) return null;
        var view = new DepartmentViewDTO(department, user);
        return view;
    }

    private async Task<List<DepartmentViewDTO>> GetDepartmentViews(List<Department> departments)
    {
        var views = new List<DepartmentViewDTO>();
        foreach (var department in departments)
        {
            var view = await GetDepartmentView(department);
            views.Add(view);
        }

        return views;
    }

    #endregion
}