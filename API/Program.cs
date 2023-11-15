using API.Data;
using API.Setup;
using Infra.Catalogo;
using System.Reflection;
using Domain.Configuration;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Filters;
using Application.Catalogo.AutoMapper;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Configuration.AddAmazonSecretsManager("us-west-2", "soat1-grp13");
builder.Services.Configure<Secrets>(builder.Configuration);

var connectionString = builder.Configuration.GetSection("ConnectionString").Value;

string secret = builder.Configuration.GetSection("ClientSecret").Value;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));

builder.Services.AddDbContext<CatalogoContext>(options =>
        options.UseNpgsql(connectionString));

builder.Services.AddAuthenticationJWT(secret);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenConfig();

builder.Services.AddAutoMapper(typeof(ProdutosMappingProfile));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.RegisterServices();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseReDoc(c =>
{
    c.DocumentTitle = "REDOC API Documentation";
    c.SpecUrl = "/swagger/v1/swagger.json";
});


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await using var scope = app.Services.CreateAsyncScope();
using var dbApplication = scope.ServiceProvider.GetService<ApplicationDbContext>();
using var dbCatalogo = scope.ServiceProvider.GetService<CatalogoContext>();

await dbApplication!.Database.MigrateAsync();
await dbCatalogo!.Database.MigrateAsync();

app.Run();
