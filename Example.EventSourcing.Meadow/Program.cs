using Example.EventSourcing.Meadow;
using Meadow;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.SqlServer;
using Microsoft.Extensions.Logging.LightWeight;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEnTier();
builder.Services.AddMeadowUnitOfWork<MeadowConfigurationProvider>();

builder.Services.AddTransient<ILogger>(sp => new ConsoleLogger().Shorten().EnableAll());


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

app.Services.GetService<ILogger>()?.UseForMeadow();

var configurations = app.Services.GetService<IMeadowConfigurationProvider>()?.GetConfigurations();

var engine = new MeadowEngine(configurations);

engine.UseSqlServer();

if (engine.DatabaseExists())
{
    engine.DropDatabase();
}

engine.CreateDatabase();

engine.BuildUpDatabase();


app.Run();
