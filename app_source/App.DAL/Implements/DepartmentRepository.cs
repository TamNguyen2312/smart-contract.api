using App.DAL.Interfaces;
using FS.DAL.Interfaces;

namespace App.DAL.Implements;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly IFSUnitOfWork<AppDbContext> _unitOfWork;

    public DepartmentRepository(IFSUnitOfWork<AppDbContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    
}