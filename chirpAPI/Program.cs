using chirpApi.Services.Model;
using chirpApi.Services.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;
using chirpApi.Services.Services;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi;

namespace chirpAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

            builder.Host.UseSerilog();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v3", new OpenApiInfo
                {
                    Title = "Cinguettio API",
                    Version = "v3",
                    Description = "API for Cinguettio, a social media platform for chirps."
                });
            });

            // Add services to the container.
            builder.Services.AddDbContext<CinguettioContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

            builder.Services.AddControllers();

            builder.Services.AddScoped<IChirpsService, LaChirpServices>();

            var app = builder.Build();

            app.UseSwagger(c =>
            {
                c.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v3/swagger.json", "Cinguettio API V1");
                
            });

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            //app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }

    
}
