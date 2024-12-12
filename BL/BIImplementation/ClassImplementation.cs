
using BIApi;

namespace BIImplementation;

internal partial class CallImplementation : ICall
{
    private readonly DalApi.IDal v_dal = DalApi.Factory.Get;
}
