using chirpApi.Services.Model;
using chirpApi.Services.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;
using chirpApi.Services.Services;

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

            // Add services to the container.
            builder.Services.AddDbContext<CinguettioContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

            builder.Services.AddControllers();

            builder.Services.AddScoped<IChirpsService, LaChirpServices>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            //app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }

    
}
