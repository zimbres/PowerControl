namespace PowerControl.Services;

public class HttpService
{
    private readonly ILogger<HttpService> _logger;
    private readonly Configuration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public HttpService(ILogger<HttpService> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration.GetSection(nameof(Configuration)).Get<Configuration>();
    }

    public async Task<bool> CheckCommandAsync()
    {
        var _httpClient = _httpClientFactory.CreateClient("Default");
        try
        {
            var response = await _httpClient.GetAsync(_configuration.Url);
            var content = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(content);
            var data = document.RootElement.GetRawText();
            if (data.Contains(_configuration.Data))
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Exception occurred while making HTTP request to {Url}", _configuration.Url);
            return false;
        }
        return false;
    }
}
