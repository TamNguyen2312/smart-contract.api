using App.API.Filter;
using App.BLL.Implements;
using App.BLL.Interfaces;
using App.DAL.Implements;
using App.DAL.Interfaces;
using App.Entity.Mappers;
using FS.BLL.Services.Implementations;
using FS.BLL.Services.Interfaces;
using FS.DAL.Implements;
using FS.DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;

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

        //App.DAL
        services.AddTransient<IEmployeeRepository, EmployeeRepository>();
        services.AddTransient<ICustomerRepository, CustomerRepository>();
        services.AddTransient<IDepartmentRepository, DepartmentRepository>();
        services.AddTransient<IManagerRepository, ManagerRepository>();
        services.AddTransient<IFileUploadRepository, FileUploadRepository>();
        services.AddTransient<IContractTypeRepository, ContractTypeRepository>();
        services.AddTransient<IContractRepository, ContractRepository>();
        services.AddTransient<ICustomerDocumentRepository, CustomerDocumentRepository>();
        services.AddTransient<ICustomerDepartmentAssignRepository, CustomerDepartmentAssignRepository>();
        services.AddTransient<IContractDocumentRepository, ContractDocumentRepository>();


        //App.BLL
        services.AddTransient<IEmployeeBizLogic, EmployeeBizLogic>();
        services.AddTransient<ICustomerBizLogic, CustomerBizLogic>();
        services.AddTransient<IDepartmentBizLogic, DepartmentBizLogic>();
        services.AddTransient<IManagerBizLogic, ManagerBizLogic>();
        services.AddTransient<IProfileBizLogic, ProfileBizLogic>();
        services.AddTransient<IFileUploadBizLogic, FileUploadBizLogic>();
        services.AddTransient<IContractTypeBizLogic, ContractTypeBizLogic>();
        services.AddTransient<IContractBizLogic, ContractBizLogic>();
        services.AddTransient<ICustomerDocumentBizLogic, CustomerDocumentBizLogic>();
        services.AddTransient<ICustomerDepartmentAssignBizLogic, CustomerDepartmentAssignBizLogic>();
        services.AddTransient<IContractDocumentBizLogic, ContractDocumentBizLogic>();


        //AutoMapper
        services.AddAutoMapper(typeof(MapperProfile));

        //Authorization
        services.AddSingleton<IAuthorizationPolicyProvider, FSAuthorizationPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, FSRolesHandler>();
        services.AddSingleton<IAuthorizationHandler, FSEmailConfirmHandler>();
        services.AddSingleton<IAuthorizationHandler, FSPermissionHandler>();
    }
}