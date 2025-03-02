namespace PowerControl.Configurations;

internal class Configuration
{
    public string Url { get; set; }
    public string Data { get; set; }
    public int Delay { get; set; }
    public string Command { get; set; }
    public string Arguments { get; set; }
    public string IamAliveUrl { get; set; }
    public bool IamAliveEnabled { get; set; }
    public bool UseMqtt { get; set; }
    public string Broker { get; set; }
    public int Port { get; set; }
    public string Topic { get; set; }
    public string WillTopic { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}
