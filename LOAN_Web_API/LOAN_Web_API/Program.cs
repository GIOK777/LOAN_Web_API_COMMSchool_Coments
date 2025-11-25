
using FluentValidation;
using FluentValidation.AspNetCore;
using LOAN_Web_API.Interfaces;
using LOAN_Web_API.Mappers;
using LOAN_Web_API.Middleware;
using LOAN_Web_API.Models;
using LOAN_Web_API.Services;
using LOAN_Web_API.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

namespace LOAN_Web_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // --- SeriLog კონფიგურაცია (გამოყენება BUILDER-ის შექმნამდე) ---
            // ლოგირების კონფიგურაცია appsettings.json-დან
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                .Build();

            // SeriLog Logger-ის შექმნა
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration) // პარამეტრების წაკითხვა appsettings-დან
                .Enrich.FromLogContext() // კონტექსტური ინფორმაციის დამატება
                .WriteTo.Console() // ლოგირება კონსოლში (Dev-ისთვის)
                                   // აქ შეგვიძლია დავამატოთ WriteTo.File ან WriteTo.MSSqlServer
                                   // (იხილეთ AppSettings-ის კონფიგურაცია)
                .CreateLogger();

            // SeriLog-ის გამოყენება ASP.NET Core-ში
            builder.Host.UseSerilog();




            // Add services to the container.

            // არეგისტრირებს Validator კლასებს DI (Dependency Injection) კონტეინერში
            builder.Services.AddFluentValidationAutoValidation();
            // მოიძებნე და დარეგისტრირე ყველა FluentValidator იმ Assembly-ში ანუ პროექტში, სადაც მოთავსებულია მითითებული კლასი.
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();
            //builder.Services.AddValidatorsFromAssemblyContaining<LoanStatusUpdateDtoValidator>();
            //builder.Services.AddValidatorsFromAssemblyContaining<LoanRequestDtoValidator>();


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                // პირველი ოფშენი სვაგერის დოკუმენტაცია აღვწეროთ
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "My web api project", Version = "V1" });
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter your Bearer JWT-Token",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header, // სად იყოს განთავსებული
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });
                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        }, new string[]{}
                    }
                });
            });


            // --- DB Context Configuration ---
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // --- Service Registrations (DIP / Dependency Injection) ---
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ILoanService, LoanService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();

            // --- AutoMapper Registration ---
            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);



            // --- JWT Authentication Configuration ---
            var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing.");
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            


            // --- Authorization Configuration (Role-Based) ---
            builder.Services.AddAuthorization(options =>
            {
                //ეს არის ASP.NET Core-ის Authorization Policy-ის კონფიგურაცია, რომელიც საშუალებას გვაძლევს, დავაყენოთ წესები,
                //თუ ვის აქვს კონკრეტულ რესურსებზე წვდომა, ტრადიციული როლების შემოწმებაზე უფრო მოქნილი გზით.
                options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Administrator"));
                options.AddPolicy("UserPolicy", policy => policy.RequireRole("User", "Administrator"));
            });




            var app = builder.Build();


            // Configure the HTTP request pipeline.

            // --- 1. Custom Exception Handling Middleware ---
            // ეს უნდა იყოს პირველი (თუ UseDeveloperExceptionPage არ მუშაობს), რათა დაიჭიროს ყველა შეცდომა.
            if (!app.Environment.IsDevelopment())
            {
                // Production გარემოში ვიყენებთ ჩვენს უსაფრთხო ჰენდლერს
                app.UseMiddleware<ExceptionHandlingMiddleware>();
            }

            if (app.Environment.IsDevelopment())
            {
                //ეს ნიშნავს, რომ თუ აპლიკაცია მუშაობის დროს წააწყდება დაუმუშავებელ შეცდომას (Exception),
                //ეს Middleware დაიჭერს მას და ბრაუზერში გამოიტანს დეტალურ, მომხმარებლისთვის მოსახერხებელ გვერდს შეცდომის შესახებ.
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseSerilogRequestLogging(); // რათა დალოგოს ყველა შემომავალი HTTP მოთხოვნა

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
