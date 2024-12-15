
namespace BIApi;
public interface IAdmin
{
    void InitializeDB();
    void ResetDB();
    TimeSpan GetRiskRange();
    void SetRiskRange(TimeSpan RiskRange);
    DateTime GetClock();
    void ForwardClock(BO.TimeUnit unit);
}
