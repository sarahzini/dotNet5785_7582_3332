namespace BlApi;
public interface IAdmin
{
    DateTime GetClock();
    void ForwardClock(BO.TimeUnit unit);
    TimeSpan GetRiskRange();
    void SetRiskRange(TimeSpan RiskRange);
    void ResetDB();
    void InitializeDB();
    void AddConfigObserver(Action configObserver);
    void RemoveConfigObserver(Action configObserver);
    void AddClockObserver(Action clockObserver);
    void RemoveClockObserver(Action clockObserver);
    void StartSimulator(int interval);
    void StopSimulator();
    void AddSimulatorStoppedObserver(Action observer);
    void RemoveSimulatorStoppedObserver(Action observer);
}
