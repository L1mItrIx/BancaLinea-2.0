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

// Registrar servicios de Cuenta
builder.Services.AddTransient<IGestionCuentaBW, GestionCuentaBW>();
builder.Services.AddTransient<IGestionCuentaDA, GestionCuentaDA>();

// Registrar servicios de CuentaBancaria
builder.Services.AddTransient<IGestionCuentaBancariaBW, GestionCuentaBancariaBW>();
builder.Services.AddTransient<IGestionCuentaBancariaDA, GestionCuentaBancariaDA>();

// Registrar servicios de Beneficiario
builder.Services.AddTransient<IGestionBeneficiarioBW, GestionBeneficiarioBW>();
builder.Services.AddTransient<IGestionBeneficiarioDA, GestionBeneficiarioDA>();

// Registrar servicios de Transferencia
builder.Services.AddTransient<IGestionTransferenciaBW, GestionTransferenciaBW>();
builder.Services.AddTransient<IGestionTransferenciaDA, GestionTransferenciaDA>();

// Registrar servicios de Servicio
builder.Services.AddTransient<IGestionServicioBW, GestionServicioBW>();
builder.Services.AddTransient<IGestionServicioDA, GestionServicioDA>();

// Registrar servicios de ContratoServicio
builder.Services.AddTransient<IGestionContratoServicioBW, GestionContratoServicioBW>();
builder.Services.AddTransient<IGestionContratoServicioDA, GestionContratoServicioDA>();

// Registrar servicios de PagoServicio
builder.Services.AddTransient<IGestionPagoServicioBW, GestionPagoServicioBW>();
builder.Services.AddTransient<IGestionPagoServicioDA, GestionPagoServicioDA>();

// Registrar servicios de Accion
builder.Services.AddTransient<IGestionAccionBW, GestionAccionBW>();
builder.Services.AddTransient<IGestionAccionDA, GestionAccionDA>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ⬅️ IMPORTANTE: CORS PRIMERO
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