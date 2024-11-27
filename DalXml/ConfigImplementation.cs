namespace Dal;
using DalApi;
using DO;

// This class implements the IConfig interface and provides access to configuration settings.
internal class ConfigImplementation : IConfig
{
    // Property to get or set the Clock configuration.
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }

    // Property to get or set the RiskRange configuration.
    public TimeSpan RiskRange
    {
        get => Config.RiskRange;
        set => Config.RiskRange = value;
    }

    // Method to reset the configuration settings.
    public void Reset()
    {
        Config.Reset();
    }
}
