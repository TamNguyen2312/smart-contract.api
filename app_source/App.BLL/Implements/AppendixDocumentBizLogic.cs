using App.BLL.Interfaces;
using App.DAL.Interfaces;
using FS.DAL.Interfaces;

namespace App.BLL.Implements;

public class AppendixDocumentBizLogic : IAppendixDocumentBizLogic
{
    private readonly IAppendixDocumentRepository _appendixDocumentRepository;
    private readonly IIdentityRepository _identityRepository;

    public AppendixDocumentBizLogic(IAppendixDocumentRepository appendixDocumentRepository, IIdentityRepository identityRepository)
    {
        _appendixDocumentRepository = appendixDocumentRepository;
        _identityRepository = identityRepository;
    }
}