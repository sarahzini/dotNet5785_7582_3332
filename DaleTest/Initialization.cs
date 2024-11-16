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
        //all these arrays was written by AI (excepted adresses/longitude/latitude)

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

        string[] adresses = { "12 Ben Yehuda Street", "45 Hillel Street", "67 Yafo Street",
            "101 Herzl Street", "23 Jaffa Street", "56 Agron Street", "89 Shlomzion Street",
            "32 King George Street", "78 Emek Refaim Street", "5 Derech Hevron",
            "14 Eliezer Kaplan Street", "33 Shmuel Hanavi Street", "8 Dov Hoz Street",
            "50 Keren Hayesod Street", "63 Tchernichovsky Street", "29 Menachem Begin Street",
            "72 Malcha Street", "101 Sderot Yerushalayim Street", "120 Shalom Aleichem Street",
            "57 Har Homa Street" };

        //pas les vraies et faut qu elle soit vraies mais pr l instant on douille
        double[] latitudes = {31.7683, 31.7751, 31.7707, 31.7592, 31.7656, 31.7718, 31.7725, 
            31.7730, 31.7754, 31.7698, 31.7677, 31.7722, 31.7641, 31.7758, 31.7770, 31.7733,
            31.7790, 31.7644, 31.7700, 31.7685};

        double[] longitudes = {35.2137, 35.2075, 35.2123, 35.1890, 35.1908, 35.2087, 35.1994,
            35.2130, 35.2002, 35.2101, 35.2138, 35.2175, 35.2104, 35.2202, 35.2300, 35.2111,
            35.2150, 35.2084, 35.2055, 35.2146};

        Job[] jobs = {Job.Volunteer, Job.Volunteer, Job.Director, Job.Volunteer, Job.Volunteer,
            Job.Volunteer, Job.Volunteer, Job.Volunteer, Job.Volunteer, Job.Volunteer, Job.Volunteer,
            Job.Director, Job.Volunteer, Job.Volunteer, Job.Volunteer, Job.Volunteer, Job.Volunteer,
            Job.Director, Job.Volunteer, Job.Volunteer }

        bool[] actives = { true, true, false, true, true, true, false, true, true,
            true, true, true, false, true, true, true, true, true, false, true };

        WhichDistance[] whichDistances = {WhichDistance.WalkingDistance, WhichDistance.WalkingDistance,
            WhichDistance.AirDistance, WhichDistance.AirDistance, WhichDistance.AirDistance,
            WhichDistance.DrivingDistance, WhichDistance.WalkingDistance, WhichDistance.AirDistance,
            WhichDistance.AirDistance, WhichDistance.AirDistance, WhichDistance.WalkingDistance, 
            WhichDistance.DrivingDistance, WhichDistance.AirDistance, WhichDistance.WalkingDistance, 
            WhichDistance.AirDistance, WhichDistance.DrivingDistance, WhichDistance.AirDistance,
            WhichDistance.AirDistance, WhichDistance.DrivingDistance, WhichDistance.AirDistance };

        for (int i = 0; i < names.Length; i++)
        {
            int id = s_rand.Next(10000000, 40000000); //generate a random id between 10000000 et 40000000
            if (s_dalVolunteer?.Read(id) == null)  //check if we can create a new volunteer
            {
                // Generate a random maximum distance for receiving a call.
                double maxDistance = s_rand.NextDouble() * 100; // Example: random distance between 0 and 100 km.

                // Create a new volunteer object and add it to the data source.
                Volunteer volunteer = new(id, names[i], phoneNumbers[i], emails[i], adresses[i], latitudes[i],
                    longitudes[i], jobs[i], actives[i], maxDistance, whichDistances[i]);
                s_dalVolunteer?.Create(volunteer);
            }
        }
    }

    //pas bonne mAIS JE TE les LAISSE C est celle de github faut les corriger comme j ai corriger volunteer, 
    // faut pas faire de maarah pr les mispar mezaee rats parce que on les genere dans les create a chaue fois
    //oublie pas qu ils sont lies entre eux, que les id de call et volunterr dans assignment c est ceux de call et assignment et faut agir beetem
    private static void createCalls()
    {
        string[] addresses = { "123 Main St", "456 Elm St", "789 Oak St", "101 Pine St" };
        //j ai rajoute dans le enum les messimot pr mada 
        SystemType[] choices = { SystemType.ICUAmbulance, SystemType.RegularAmbulance, SystemType.Driving, SystemType.Administration };


        for (int i = 0; i < addresses.Length; i++)
        {
            Call call = new(i + 1, addresses[i], s_rand.NextDouble() * 90, s_rand.NextDouble() * 180, DateTime.Now, choices[i]);
            if (s_dalCall?.Read(call.Id) == null)
            {
                s_dalCall?.Create(call);
            }
        }
    }

    
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
}


