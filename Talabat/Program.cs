using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.APIs.Middlwares;
using Talabat.Core;
using Talabat.Core.Entities.Identity;
using Talabat.Core.IRepositories;
using Talabat.Core.IServices;
using Talabat.Repository;
using Talabat.Repository.Data;
using Talabat.Repository.Identity;
using Talabat.Service;

namespace Talabat
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            #region Configure Services
            builder.Services.AddControllers();

            builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

            builder.Services.AddScoped(typeof(IProductService), typeof(ProductService));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<StoreContext>(b =>
            {
                b.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddDbContext<IdentityContext>(b =>
                b.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"))
            );

            builder.Services.AddScoped(typeof(IOrderService), typeof(OrderService));

            builder.Services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));

            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            builder.Services.AddScoped(typeof(IAuthService), typeof(AuthService));

            builder.Services.AddScoped(typeof(IPaymentService), typeof(PaymentService));

            builder.Services.AddSingleton(typeof(IResponseCacheService), typeof(ResponseCacheService));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", options =>
                {
                    options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });

            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {

            }).AddEntityFrameworkStores<IdentityContext>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["JWT:Audience"],
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration["JWT:Issuer"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"])),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromDays(double.Parse(builder.Configuration["JWT:DurationInDays"]))
                    };
                });
            builder.Services.AddAutoMapper(typeof(MappingProfiles));

            builder.Services.AddSingleton<IConnectionMultiplexer>((ServiceProvider) =>
            {
                var connection = builder.Configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(connection);
            });

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
                                                         .SelectMany(P => P.Value.Errors)
                                                         .Select(E => E.ErrorMessage)
                                                         .ToList();
                    var response = new APIsValidationErrorResponse()
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(response);
                };
            });
            #endregion


            var app = builder.Build();

            using var scope = app.Services.CreateScope();
            
            var service = scope.ServiceProvider;

            var dbContext = service.GetRequiredService<StoreContext>();

            var IdentityDbContext = service.GetRequiredService<IdentityContext>();

            var loggerFactory = service.GetRequiredService<ILoggerFactory>();

            var userManager = service.GetRequiredService<UserManager<AppUser>>();


            try
            {
                await dbContext.Database.MigrateAsync();
                await IdentityDbContext.Database.MigrateAsync();
                await StoreContextSeed.SeedAsync(dbContext);
                await IdentityContextSeed.SeedUserAsync(userManager);
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An error has occured during apply migration");
            }
            // Configure the HTTP request pipeline.
            #region Configure Kestral pipelines
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseMiddleware<APIsExceptionMiddleware>();
            }

            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCors("MyPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            #endregion

            app.Run();
        }
    }
}
