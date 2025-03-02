# Power Control

The PowerControl is a background service that monitors commands from an HTTP service and performs system actions, such as remote shutdowns. It also sends periodic "heartbeat" messages to confirm that the software is operational.

### Configuration required:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.Hosting.Lifetime": "Warning"
    }
  },
  "Configuration": {
    "Url": "https://127.0.0.1/get_power_command",
    "Data": "shutdown",
    "Delay": 60,
    "Command": "shutdown",
    "Arguments": "/s /t 60",
    "IamAliveUrl": "https://127.0.0.1/iam_alive",
    "IamAliveEnabled": false,
    "UseMqtt": false,
    "Broker": "127.0.0.1",
    "Port": 1883,
    "Topic": "example/topic",
    "WillTopic": "example/LWT",
    "Username": "",
    "Password": ""
  }
}
```

---

[Microsoft Help: Create Windows Service](https://learn.microsoft.com/en-us/dotnet/core/extensions/windows-service)

---

Application icon by [Flaticon](https://www.flaticon.com/free-icons/power)