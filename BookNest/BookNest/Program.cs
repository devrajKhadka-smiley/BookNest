using BookNest.Data;
using BookNest.Data.Entities;
using BookNest.Data.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"))
    );

builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});



var app = builder.Build();

app.UseCors("AllowReactApp");


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    //Role Seedings
    var roleManager = services.GetRequiredService<RoleManager<Role>>();
    var userManager = services.GetRequiredService<UserManager<User>>();

    //Admin Seedings
    await RoleSeeders.SeedRolesAsync(roleManager);
    await AdminSeeders.SeedAdminUserAsync(userManager, roleManager);

}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
