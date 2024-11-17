using Dal;
using DalApi;
using DalList;
namespace DaleTest;

internal class Program
{
    private static IAssignment? s_dalAssignement = new AssignmentImplementation(); //stage 1
    private static ICall? s_dalCall = new CallImplementations(); //stage 1
    private static IVolunteer? s_dalVolunteer = new VolunteerImplementations(); //stage 1
    private static IConfig? s_dalConfig = new ConfigImplementation(); //stage 1

}
