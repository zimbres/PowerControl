var builder = Host.CreateApplicationBuilder(args);

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    builder.Services.AddWindowsService(options =>
    {
        options.ServiceName = "Power Control";
    });
}
else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    builder.Services.AddSystemd();
}

builder.Services.AddHttpClient("Default")
.ConfigurePrimaryHttpMessageHandler(() =>
{
    return new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (m, c, ch, e) => true
    };
});

builder.Services.AddSingleton<HttpService>();
builder.Services.AddSingleton<IamAliveService>();
builder.Services.AddSingleton<CommandService>();
builder.Services.AddSingleton<MqttService>();

builder.Services.AddHostedService<Worker>().Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

var host = builder.Build();

await host.RunAsync();
