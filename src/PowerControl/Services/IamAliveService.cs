namespace PowerControl.Services;

public class IamAliveService
{
    private readonly ILogger<IamAliveService> _logger;
    private readonly Configuration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public IamAliveService(ILogger<IamAliveService> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration.GetSection(nameof(Configuration)).Get<Configuration>();
    }

    public async Task GetHttpAsync()
    {
        var _httpClient = _httpClientFactory.CreateClient("Default");
        try
        {
            await _httpClient.GetAsync(_configuration.IamAliveUrl);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Exception occurred while making HTTP request to {Url}", _configuration.Url);
        }
    }
}
