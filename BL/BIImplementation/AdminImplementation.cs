using BIApi;
using Helpers;

namespace BIImplementation;
internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    ClockManager.Now;
    ClockManager.UpdateClock(ClockManager.Now.AddMinutes(1));

}
