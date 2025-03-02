namespace PowerControl.Services;

public class MqttService
{
    private readonly ILogger<MqttService> _logger;
    private readonly Configuration _configuration;
    private readonly MqttClientFactory _mqttFactory;
    private readonly IMqttClient _mqttClient;

    public MqttService(ILogger<MqttService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration.GetSection(nameof(Configuration)).Get<Configuration>();
        _mqttFactory = new MqttClientFactory();
        _mqttClient = _mqttFactory.CreateMqttClient();
    }

    public void ExecuteCommand(CancellationToken stoppingToken)
    {
        try
        {
            var optionsBuilder = new MqttClientOptionsBuilder()
                .WithTcpServer(_configuration.Broker, _configuration.Port)
                .WithCleanSession()
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(30))
                .WithWillQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithWillRetain(true)
                .WithWillTopic(_configuration.WillTopic)
                .WithWillPayload("Offline");
            if (!string.IsNullOrEmpty(_configuration.Username) && !string.IsNullOrEmpty(_configuration.Password))
            {
                optionsBuilder.WithCredentials(_configuration.Username, _configuration.Password);
            }
            var options = optionsBuilder.Build();

            _mqttClient.ConnectedAsync += async e =>
            {
                var onlineMessage = new MqttApplicationMessageBuilder()
                    .WithTopic(_configuration.WillTopic)
                    .WithPayload("Online")
                    .WithRetainFlag()
                    .Build();
                await _mqttClient.PublishAsync(onlineMessage);
            };

            _mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                if (string.Equals(payload, _configuration.Data, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("Received message: {payload}", payload);
                    Process.Start(_configuration.Command, _configuration.Arguments);
                }
                return Task.CompletedTask;
            };

            _ = Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        if (!await _mqttClient.TryPingAsync())
                        {
                            await _mqttClient.ConnectAsync(options, CancellationToken.None);
                            await _mqttClient.SubscribeAsync(_configuration.Topic, MqttQualityOfServiceLevel.AtMostOnce);

                            _logger.LogInformation("Connected to MQTT broker.");
                        }
                    }
                    catch
                    {
                        _logger.LogError("A connection error occurred with the MQTT broker!");
                    }
                    finally
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5));
                    }
                }
            }, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while connecting to MQTT broker!");
        }
    }
}
