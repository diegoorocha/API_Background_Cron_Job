using API_Background.BackgroundServices;
using API_Background.Data;
using API_Background.Models;
using API_Background.Repositories;
using API_Background.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Entity Framework para SQL Server
builder.Services.AddDbContext<BackgroundDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registra os repositórios e serviços
builder.Services.AddScoped<IVendaService, VendaService>();
builder.Services.AddScoped<IBaseRepository<Venda>, BaseRepository<Venda>>();
builder.Services.AddScoped<IBaseRepository<Carro>, BaseRepository<Carro>>();
builder.Services.AddScoped<IBaseRepository<Cliente>, BaseRepository<Cliente>>();
builder.Services.AddScoped<IBaseRepository<CarroVenda>, BaseRepository<CarroVenda>>();
builder.Services.AddScoped<IBaseRepository<LogVendaProcessado>, BaseRepository<LogVendaProcessado>>();

// Registra o IHostedService (background)
builder.Services.AddHostedService<VendaBackgroundService>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
