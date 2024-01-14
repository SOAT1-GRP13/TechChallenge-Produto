using API.Setup;
using System.Text;
using Infra.Catalogo;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Application.Catalogo.AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Tests
{
    public class TestStartup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            var jsonString = @"{""ConnectionString"": ""teste""}";

            var configuration = new ConfigurationBuilder()
                    .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
                    .Build();

            services.AddSingleton<IConfiguration>(configuration);

            services.AddDbContext<CatalogoContext>(options =>
                options.UseNpgsql("User ID=fiap;Password=S3nh@L0c@lP40dut0;Host=localhost;Port=15433;Database=techChallengeProduto;Pooling=true;"));

            services.AddAutoMapper(typeof(ProdutosMappingProfile));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            DependencyInjection.RegisterServices(services);

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
    }
}
