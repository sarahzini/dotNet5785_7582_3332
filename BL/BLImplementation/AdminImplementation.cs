using BlApi;
using Helpers;
namespace BlImplementation;
internal class AdminImplementation : IAdmin
{
    /// <summary>
    /// The DAL instance used by the BL.
    /// </summary>
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public DateTime GetClock() => ClockManager.Now;

    /// <summary>
    /// This method advances the clock by the specified time unit.
    /// </summary>
    /// <param name="unit"></param>
    public void ForwardClock(BO.TimeUnit unit) => ClockManager.UpdateClock(unit switch
    {
        BO.TimeUnit.Minute => ClockManager.Now.AddMinutes(1),
        BO.TimeUnit.Hour => ClockManager.Now.AddHours(1),
        BO.TimeUnit.Day => ClockManager.Now.AddDays(1),
        BO.TimeUnit.Month => ClockManager.Now.AddMonths(1),
        BO.TimeUnit.Year => ClockManager.Now.AddYears(1),
        _ => DateTime.MinValue
    });

    /// <summary>
    /// This method returns the risk range.
    /// </summary>
    public TimeSpan GetRiskRange() 
    {
        return _dal.Config.RiskRange;
    }

    /// <summary>
    /// This method sets the risk range.
    /// </summary>
    public void SetRiskRange(TimeSpan riskRange)
    {
        _dal.Config.RiskRange = riskRange;
    }

    /// <summary>
    /// This method initializes the database.
    /// </summary>
    public void InitializeDB()
    {
        DalTest.Initialization.Do();
        ClockManager.UpdateClock(ClockManager.Now);
    }

    /// <summary>
    /// This method resets the database.
    /// </summary>
    public void ResetDB()
    {
        _dal.ResetDB();
        ClockManager.UpdateClock(ClockManager.Now);
    }
}
