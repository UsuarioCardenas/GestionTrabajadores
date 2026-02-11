using FluentValidation;
using GestiónTrabajadores.Application.Interfaces;
using GestiónTrabajadores.Application.Mappings;
using GestiónTrabajadores.Application.Services;
using GestiónTrabajadores.Application.Validators;
using GestiónTrabajadores.Infrastructure.Data;
using GestiónTrabajadores.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TrabajadoresDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TrabajadoresConnection")));

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

builder.Services.AddValidatorsFromAssemblyContaining<CreateTrabajadorDtoValidator>();

builder.Services.AddScoped<ITrabajadorRepository, TrabajadorRepository>();
builder.Services.AddScoped<ITrabajadorService, TrabajadorService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazor");
app.UseAuthorization();
app.MapControllers();

app.Run();