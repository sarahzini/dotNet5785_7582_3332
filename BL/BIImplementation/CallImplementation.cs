
using BIApi;

namespace BIImplementation;

internal class CallImplementation:ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
}
