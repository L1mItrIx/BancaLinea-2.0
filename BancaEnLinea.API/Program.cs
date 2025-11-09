using BancaEnLinea.BW.CU;
using BancaEnLinea.BW.Interfaces.BW;
using BancaEnLinea.BW.Interfaces.DA;
using BancaEnLinea.DA.Acciones;
using BancaEnLinea.DA.Config;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar CORS
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowAllOrigins",
      policy =>
      {
        policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
      });
});

// Configurar DbContext
builder.Services.AddDbContext<BancaEnLineaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar servicios
builder.Services.AddTransient<IGestionCuentaBW, GestionCuentaBW>();
builder.Services.AddTransient<IGestionCuentaDA, GestionCuentaDA>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAllOrigins");

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
