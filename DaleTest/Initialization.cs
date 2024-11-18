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


       
        string[] adresses = { 
            "12 Ben Yehuda Street, Jerusalem", "45 Hillel Street, Jerusalem", "67 Yafo Street, Jerusalem", "101 Herzl Street, Jerusalem", "23 King David Street, Jerusalem",
            "56 Agron Street, Jerusalem", "89 Shlomzion Hamalka Street, Jerusalem", "32 King George Street, Jerusalem", "78 Emek Refaim Street, Jerusalem", "5 Derech Hevron, Jerusalem",
            "14 Eliezer Kaplan Street, Jerusalem", "33 Shmuel Hanavi Street, Jerusalem", "8 Dov Hoz Street, Jerusalem", "50 Keren Hayesod Street, Jerusalem", "63 Tchernichovsky Street, Jerusalem",
            "29 Menachem Begin Street, Jerusalem", "72 Malcha Street, Jerusalem", "101 Sderot Yerushalayim Street, Jerusalem", "120 Shalom Aleichem Street, Jerusalem", "57 Har Homa Street, Jerusalem"
            };

        double[] latitudes = {
    31.7815951,
    31.7793341,
    31.783678,
    31.780965,
    31.7745543,
    31.7779632,
    31.7788813,
    31.779475,
    31.759882,
    31.779475,
    31.779277,
    31.7898752,
    31.756329,
    31.7716677,
    31.7632657,
    31.777348,
    31.7943135,
    31.7797692,
    31.771344,
    31.7677499
};

        double[] longitudes = {
    35.2175388,
    35.2214706,
    35.216553,
    35.188818,
    35.2226839,
    35.2221859,
    35.2226822,
    35.215608,
    35.178086,
    35.2151934,
    35.2017399,
    35.2249504,
    35.195831,
    35.2224285,
    35.2023972,
    35.212409,
    35.2244155,
    35.1880402,
    35.2207611,
    35.162764
};

        Job[] jobs = {Job.Volunteer, Job.Volunteer, Job.Director, Job.Volunteer, Job.Volunteer,
            Job.Volunteer, Job.Volunteer, Job.Volunteer, Job.Volunteer, Job.Volunteer, Job.Volunteer,
            Job.Director, Job.Volunteer, Job.Volunteer, Job.Volunteer, Job.Volunteer, Job.Volunteer,
            Job.Director, Job.Volunteer, Job.Volunteer };

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

    private static void createCalls()
    {
        string[] addresses =
        { "10 yafo st, Jerusalem","2 Jaffa St, Jerusalem","3 Ben Yehuda St, Jerusalem","4 Herzl St, Jerusalem","5 Azza st, Jerusalem",
          "6 King George St, Jerusalem", "50 King George St, Jerusalem", "12 King George St, Jerusalem", "45 Jaffa St, Jerusalem",
          "78 Ben Yehuda St, Jerusalem","23 Agron St, Jerusalem","56 Hillel St, Jerusalem","89 Shlomzion Hamalka St, Jerusalem,",
          "34 HaNevi'im St, Jerusalem","67 Emek Refaim St, Jerusalem","90 Derech Hebron St, Jerusalem", "11 Keren Hayesod St, Jerusalem",
          "22 Ramban St, Jerusalem, Israel","33 Gaza St, Jerusalem","44 Azza St, Jerusalem, Israel","55 Bezalel St, Jerusalem",
          "66 Haneviim St, Jerusalem","77 Shmuel Hanagid St, Jerusalem","88 King David St, Jerusalem","99 Yaffo St, Jerusalem",
          "100 Agripas St, Jerusalem","101 Haneviim St, Jerusalem","102 Ben Maimon St, Jerusalem","103 Keren Kayemet St, Jerusalem",
          "104 Shatz St, Jerusalem","105 Hillel St, Jerusalem","106 Shlomzion Hamalka St, Jerusalem","107 King George St, Jerusalem",
          "108 Jaffa St, Jerusalem","109 Ben Yehuda St, Jerusalem","110 Agron St, Jerusalem","111 Haneviim St, Jerusalem",
          "112 Emek Refaim St, Jerusalem","113 Derech Hebron St, Jerusalem","114 Keren Hayesod St, Jerusalem","115 Ramban St, Jerusalem",
          "116 Azza St, Jerusalem","117 Azza St, Jerusalem","118 Bezalel St, Jerusalem","119 Haneviim St, Jerusalem","120 Shmuel Hanagid St, Jerusalem",
          "121 King David St, Jerusalem","122 Yaffo St, Jerusalem","123 Agripas St, Jerusalem","124 Haneviim St, Jerusalem"
        };

        double[] latitudes = {31.753383,31.7780277,31.781655,31.7880941,31.773728,31.782715,31.7767741,31.782273,31.782449,31.7809244,31.759239,32.025681,31.7788813,
                              31.7842556,31.759897,31.7832124,31.774514,31.774893,31.771599,31.771743,31.7844665,31.793836,31.7749251,31.7846329,31.7846329,31.7849633,
                              31.7737395,31.7972733,31.7801538,31.7793341,31.7787378,31.7757616,31.7757616,31.7859455,31.7806127,31.7779632,31.7849633,31.759882,
                              31.75279,31.750182,31.7741998,31.7704436,35.2116773,31.7701754,31.782949,31.7849633,31.794604,31.7740468,31.786086,31.784526,31.7852595
                             };

        double[] longitudes = {35.222896,35.2257855,35.218937,35.199252,35.216547,35.217257,35.216209,35.216872,35.218715,35.2143122,35.205496,34.751687,35.2226822,
                               35.2243949,35.215612,35.1948336,35.218761,35.2147,35.213181,35.212941,35.2194595,35.221176,35.2217823,35.2147682,35.2147682,35.2151037,
                               35.2112687,35.1419648,35.2149897,35.2214706,35.2223413,35.2176332,35.2176332,35.2126195,35.2144134,35.2221859,35.2151037,35.2151934,
                               35.220242,34.992038,35.2107476,35.2116773,35.2118899,35.2092297,35.2151037,35.221121,35.2223356,35.21214,35.211011,35.2152564
                              };

        SystemType[] types = { SystemType.RegularAmbulance, SystemType.RegularAmbulance,
            SystemType.ICUAmbulance, SystemType.RegularAmbulance, SystemType.ICUAmbulance,
            SystemType.ICUAmbulance, SystemType.RegularAmbulance, SystemType.RegularAmbulance,
            SystemType.RegularAmbulance, SystemType.RegularAmbulance, SystemType.ICUAmbulance,
            SystemType.RegularAmbulance, SystemType.RegularAmbulance, SystemType.RegularAmbulance,
            SystemType.RegularAmbulance, SystemType.RegularAmbulance, SystemType.RegularAmbulance,
            SystemType.ICUAmbulance, SystemType.ICUAmbulance, SystemType.RegularAmbulance,
            SystemType.ICUAmbulance, SystemType.ICUAmbulance, SystemType.RegularAmbulance,
            SystemType.RegularAmbulance, SystemType.ICUAmbulance, SystemType.RegularAmbulance,
            SystemType.RegularAmbulance, SystemType.ICUAmbulance, SystemType.RegularAmbulance,
            SystemType.ICUAmbulance, SystemType.RegularAmbulance, SystemType.ICUAmbulance,
            SystemType.ICUAmbulance, SystemType.RegularAmbulance, SystemType.RegularAmbulance,
            SystemType.RegularAmbulance, SystemType.RegularAmbulance, SystemType.ICUAmbulance,
            SystemType.RegularAmbulance, SystemType.RegularAmbulance, SystemType.ICUAmbulance,
            SystemType.ICUAmbulance, SystemType.RegularAmbulance, SystemType.ICUAmbulance,
            SystemType.ICUAmbulance, SystemType.RegularAmbulance, SystemType.RegularAmbulance,
            SystemType.ICUAmbulance, SystemType.RegularAmbulance };

        string[] descriptions =
        { "Response to a road accident.","Assistance for a fainting incident.","Cardiac arrest case.","Helping a dehydrated individual.",
          "Child choking on food.","Severe allergic reaction.","Elderly person collapsed.","Labor contractions reported.","Sports injury during a game.",
          "Overdose situation.","Smoke inhalation victims.","Drowning rescue.","Fever and weakness in a patient.","Cyclist injured in an accident.",
          "Electric shock incident.","Severe bleeding from a knife accident.","Car crash involving multiple passengers.",
          "Fractured leg during an activity.","Burn injuries from a fire.","Diabetic patient with low blood sugar.","Asthma attack during an event.",
          "Teenager injured in an accident.","Suspected stroke reported.","Seizure episode.","Gunshot wound victim.",
          "Motorcyclist injured in a collision.","Pregnant woman experiencing complications.","Accidental poisoning in a child.","Severe headache and confusion in a patient.",
          "Bystander assistance for a fall.","Child injured on playground equipment.","Unconscious person found.","Severe burns from a chemical accident.",
          "Dog bite incident requiring medical attention.","Heatstroke case.","Rescue for a stranded motorist.","Broken arm during an activity.",
          "Choking incident.","Respiratory distress in a patient.","Pedestrian hit by a vehicle.","Fall from a ladder.","Electrocution from faulty wiring.",
          "Allergic reaction to an insect sting.","Gunfire victim.","Emergency childbirth.","Cyclist hit by a vehicle.","Fainting during an event.",
          "Hypothermia from exposure to cold.","Collapse due to dehydration.","Boat accident rescue."
        };

        for (int i = 0; i < 50; i++)
        {
            DateTime start = s_dalConfig.Clock.AddMinutes(-40); // 40 minutes before the current time
            int range = 30; // The maximum gap in minutes, here 30 minutes
            DateTime startTime = start.AddMinutes(s_rand.Next(range)); // The end time is random between 0 and 30 minutes after the start time
            DateTime endTime = startTime.AddMinutes(s_rand.Next(30));


            Call call = new(0,
                addresses[i],
                latitudes[i],
                longitudes[i],
                startTime,
                types[i],
                descriptions[i],
                endTime);
            s_dalCall?.Create(call);
        }

    }
    /// Create a list of assignments by randomly assigning volunteers to calls.
    private static void createAssignments()
    {
        List<Volunteer> volunteers = s_dalVolunteer?.ReadAll() ?? new List<Volunteer>();
        List<Call> calls = s_dalCall?.ReadAll() ?? new List<Call>();

        foreach (var call in calls)
        {
            // Randomly select a volunteer
            Volunteer volunteer = volunteers[s_rand.Next(volunteers.Count)];

            // Generate random start time for the assignment between the start of the call and 20 minutes after
            DateTime startTime = call.DateTime.AddMinutes(s_rand.Next(20));

            // Generate random end time for the assignment
            DateTime endTime = startTime.AddMinutes(s_rand.Next(20));
            EndStatus endStatus = EndStatus.Completed;

            if (endTime < call.EndDateTime)
            {
                endStatus = (EndStatus)s_rand.Next(1, 3); // Randomly choose between Completed, SelfCancelled and DirectorCancelled
            }
            else
                endStatus = EndStatus.Expired;

            // Create the assignment
            Assignment assignment = new(0, call.Id, volunteer.Id, startTime, endTime, endStatus);
            s_dalAssignement?.Create(assignment);

        }
    }

    public static void Do(IVolunteer? dalVolunteer, IAssignment? dalAssignment, ICall? dalCall, IConfig? dalConfig)
    {
        // Assign the provided DAL objects to the static fields, throwing an exception if any are null
        s_dalVolunteer = dalVolunteer ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalAssignement = dalAssignment ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalCall = dalCall ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalConfig = dalConfig ?? throw new NullReferenceException("DAL object can not be null!");

        // Reset configuration and clear all existing data
        Console.WriteLine("Reset Configuration values and List values...");
        s_dalConfig.Reset();
        s_dalVolunteer.DeleteAll();
        s_dalAssignement.DeleteAll();
        s_dalCall.DeleteAll();

        // Initialize the volunteers list
        Console.WriteLine("Initializing Volunteers list ...");
        createVolunteers();

        // Initialize the calls list
        Console.WriteLine("Initializing Calls list ...");
        createCalls();

        // Initialize the assignments list
        Console.WriteLine("Initializing Assignments list ...");
        createAssignments();
    }


}