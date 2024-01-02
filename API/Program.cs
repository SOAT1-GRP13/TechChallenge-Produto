using API.Data;
using API.Setup;
using Infra.Catalogo;
using System.Reflection;
using Domain.ValueObjects;
using Domain.Configuration;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Filters;
using Application.Catalogo.AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

string connectionString = "";
string secret = "";

if (builder.Environment.IsProduction())
{
    builder.Configuration.AddAmazonSecretsManager("us-west-2", "produto-secret");
    builder.Services.Configure<Secrets>(builder.Configuration);

    connectionString = builder.Configuration.GetSection("ConnectionStringProduto").Value;

    secret = builder.Configuration.GetSection("ClientSecret").Value;
} 
else
{
    //local
    builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection(DatabaseSettings.DatabaseConfiguration));
    connectionString = builder.Configuration.GetSection("ConnectionString").Value;

    secret = builder.Configuration.GetSection("ConfiguracaoToken:ClientSecret").Value;

    builder.Services.Configure<ConfiguracaoToken>(builder.Configuration.GetSection(ConfiguracaoToken.Configuration));
}

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

app.UsePathBase(new PathString("/produto"));
app.UseRouting();
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
