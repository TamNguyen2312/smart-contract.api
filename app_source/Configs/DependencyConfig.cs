using FS.BLL.Services.Implementations;
using FS.BLL.Services.Interfaces;
using FS.DAL.Implements;
using FS.DAL.Interfaces;

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
    }
}