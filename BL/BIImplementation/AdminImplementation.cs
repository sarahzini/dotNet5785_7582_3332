using BIApi;
using BO;
using Helpers;

namespace BIImplementation;
internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void InitializeDB()
    {
        DalTest.Initialization.Do(); 
        ClockManager.UpdateClock(ClockManager.Now);
    }

    public void ResetDB()
    {
        _dal.ResetDB(); 
        ClockManager.UpdateClock(ClockManager.Now);
    }

    public TimeSpan GetMaxRange() 
    {
        return _dal.config.RiskRange;
    }

    public void SetMaxRange(TimeSpan maxRange)
    {
        _dal.config.RiskRange = maxRange;
    }

    public DateTime GetClock() => ClockManager.Now;

    public void ForwardClock(BO.TimeUnit unit) => ClockManager.UpdateClock(unit switch
    {
        BO.TimeUnit.Minute => ClockManager.Now.AddMinutes(1),
        BO.TimeUnit.Hour => ClockManager.Now.AddHours(1),
        BO.TimeUnit.Day => ClockManager.Now.AddDays(1),
        BO.TimeUnit.Month => ClockManager.Now.AddMonths(1),
        BO.TimeUnit.Year => ClockManager.Now.AddYears(1),
        _ => DateTime.MinValue
    });
}
