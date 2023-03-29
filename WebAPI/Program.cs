using DBManager;
using Microsoft.EntityFrameworkCore;
using DBManager.Pattern.UnitOfWork;


var builder = WebApplication.CreateBuilder(args);

///
string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
                                                options.UseSqlServer(connection));
builder.Services.AddUnitOfWork<AppDbContext>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork<AppDbContext>>();

//builder.Services.AddTransient<>();
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//                .AddJwtBearer(options => {
//                    options.TokenValidationParameters = new TokenValidationParameters {
//                        ValidateIssuer = true,
//                        ValidateAudience = true,
//                        ValidateLifetime = true,
//                        ValidateIssuerSigningKey = true,
//                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
//                        ValidAudience = builder.Configuration["Jwt:Audience"],
//                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//                    };
//                });


///
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

///
app.UseAuthentication();
///

app.UseAuthorization();

app.MapControllers();
///
//ServiceProvider = builder.Services.BuildServiceProvider();
///
app.Run();

//Используется для получения и работы с DI объектами.
//public partial class Program {
//    public static IServiceProvider ServiceProvider { get; private set; }
//}