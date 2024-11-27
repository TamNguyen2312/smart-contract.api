using App.Entity.DTOs.AppendixDocument;
using FS.Commons.Models;

namespace App.BLL.Interfaces;

public interface IAppendixDocumentBizLogic
{
    Task<BaseResponse> CreateUpdateAppendixDocument(AppendixDocumentRequestDTO dto, long userId);
}