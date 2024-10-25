using FS.BLL.Services.Implementations;
using FS.BLL.Services.Interfaces;

namespace App.API.Configs;

public class DependencyConfig
{
    public static void Register(IServiceCollection services)
    {
        //FS.BLL
        services.AddTransient<IEmailService, EmailService>();
    }
}