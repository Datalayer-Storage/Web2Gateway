using Web2Gateway;
using chia.dotnet;

// doing all of this in the mini-api expressjs-like approach
// instead of the IActionResult approach

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    builder.Services.AddApplicationInsightsTelemetry();
}

// we can take the path to an appsettings.json file as an argument
// if not provided, the default appsettings.json will be used and settings
// will come from there or from environment variables
if (args.Any())
{
    var configurationBinder = new ConfigurationBuilder()
        .AddJsonFile(args.First());

    var config = configurationBinder.Build();
    builder.Configuration.AddConfiguration(config);
}

// Add services to the container.
builder.Logging.ClearProviders()
    .AddConsole();

builder.Services.AddControllers();

builder.Services.AddSingleton<ChiaConfig>()
    .AddSingleton((provider) => new HttpRpcClient(provider.GetService<ChiaConfig>()!.GetDataLayerEndpoint()))
    .AddSingleton((provider) => new DataLayerProxy(provider.GetService<HttpRpcClient>()!, "g2to3"))
    .AddSingleton<G2To3Service>()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddMemoryCache();

var logger = LoggerFactory.Create(config =>
{
    config.AddConsole();
}).CreateLogger("Program");
var configuration = builder.Configuration;

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger()
    .UseSwaggerUI();

// the service end points are defined in here
app.ConfigureApi(logger)
    .UseCors();

app.MapControllers();
app.Run();
