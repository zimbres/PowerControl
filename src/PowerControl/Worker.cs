namespace PowerControl;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly Configuration _configuration;
    private readonly IamAliveService _iamAliveService;
    private readonly CommandService _commandService;
    private readonly MqttService _mqttService;

    public Worker(ILogger<Worker> logger, IConfiguration configuration, IamAliveService iamAliveService, CommandService commandService, MqttService mqttService)
    {
        _logger = logger;
        _configuration = configuration.GetSection(nameof(Configuration)).Get<Configuration>();
        _iamAliveService = iamAliveService;
        _commandService = commandService;
        _mqttService = mqttService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogWarning("Power Control version: {Version}.", Assembly.GetExecutingAssembly().GetName().Version.ToString());
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_configuration.UseMqtt)
            {
                _mqttService.ExecuteCommand(stoppingToken);
                await Task.Delay(-1, stoppingToken);
            }

            await _commandService.ExecuteCommandAsync();

            if (_configuration.IamAliveEnabled)
            {
                await _iamAliveService.GetHttpAsync();
            }
            await Task.Delay(_configuration.Delay * 1000, stoppingToken);
        }
    }
}
