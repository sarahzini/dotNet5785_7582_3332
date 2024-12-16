
namespace BIApi;
public interface IAdmin
{
    DateTime GetClock();
    void ForwardClock(BO.TimeUnit unit);
    TimeSpan GetRiskRange();
    void SetRiskRange(TimeSpan RiskRange);
    void ResetDB();
    void InitializeDB();

}
