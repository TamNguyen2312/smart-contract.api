using App.BLL.Implements;
using App.BLL.Interfaces;
using App.DAL.Implements;
using App.DAL.Interfaces;
using App.Entity.Mappers;
using FS.BLL.Services.Implementations;
using FS.BLL.Services.Interfaces;
using FS.DAL.Implements;
using FS.DAL.Interfaces;
using Microsoft.CodeAnalysis.Operations;

namespace App.API.Configs;

public class DependencyConfig
{
    public static void Register(IServiceCollection services)
    {
        //FS.BLL
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IIdentityBizLogic, IdentityBizLogic>();

        //FS.DAL
        services.AddTransient<IIdentityRepository, IdentityRepository>();
        services.AddTransient(typeof(IGenericRepository<,>), typeof(GenericeRepository<,>));
        services.AddScoped(typeof(IFSUnitOfWork<>), typeof(FSUnitOfWork<>));
        
        //App.BLL
        services.AddTransient<IEmployeeRepository, EmployeeRepository>();



        //App.DAL
        services.AddTransient<IEmployeeBizLogic, EmployeeBizLogic>();
        
        
        //AutoMapper
        services.AddAutoMapper(typeof(MapperProfile));
    }
}