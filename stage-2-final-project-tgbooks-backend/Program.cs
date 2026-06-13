
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using stage_2_final_project_tgbooks_backend.DaEditBookByIdEditBookByIdAsyncta.Implementations;
using stage_2_final_project_tgbooks_backend.Data;
using stage_2_final_project_tgbooks_backend.Data.Implementations;
using stage_2_final_project_tgbooks_backend.Data.Interfaces;
using stage_2_final_project_tgbooks_backend.Services.Implementations;
using stage_2_final_project_tgbooks_backend.Services.Interfaces;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;
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
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IBookAnalyticsService, BookAnalyticsService>(); 
            builder.Services.AddSingleton<IStorageService, BlobService>();
            builder.Services.AddSingleton<IEmailSender, EmailSender>();

            // Register the Rate Limiting Services
            builder.Services.AddRateLimiter(options =>
            {
                // Return HTTP 429 Too Many Requests when a client is blocked
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                // Define a custom policy named "StrictIpValidation"
                options.AddPolicy("StrictIpValidation", httpContext =>
                {
                    // Safely extract the client's remote IP address
                    var remoteIpAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown-ip";

                    // Apply a fixed window limit: 20 requests every 50 minute per unique IP
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: remoteIpAddress,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 20,                  // Allow maximum 20 requests
                            Window = TimeSpan.FromMinutes(60), // Inside a 1-hour window
                            QueueLimit = 0                    // Drop extra requests instantly instead of queuing them
                        });
                });
            });


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // register all validators in the assembly
            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Add DbContext
            var connectionString = Environment.GetEnvironmentVariable("Default_Db_Connection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Default_Db_Connection environment variable is not set.");
            }

            builder.Services.AddDbContext<DatabaseContext>(options =>
                options
                    .UseSqlServer(connectionString));

           
            // Add Swagger (Swashbuckle)
            builder.Services.AddEndpointsApiExplorer();
            // 2. Configure Swagger Generation
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "10GO's Books .NET 10 API", Version = "v1" });


                //While AddSecurityRequirement handles the where, AddSecurityDefinition is all about the what.
                // AddSecurityDefinition tells Swagger exactly how your security is structured.
                // It defines the "recipe" for authentication so that the Swagger UI knows what kind of input to ask the user for.
                options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    // Tells Swagger this is a standard HTTP authentication scheme
                    Type = SecuritySchemeType.Http,
                    // Specifies that the 'Authorization' header will start with 'Bearer'
                    Scheme = "bearer",
                    // Informational: lets the user know we expect a JSON Web Token
                    BearerFormat = "JWT",
                    // The text shown in the Swagger UI popup to help the user
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                //AddSecurityRequirement is the method that links your security configuration to your actual API endpoints in the documentation.
                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    // This 'Reference' links this requirement to the definition we made above. 
                    // The ID "bearer" must match the name in AddSecurityDefinition exactly.
                    [new OpenApiSecuritySchemeReference("bearer", document)] =
                    // This empty array [] represents 'scopes'. 
                    // For standard JWT Bearer auth, no specific OAuth scopes are needed here.
                    []
                });
            });

            // 3. Add Authentication & Authorization (Ensure these are configured)
            // JWT AUTHENTICATION
            var jwtSecret = Environment.GetEnvironmentVariable("JwtSecret");
            if (string.IsNullOrWhiteSpace(jwtSecret))
            {
                throw new InvalidOperationException("JWT secret key is not set in environment variables.");
            }

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "10GO's books backend",
                        ValidAudiences = new[] { "10GO-Android-App", "10GO-iOS-App", "10GO-Web-App" },
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))

                    };
                });

            builder.Services.AddAuthorization();

            builder.Services.AddHostedService<UnverifiedUserCleanupService>();

            var app = builder.Build();

            // Apply migrations automatically
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                db.Database.Migrate(); // This will apply pending migrations to Azure DB
            }

            // 4. Configure Middleware Order (CRITICAL)
            //if (app.Environment.IsDevelopment()){
            app.UseSwagger();
                app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            // Enable the Rate Limiter Middleware
            app.UseRateLimiter();

            // Authentication MUST come before Authorization and MapControllers
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
