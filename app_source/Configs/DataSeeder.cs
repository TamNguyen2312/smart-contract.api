using System;
using System.Reflection.Metadata;
using FS.BaseModels.IdentityModels;
using FS.BLL.Services.Interfaces;
using FS.Commons;
using FS.Commons.Models;
using Microsoft.AspNetCore.Identity;

namespace App.API.Configs;

public static class DataSeeder
{
    public static async Task SeedData(this IServiceProvider serviceProvider)
    {
        var identityBizLogic = serviceProvider.GetRequiredService<IIdentityBizLogic>();

        var adminEmail = "minhtam14231204@gmail.com";
        var adminUserName = "minhtam14231204@gmail.com";
        var adminPhoneNumber = "0942775673";
        var adminPassword = "ThisIsAdmin123456@";
        var adminFirstName = "System";
        var adminLastName = "Admin";

        // Seed Roles
        var addRoleAdmin = await identityBizLogic.CreateUpdateRoleAsync(UserType.Admin.ToString(), true);
        var addRoleManger = await identityBizLogic.CreateUpdateRoleAsync(UserType.Manager.ToString(), false);
        var addRoleEmployee = await identityBizLogic.CreateUpdateRoleAsync(UserType.Employee.ToString(), false);
        var addRoleCustomer = await identityBizLogic.CreateUpdateRoleAsync(UserType.Customer.ToString(), false);

        // Seed Admin User
        if (await identityBizLogic.GetByEmailAsync(adminEmail) == null)
        {
            var adminUser = new ApplicationUser
            {
                Email = adminEmail,
                UserName = adminUserName,
                PhoneNumber = adminPhoneNumber,
                EmailConfirmed = true,
                Avatar = Constants.DefaultAvatar,
                FirstName = adminFirstName,
                LastName = adminLastName
            };

            var result = await identityBizLogic.AddUserAsync(adminUser, adminPassword);
            if (result > 0)
            {
                await identityBizLogic.AddRoleByNameAsync(adminUser.Id.ToString(), UserType.Admin.ToString());
            }
        }
    }
}
