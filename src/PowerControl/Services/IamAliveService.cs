namespace PowerControl.Services;

public class IamAliveService
{
    private readonly ILogger<IamAliveService> _logger;
    private readonly Configuration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private HttpClient _httpClient;

    public IamAliveService(ILogger<IamAliveService> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration.GetSection(nameof(Configuration)).Get<Configuration>();
    }

    public async Task GetHttpAsync()
    {
        _httpClient = _httpClientFactory.CreateClient("Default");
        try
        {
            await _httpClient.GetAsync(_configuration.IamAliveUrl);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Exception occurred while making HTTP request to {Url}. {ex}", _configuration.IamAliveUrl, ex.Message);
        }
    }
}
