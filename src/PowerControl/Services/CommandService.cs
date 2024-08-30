namespace PowerControl.Services;

public class CommandService
{
    private readonly ILogger<CommandService> _logger;
    private readonly Configuration _configuration;
    private readonly HttpService _httpService;

    public CommandService(ILogger<CommandService> logger, HttpService httpService, IConfiguration configuration)
    {
        _logger = logger;
        _httpService = httpService;
        _configuration = configuration.GetSection(nameof(Configuration)).Get<Configuration>();

    }
    public async Task ExecuteCommandAsync()
    {
        if (await _httpService.CheckCommandAsync())
        {
            try
            {
                Process.Start(_configuration.Command, _configuration.Arguments);
                _logger.LogWarning("Shutdown!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occured.");
            }
        }
    }
}
