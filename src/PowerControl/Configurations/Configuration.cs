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
}
