using App.Entity.DTOs.Profile;
using FS.Commons.Models;
using FS.Commons.Models.DTOs;

namespace App.BLL.Interfaces;

public interface IProfileBizLogic
{
    Task<UserViewDTO> GetPersonalProfile(long userId);
    Task<BaseResponse> EditPersonalProfile(PersonalProfileDTO dto, long userId);
}