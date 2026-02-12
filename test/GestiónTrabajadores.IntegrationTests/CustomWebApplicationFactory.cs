using GestiónTrabajadores.Application.Interfaces;
using GestiónTrabajadores.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
namespace GestiónTrabajadores.IntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<TrabajadoresDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            var cloudinaryDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(ICloudinaryService));
            if (cloudinaryDescriptor != null)
            {
                services.Remove(cloudinaryDescriptor);
            }
            var mockCloudinary = new Mock<ICloudinaryService>();
            mockCloudinary.Setup(c => c.UploadImageAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync("https://test-image-url.com/test.jpg");
            services.AddScoped(_ => mockCloudinary.Object);
            services.AddDbContext<TrabajadoresDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase_" + Guid.NewGuid().ToString());
            });
            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<TrabajadoresDbContext>();
                var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TProgram>>>();
                db.Database.EnsureCreated();
                logger.LogInformation("Base de datos de prueba creada exitosamente");
            }
        });
        builder.UseEnvironment("Testing");
    }
}