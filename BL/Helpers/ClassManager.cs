using BO;
using DalApi;
using DO;
using System.Net;
using System.Text.RegularExpressions;
namespace Helpers;
/// <summary>
/// 
/// </summary>
internal static partial class CallManager
{
    private static IDal v_dal = Factory.Get; //stage 4
    internal static void ValidateCallDetails(BO.Call volunteer)
    {
        //appeler celle de Sara
       
    }
    internal static DO.Call ConvertToLogicCall(BO.Call call)
    {
        return new DO.Call
        {
            Id=call.CallId,
            Address=null,
            Latitude=call.CallLatitude,
            Longitude=call.CallLongitude,
            DateTime=call.BeginTime, 
            Choice=(DO.SystemType)call.TypeOfCall,
            Description = null,  //pas bon a changer
            EndDateTime = null  //pas bon a changer
        };
    }

}
