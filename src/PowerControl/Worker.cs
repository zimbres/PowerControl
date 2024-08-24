namespace PowerControl;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly HttpService _httpService;
    private readonly Configuration _configuration;
    private readonly IamAliveService _iamAliveService;

    public Worker(ILogger<Worker> logger, HttpService httpService, IConfiguration configuration, IamAliveService iamAliveService)
    {
        _logger = logger;
        _httpService = httpService;
        _configuration = configuration.GetSection(nameof(Configuration)).Get<Configuration>();
        _iamAliveService = iamAliveService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogWarning("Power Control version: {version}.", Assembly.GetExecutingAssembly().GetName().Version.ToString());

        while (!stoppingToken.IsCancellationRequested)
        {
            if (await _httpService.CheckCommandAsync())
            {
                try
                {
                    _logger.LogWarning("Shutdown!");
                    Process.Start(_configuration.Command, _configuration.Arguments);
                }
                catch (Exception ex)
                {
                    _logger.LogError("{ex}", ex);
                }
            }
            if (_configuration.IamAliveEnabled)
            {
                await _iamAliveService.GetHttpAsync();
            }
            await Task.Delay(_configuration.Delay * 1000, stoppingToken);
        }
    }
}
