using App.Entity.Entities;
using FS.Commons.Models;

namespace App.DAL.Interfaces;

public interface IManagerRepository
{
    Task<BaseResponse> CreateUpdateManager(Manager manager, long userId);
}