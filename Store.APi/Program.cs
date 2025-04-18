
using Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Data;
using Services;
using Services.Abstractions;
using System.Threading.Tasks;
using AssemblyMappint = Services.AssemblyReference;
namespace Store.APi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<StoreDbContext>(op =>
            {
                op.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddScoped<IDbInitializer, DbInitializer>(); // Allow DI 
            builder.Services.AddScoped<IUnitOfWord,UintOfWord>();
            builder.Services.AddAutoMapper(typeof(AssemblyMappint).Assembly);
            builder.Services.AddScoped<IServiceManager, ServiceManager>();

            var app = builder.Build();

            #region Seeding

            using var scope = app.Services.CreateScope();
            var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>(); // Ask CLR Create Object From DbInitializer 
            await dbInitializer.InitializeAsync();

            #endregion



            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseStaticFiles(); 

            app.MapControllers();

            app.Run();
        }
    }
}