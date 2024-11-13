using App.BLL.Interfaces;
using App.Entity.DTOs.Profile;
using FS.BaseModels;
using FS.BaseModels.IdentityModels;
using FS.Commons;
using FS.Commons.Extensions;
using FS.Commons.Models;
using FS.Commons.Models.DTOs;
using FS.DAL.Interfaces;

namespace App.BLL.Implements;

public class ProfileBizLogic : IProfileBizLogic
{
    private readonly IIdentityRepository _identityRepository;

    public ProfileBizLogic(IIdentityRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }


    #region PERSONAL

    public async Task<UserViewDTO> GetPersonalProfile(long userId)
    {
        var user = await _identityRepository.GetByIdAsync(userId);
        if (user == null) return null;
        var userRoles = await _identityRepository.GetRolesAsync(userId);
        var view = new UserViewDTO(user, userRoles.ToList());
        return view;
    }

    public async Task<BaseResponse> EditPersonalProfile(PersonalProfileDto dto, long userId)
    {
        var user = await _identityRepository.GetByIdAsync(userId);
        if (user == null) return new BaseResponse { IsSuccess = false, Message = Constants.EXPIRED_SESSION };
        dto.UpdateNonDefaultProperties(user);
        var tryUpdate = await _identityRepository.UpdateAsync(user);
        if (!tryUpdate) return new BaseResponse { IsSuccess = false, Message = Constants.SaveDataFailed };
        return new BaseResponse { IsSuccess = true, Message = Constants.SaveDataSuccess };
    }

    #endregion

    #region ADMIN

    public async Task<BaseResponse> EditProfile(ProfileUpdateDto dto)
    {
        var user = await _identityRepository.GetByIdAsync(dto.Id);
        if (user == null) return new BaseResponse { IsSuccess = false, Message = Constants.EXPIRED_SESSION };
        dto.UpdateNonDefaultProperties(user);
        var tryUpdate = await _identityRepository.UpdateAsync(user);
        if (!tryUpdate) return new BaseResponse { IsSuccess = false, Message = Constants.SaveDataFailed };
        return new BaseResponse { IsSuccess = true, Message = Constants.SaveDataSuccess };
    }

    #endregion
    
}