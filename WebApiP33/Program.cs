using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApiP33.Models;
using WebApiP33.Models.DAL;
using WebApiP33.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "WebApiP33",
    });
    c.EnableAnnotations();
});


builder.Services.AddDbContext<ChatContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IChatService, ChatService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()   // Allow requests from any origin
               .AllowAnyMethod()   // Allow any HTTP method (GET, POST, PUT, etc.)
               .AllowAnyHeader();  // Allow any header
    });
});

var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!);
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.FromMinutes(5)
        };

        //options.Events = new JwtBearerEvents
        //{
        //    //OnChallenge = context =>
        //    //{
        //    //            context.HandleResponse();
        //    //    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        //    //    context.Response.ContentType = "application/json";
        //    //    return context.Response.WriteAsync(
        //    //        JsonSerializer.Serialize(
        //    //        OperationResult<object>.Fail("Invalid token"))
        //    //        );
        //    //},
        //    //OnForbidden = context =>
        //    //{
        //    //    context.Response.StatusCode = StatusCodes.Status403Forbidden;
        //    //    return Task.CompletedTask;
        //    //},
        //    //OnAuthenticationFailed = context =>
        //    //{
        //    //    // ╧юф│  тшъюэє║Є№ё , ъюыш ртЄхэЄшЇ│ърЎ│  эх тфрырё 
        //    //    Console.WriteLine("JWT Auth failed: " + context.Exception.Message);
        //    //    return Task.CompletedTask;
        //    //},
        //};

    });

builder.Services.AddIdentityCore<User>(options => {
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedAccount = false;

    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 3;
})
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<ChatContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization();



var app = builder.Build();

// Auto-apply EF Core migrations on startup (creates DB if it doesn't exist)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ChatContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Only redirect to HTTPS in Development (Docker runs plain HTTP on port 8080)
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();



/*
 * 
 * Користувач
 * Повідомлення
 * 
 * Канал
 * 
 * 
 * 
 * 
 * Месенджер
 * 
 * Авторизація
 * Профіль
 * 
 * Пошук користувачів
 * Надіслати повідомлення
 * Завантажити повідомлення
 * 
 * 
 * 
 */ 