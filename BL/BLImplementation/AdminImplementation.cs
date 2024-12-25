using BlApi;
using Helpers;
namespace BlImplementation;
internal class AdminImplementation : IAdmin
{
    /// <summary>
    /// The DAL instance used by the BL.
    /// </summary>
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public DateTime GetClock() => AdminManager.Now;

    /// <summary>
    /// This method advances the clock by the specified time unit.
    /// </summary>
    /// <param name="unit"></param>
    public void ForwardClock(BO.TimeUnit unit) => AdminManager.UpdateClock(unit switch
    {
        BO.TimeUnit.Minute => AdminManager.Now.AddMinutes(1),
        BO.TimeUnit.Hour => AdminManager.Now.AddHours(1),
        BO.TimeUnit.Day => AdminManager.Now.AddDays(1),
        BO.TimeUnit.Month => AdminManager.Now.AddMonths(1),
        BO.TimeUnit.Year => AdminManager.Now.AddYears(1),
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
        AdminManager.UpdateClock( AdminManager.Now);
        AdminManager.RiskRange = AdminManager.RiskRange;

    }

    /// <summary>
    /// This method resets the database.
    /// </summary>
    public void ResetDB()
    {
        _dal.ResetDB();
        AdminManager.UpdateClock(AdminManager.Now);
        AdminManager.RiskRange = AdminManager.RiskRange;
    }

    /// <summary>
    /// This method adds a clock observer.
    /// </summary>
    /// <param name="clockObserver">The clock observer to add</param>
    public void AddClockObserver(Action clockObserver) =>
           AdminManager.ClockUpdatedObservers += clockObserver;

    /// <summary>
    /// This method removes a clock observer.
    /// </summary>
    /// <param name="clockObserver">clock observer to remove</param>
    public void RemoveClockObserver(Action clockObserver) =>
           AdminManager.ClockUpdatedObservers -= clockObserver;

    /// <summary>
    /// This method adds a configuration observer.
    /// </summary>
    /// <param name="configObserver">The config observer to add</param>
    public void AddConfigObserver(Action configObserver) =>
           AdminManager.ConfigUpdatedObservers += configObserver;

    /// <summary>
    /// This method removes a configuration observer.
    /// </summary>
    /// <param name="configObserver">The config observer to remove</param>
    public void RemoveConfigObserver(Action configObserver) =>
           AdminManager.ConfigUpdatedObservers -= configObserver;

}
