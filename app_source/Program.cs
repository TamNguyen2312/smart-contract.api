using System.Text;
using App.API.Configs;
using FS.BaseModels.IdentityModels;
using FS.IdentityFramework;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using App.API.Configs;
using Microsoft.EntityFrameworkCore.Migrations;
using FS.Commons;

namespace App.API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			//<=====Set up policy=====>
			builder.Services.AddCors(opts =>
			{
				opts.AddPolicy("corspolicy", build =>
				{
					build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
				});
			});

			//<=====Add Database=====>
			var connectionString = builder.Configuration.GetConnectionString("SmartContract");
			builder.Services.AddEntityFrameworkSqlServer().AddDbContext<FSDbContext>(
				opts => opts.UseSqlServer(connectionString, options =>
				{
					options.MigrationsAssembly("App.API");
					options.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "fs");
				}));


			//<=====Add Identity=====>
			builder.Services.AddAuthorization();
			builder.Services.AddIdentity<ApplicationUser, Role>().AddEntityFrameworkStores<FSDbContext>().AddDefaultTokenProviders();
			builder.Services.Configure<IdentityOptions>(options =>
			{
				// Default Password settings.
				options.Password.RequireDigit = false;
				options.Password.RequireLowercase = false;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = false;
				options.Password.RequiredLength = 6;
				options.Password.RequiredUniqueChars = 0;

			});

			//<=====Add config for Required Email=====>
			builder.Services.Configure<IdentityOptions>(opts =>
			{
				opts.SignIn.RequireConfirmedEmail = true;
				opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
				opts.Lockout.MaxFailedAccessAttempts = 5;
				opts.Lockout.AllowedForNewUsers = true;
			});

			//<=====Add config for verify token=====>
			builder.Services.Configure<DataProtectionTokenProviderOptions>(opts => opts.TokenLifespan = TimeSpan.FromHours(1));

			//<=====API Behavior=====>
			builder.Services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

			//<=====Forward Headers=====>
			builder.Services.Configure<ForwardedHeadersOptions>(options =>
			{
				options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
			});

			//<=====Add HttpClient=====>
			builder.Services.AddHttpClient();

			//<=====Add JWT Authentication=====>
			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.SaveToken = true;
				options.RequireHttpsMetadata = false;
				options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidAudience = builder.Configuration["JWT:ValidAudience"],
					ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
					ClockSkew = TimeSpan.Zero,
					ValidateLifetime = true,
				};

				options.Events = new JwtBearerEvents
				{
					OnMessageReceived = context =>
					{
						var accessToken = context.Request.Query["access_token"];
						// If the request is for our hub...
						var path = context.HttpContext.Request.Path;
						if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/eventHub")))
						{
							// Read the token out of the query string
							context.Token = accessToken;
						}
						return Task.CompletedTask;
					}
				};
			});

			//<=====Add Dependency Injection=====>
			DependencyConfig.Register(builder.Services);

			var app = builder.Build();

			//<=====Seed Base data system=====>
			using (var scope = app.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				try
				{
					services.SeedData().Wait();
				}
				catch (Exception ex)
				{
					ConsoleLog.WriteExceptionToConsoleLog(ex);
				}
			}

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseCors("corspolicy");
			app.UseHttpsRedirection();
			app.UseAuthentication();
			app.UseAuthorization();
			app.MapControllers();
			app.Run();
		}
	}
}
