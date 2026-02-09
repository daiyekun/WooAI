using Dev.WooAI.AgentPlugin;
using System;
using System.ComponentModel;


namespace Dev.WooAI.AiGatewayService.Plugins;

public class TimePlugin : AgentPluginBase
{
    [Description("获取当前系统时间")]
    public string GetCurrentTime()
    {
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}