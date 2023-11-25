using chia.dotnet;

namespace Web2Gateway;

/// <summary>
/// Provides methods for interacting with the Chia blockchain.
/// </summary>
public sealed class ChiaConfig
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ChiaConfig> _logger;

    public ChiaConfig(ILogger<ChiaConfig> logger, IConfiguration configuration) =>
            (_logger, _configuration) = (logger, configuration);

    private Config GetConfig()
    {
        // first see if we have a config file path in the appsettings.json
        var configPath = _configuration.GetValue("App:chia_config_path", "");
        if (!string.IsNullOrEmpty(configPath))
        {
            _logger.LogInformation("Using chia config {configPath}", configPath);

            return Config.Open(configPath);
        }

        // if not use the chia default '~/.chia/mainnet/config/config.yaml'
        _logger.LogInformation("Using default chia config");

        return Config.Open();
    }

    public EndpointInfo GetDataLayerEndpoint()
    {
        // first check user secrets for the data_layer connection
        // https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&tabs=windows
        var dataLayerUri = _configuration.GetValue("data_layer_uri", "")!;
        if (!string.IsNullOrEmpty(dataLayerUri))
        {
            _logger.LogInformation("Connecting to {dataLayerUri}", dataLayerUri);
            return new EndpointInfo()
            {
                Uri = new Uri(dataLayerUri),
                // when stored in an environment variable the newlines might be escaped
                Cert = _configuration.GetValue("data_layer_cert", "")!.Replace("\\n", "\n"),
                Key = _configuration.GetValue("data_layer_key", "")!.Replace("\\n", "\n")
            };
        }
        else
        {
            // if not present see if we can get it from the config file
            return GetConfig().GetEndpoint("data_layer");
        }
    }
}

