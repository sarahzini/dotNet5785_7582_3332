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
    public void ForwardClock(BO.TimeUnit unit)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        AdminManager.UpdateClock(unit switch
        {
            BO.TimeUnit.Minute => AdminManager.Now.AddMinutes(1),
            BO.TimeUnit.Hour => AdminManager.Now.AddHours(1),
            BO.TimeUnit.Day => AdminManager.Now.AddDays(1),
            BO.TimeUnit.Month => AdminManager.Now.AddMonths(1),
            BO.TimeUnit.Year => AdminManager.Now.AddYears(1),
            _ => DateTime.MinValue
        });
    }

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
        AdminManager.ThrowOnSimulatorIsRunning();
        _dal.Config.RiskRange = riskRange;

        CallManager.Observers.NotifyListUpdated();

    }

    /// <summary>
    /// This method initializes the database.
    /// </summary>
    public void InitializeDB()
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.InitializeDB(); //stage 7
    }

    /// <summary>
    /// This method resets the database.
    /// </summary>
    public void ResetDB()
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.ResetDB(); //stage 7
    }

    public void StartSimulator(int interval)  //stage 7
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.Start(interval); //stage 7
    }
    public void StopSimulator()
        => AdminManager.Stop(); //stage 7


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

    /// <summary>
    /// This method adds a simulator stopped observer.
    /// </summary>
    /// <param name="observer">The observer to add</param>
    public void AddSimulatorStoppedObserver(Action observer) =>
        AdminManager.SimulatorStoppedObservers += observer;

    /// <summary>
    /// This method removes a simulator stopped observer.
    /// </summary>
    /// <param name="observer">The observer to remove</param>
    public void RemoveSimulatorStoppedObserver(Action observer) =>
        AdminManager.SimulatorStoppedObservers -= observer;
}
