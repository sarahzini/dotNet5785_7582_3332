namespace DalApi;

/// <summary>
/// Provides configuration settings and operations for the application.
/// </summary>
public interface IConfig
{
    DateTime Clock { get; set; }
    TimeSpan RiskRange { get; set; }
    void Reset();
}
