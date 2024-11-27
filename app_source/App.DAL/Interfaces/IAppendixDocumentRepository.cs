using App.Entity.Entities;
using FS.BaseModels.IdentityModels;
using FS.Commons.Models;

namespace App.DAL.Interfaces;

public interface IAppendixDocumentRepository
{
    Task<BaseResponse> CreateUpdateAppendixDocument(AppendixDocument appendixDocument, ApplicationUser user);
}