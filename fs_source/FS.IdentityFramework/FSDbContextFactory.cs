// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Design;
// using Microsoft.Extensions.Configuration;

// namespace FS.IdentityFramework;

// internal class FSDbContextFactory : IDesignTimeDbContextFactory<FSDbContext>
// {
//     public FSDbContext CreateDbContext(string[] args)
//     {
//         var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"); 
//         var basePath = Path.Combine(AppContext.BaseDirectory, "../smart-contract.api");
//         IConfigurationRoot configuration = new ConfigurationBuilder()
//             .SetBasePath(basePath)
//             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) 
//             .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true) 
//             .Build();

//         var builder = new DbContextOptionsBuilder<FSDbContext>();
//         var connectionString = configuration.GetConnectionString("SmartContract");

//         builder.UseSqlServer(connectionString);

//         return new FSDbContext(builder.Options);
//     }
// }