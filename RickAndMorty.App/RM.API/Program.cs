using Microsoft.OpenApi.Models;

using RM.BLL.Interfaces;
using RM.BLL.Services;
using RM.Common;
using RM.DAL.CharactersOperations;
using RM.DAL.ConnectionManagement;

using Serilog;
using Serilog.Events;

namespace RM.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();
            builder.Host.UseSerilog(Log.Logger);

            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog();
            });

            builder.Services.Configure<SettingsModel>(builder.Configuration.GetSection("MySettings"));

            builder.Services.AddSingleton<IDbConnectionManager, DbConnectionManager>();
            builder.Services.AddSingleton<IThirdPartyWebApiClient, ThirdPartyWebApiClient>();
            builder.Services.AddSingleton<ICharDataService, CharDataService>();
            builder.Services.AddSingleton<ICharService, CharService>();

            builder.Services.AddAutoMapper(typeof(Program));
            builder.Services.AddControllers();
            builder.Services.AddResponseCaching(); 
            builder.Services.AddCors(options =>
                {
                    options.AddDefaultPolicy(
                        policyBuilder =>
                            {
                                policyBuilder.AllowAnyOrigin()
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
                            });
                });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RickAndMortyAPI", Version = "v1" });
                });

            var app = builder.Build();

            var charDataService = app.Services.GetService<ICharDataService>();
            charDataService?.CreateTableIfNotExistAsync().Wait(); 

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSerilogRequestLogging(options =>
                {
                    options.GetLevel = (ctx, elapsed, ex) =>
                        {
                            if (ex != null || ctx.Response.StatusCode > 499)
                                return LogEventLevel.Error;

                            return elapsed > 3 ? LogEventLevel.Warning : LogEventLevel.Information;
                        };
                });

            app.UseStaticFiles();

            app.UseHttpsRedirection(); 
            
            app.UseCors();

            app.UseResponseCaching();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}