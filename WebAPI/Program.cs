using DBManager;
using System.Text;
using DBManager.Pattern.Interface;
using DBManager.Pattern.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebAPI.Data;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

///
string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
                                                options.UseSqlServer(connection));
builder.Services.AddUnitOfWork<AppDbContext>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork<AppDbContext>>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                        ValidateIssuerSigningKey = true
                    };
                });
builder.Services.AddCors(options => {
    options.AddPolicy("EnableCORS", builder => {
        builder.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
///

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
        c => {
            c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Description = "Please insert token",
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });
            c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{ }
                }
            });
        }
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("EnableCORS");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
