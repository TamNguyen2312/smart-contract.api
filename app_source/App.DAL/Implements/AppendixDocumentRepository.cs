using App.DAL.Interfaces;
using FS.DAL.Interfaces;

namespace App.DAL.Implements;

public class AppendixDocumentRepository : IAppendixDocumentRepository
{
    private readonly IFSUnitOfWork<AppDbContext> _unitOfWork;

    public AppendixDocumentRepository(IFSUnitOfWork<AppDbContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    
}