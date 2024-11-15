namespace DalTest;
using DalApi;
using DO;
public static class Initialization
{
    // Fields for managing various entities and configurations in the Data Access Layer (DAL).
    private static IVolunteer? s_dalVolunteer;
    private static IAssignment? s_dalAssignement;
    private static ICall? s_dalCall;
    private static IConfig? s_dalConfig;
    private static readonly Random s_rand = new();

    /// <summary>
    ///Bonjour Sarah mon coeur
    /// </summary>
    private static void createVolunteers()
    {
        //all these arrays was written by AI
        int[] Id = { 13284756, 21765348, 30198475, 25073916, 16284039, 31456829, 22904175,
            31076452, 12837465, 23195608, 17583924, 29684713, 30317628, 14832075, 25369401,
            31752084, 18045923, 23187506, 12690847, 23816579 };

        string[] names = { "Sarah Cohen", "Osher Mizrahi", "Yaara Levi", "Eli Ben-David", "Maya Shapiro", "Yair Katz",
            "Noa Peretz", "Aviad Cohen", "Tamar Israeli", "Lior Baruch", "Daniella Rosen", "Oren Goldstein",
            "Michal Avrahami", "Nadav Shulman", "Rachel Dubinsky", "Uri Dahan", "Yael Chaimovitz", "Ronit Gross",
            "Meir Ziv", "Tal Ben-Ari", "Hila Zaken" };

        string[] phoneNumbers = { "052-3918274", "050-5639842", "054-1283795", "053-6472391", "058-9042318", "050-9834657",
            "052-7512386", "053-4326759", "054-1863420", "058-2917463", "050-3275641", "052-5068493", "053-7821594",
            "054-9342105", "058-6120394", "050-7461283", "052-9784530", "053-5289761", "054-3175928", "058-8456201" };

        string[] emails = { "Sarah.Cohen@gmail.com", "Osher.Mizrahi@yahoo.com", "Yaara.Levi@outlook.com", "Eli.BenDavid@mail.com", 
            "Maya.Shapiro@icloud.com", "Yair.Katz@googlemail.com", "Noa.Peretz@hotmail.com", "Aviad.Cohen@live.com",
            "Tamar.Israeli@protonmail.com", "Lior.Baruch@aol.com", "Daniella.Rosen@zoho.com", "Oren.Goldstein@fastmail.com",
            "Michal.Avrahami@ymail.com", "Nadav.Shulman@hushmail.com", "Rachel.Dubinsky@msn.com", "Uri.Dahan@comcast.net",
            "Yael.Chaimovitz@outlook.co.il", "Ronit.Gross@tutanota.com", "Meir.Ziv@me.com", "Tal.BenAri@icloud.com",
            "Hila.Zaken@webmail.com" };

        for (int i = 0; i < names.Length; i++)
        {
            if (s_dalVolunteer?.Read(Id[i]) == null)
            {

                // Generate a random maximum distance for receiving a call.
                double maxDistance = s_rand.NextDouble() * 100; // Example: random distance between 0 and 100 km.

                // Create a new volunteer object and add it to the data source.
                Volunteer volunteer = new(Id[i], names[i], phoneNumbers[i], emails[i], distance: maxDistance);
                s_dalVolunteer?.Create(volunteer);
            }
        }
    }

    //a partir de la il faut voir pr les mispar mezaee rats
    private static void createAssignments()
    {
        for (int i = 0; i < 10; i++)
        {
            Assignment assignment = new(i + 1, s_rand.Next(1, 5), s_rand.Next(1, 5), DateTime.Now);
            if (s_dalAssignement?.Read(assignment.Id) == null)
            {
                s_dalAssignement?.Create(assignment);
            }
        }
    }

    private static void createCalls()
    {
        string[] addresses = { "123 Main St", "456 Elm St", "789 Oak St", "101 Pine St" };
        SystemType[] choices = { SystemType.Food, SystemType.Medical, SystemType.Transport, SystemType.Other };

        for (int i = 0; i < addresses.Length; i++)
        {
            Call call = new(i + 1, addresses[i], s_rand.NextDouble() * 90, s_rand.NextDouble() * 180, DateTime.Now, choices[i]);
            if (s_dalCall?.Read(call.Id) == null)
            {
                s_dalCall?.Create(call);
            }
        }
    }
}


