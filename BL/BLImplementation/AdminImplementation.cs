using BIApi;
using Helpers;

namespace BLImplementation;
internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
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
    public TimeSpan GetRiskRange() 
    {
        return _dal.Config.RiskRange;
    }
    public void SetRiskRange(TimeSpan riskRange)
    {
        _dal.Config.RiskRange = riskRange;
    }
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
}
