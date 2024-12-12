
using BIApi;

namespace BIImplementation;

internal partial class CallImplementation:ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
}
