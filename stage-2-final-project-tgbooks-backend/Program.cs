
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using stage_2_final_project_tgbooks_backend.DaEditBookByIdEditBookByIdAsyncta.Implementations;
using stage_2_final_project_tgbooks_backend.Data;
using stage_2_final_project_tgbooks_backend.Data.Implementations;
using stage_2_final_project_tgbooks_backend.Data.Interfaces;
using stage_2_final_project_tgbooks_backend.Services.Implementations;
using stage_2_final_project_tgbooks_backend.Services.Interfaces;
using System.Reflection;
using WebApplication2.Services;
using WebApplication2.Services.Interfaces;

namespace stage_2_final_project_tgbooks_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // Add services
            builder.Services.AddControllers();
            builder.Services.AddScoped<IDatabaseManager, DatabaseManager>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IBookService, BookService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IAuthorService, AuthorService>();
            builder.Services.AddSingleton<IStorageService, BlobService>();
            builder.Services.AddSingleton<IEmailSender, EmailSender>();

            // register all validators in the assembly
            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Add Swagger (Swashbuckle)
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add DbContext
            builder.Services.AddDbContext<DatabaseContext>(options =>
                options
                    .UseSqlServer(
                        builder.Configuration.GetConnectionString("DefaultConnection")
                    )
            );

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                // Enable Swagger UI in development
                app.UseSwagger();             // generates swagger JSON
                app.UseSwaggerUI();           // serves the Swagger UI
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
