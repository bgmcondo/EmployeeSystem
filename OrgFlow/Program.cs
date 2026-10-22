using Microsoft.EntityFrameworkCore;
using OrgFlow.Data;
using OrgFlow.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//Dependecy injection için servis kaydı oluşturuyoruz
builder.Services.AddDbContext<AppDbContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();


builder.Services.AddScoped<IEmployeeService, EmployeeService>(); //IEmployeeService interface'ini kullanarak EmployeeService sınıfına erişim sağlanır. Dependency Injection ile IEmployeeService interface'ini alır ve _employeeService alanına atar.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
