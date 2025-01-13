namespace DalTest;
using System;
using DalApi;
using DO;
public static class Initialization
{
    // Using just one DAL object for all data sources.
    private static IDal? s_dal;
    private static readonly Random s_rand = new();

    /// <summary>
    /// The createVolunteers method initializes a list of Volunteer objects with predefined
    /// names, phone numbers, email addresses, and other details. It then creates
    /// the Volunteer objects using these values and adds them to the data source.
    /// </summary>

    private static void createVolunteers()
    {
        //all these arrays was written by AI (excepted Addresses/longitude/latitude)

        string[] names = { "Sarah Cohen", "Osher Mizrahi", "Yaara Levi", "Eli Ben-David", "Maya Shapiro", "Yair Katz",
            "Noa Peretz", "Aviad Cohen", "Tamar Israeli", "Lior Baruch", "Daniella Rosen", "Oren Goldstein",
            "Michal Avrahami", "Nadav Shulman", "Rachel Dubinsky", "Uri Dahan", "Yael Chaimovitz", "Ronit Gross",
            "Meir Ziv", "Tal Ben-Ari" };

        string[] phoneNumbers = { "052-3918274", "050-5639842", "054-1283795", "053-6472391", "058-9042318", "050-9834657",
            "052-7512386", "053-4326759", "054-1863420", "058-2917463", "050-3275641", "052-5068493", "053-7821594",
            "054-9342105", "058-6120394", "050-7461283", "052-9784530", "053-5289761", "054-3175928", "058-8456201" };

        string[] emails = { "Sarah.Cohen@gmail.com", "Osher.Mizrahi@yahoo.com", "Yaara.Levi@outlook.com", "Eli.BenDavid@mail.com",
            "Maya.Shapiro@icloud.com", "Yair.Katz@googlemail.com", "Noa.Peretz@hotmail.com", "Aviad.Cohen@live.com",
            "Tamar.Israeli@protonmail.com", "Lior.Baruch@aol.com", "Daniella.Rosen@zoho.com", "Oren.Goldstein@fastmail.com",
            "Michal.Avrahami@ymail.com", "Nadav.Shulman@hushmail.com", "Rachel.Dubinsky@msn.com", "Uri.Dahan@comcast.net",
            "Yael.Chaimovitz@outlook.co.il", "Ronit.Gross@tutanota.com", "Meir.Ziv@me.com", "Tal.BenAri@icloud.com" };

        string[] Passwords = {"Sarah1234", "Mizrahi789", "YL2023", "EliBD2021", "Shapiro44A", "YairKatz99",
            "NoaP321", "Aviad567", "Xx1Ab2Cd3", "Baruch2021", "Daniella123R", "OG9876",
            "MichalA1001", "Shulman5555", "Rachel78", "UD3456", "YaelCh10", "RonitG321",
            "MeirZ2022", "TalBen22A" };

        string[] Addresses = {"12 Ben Yehuda Street, Jerusalem, Israel", "45 Hillel Street, Jerusalem, Israel", "67 Yafo Street, Jerusalem, Israel",
                             "101 Herzl Street, Jerusalem, Israel", "23 King David Street, Jerusalem, Israel", "56 Agron Street, Jerusalem, Israel",
                             "89 Shlomzion Hamalka Street, Jerusalem, Israel", "32 King George Street, Jerusalem, Israel", "78 Emek Refaim Street, Jerusalem, Israel",
                             "5 Derech Hevron, Jerusalem, Israel", "14 Eliezer Kaplan Street, Jerusalem, Israel", "33 Shmuel Hanavi Street, Jerusalem, Israel",
                             "8 Dov Hoz Street, Jerusalem, Israel", "50 Keren Hayesod Street, Jerusalem, Israel", "63 Tchernichovsky Street, Jerusalem, Israel",
                             "29 Menachem Begin Street, Jerusalem, Israel", "72 Malcha Street, Jerusalem, Israel", "101 Sderot Yerushalayim Street, Jerusalem, Israel",
                             "120 Shalom Aleichem Street, Jerusalem, Israel", "57 Har Homa Street, Jerusalem, Israel"
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

        Job[] jobs = {Job.Volunteer, Job.Volunteer, Job.Manager, Job.Volunteer, Job.Volunteer,
            Job.Volunteer, Job.Volunteer, Job.Volunteer, Job.Volunteer, Job.Volunteer, Job.Volunteer,
            Job.Volunteer, Job.Volunteer, Job.Volunteer, Job.Volunteer, Job.Volunteer, Job.Volunteer,
            Job.Manager, Job.Volunteer, Job.Volunteer };

        bool[] actives = { true, true, false, false, true, true, true, true, true,
            true, true, true, true, true, true, true, true, true, true, true };

        DistanceType[] DistanceTypes = {DistanceType.WalkingDistance, DistanceType.WalkingDistance,
            DistanceType.AirDistance, DistanceType.AirDistance, DistanceType.AirDistance,
            DistanceType.DrivingDistance, DistanceType.WalkingDistance, DistanceType.AirDistance,
            DistanceType.AirDistance, DistanceType.AirDistance, DistanceType.WalkingDistance,
            DistanceType.DrivingDistance, DistanceType.AirDistance, DistanceType.WalkingDistance,
            DistanceType.AirDistance, DistanceType.DrivingDistance, DistanceType.AirDistance,
            DistanceType.AirDistance, DistanceType.DrivingDistance, DistanceType.AirDistance };

        for (int i = 0; i < names.Length; i++)
        {
            int id = s_rand.Next(10000000, 40000000); //generate a random id between 10000000 et 40000000
            if (s_dal!.Volunteer?.Read(id) == null)  //check if we can create a new volunteer
            {
                // Generate a random maximum distance for receiving a call.
                double maxDistance = s_rand.NextDouble() * 100; // Example: random distance between 0 and 100 km.

                // Create a new volunteer object and add it to the data source.
                Volunteer volunteer = new(id, names[i], phoneNumbers[i], emails[i], Passwords[i],
                    Addresses[i], latitudes[i], longitudes[i], jobs[i], actives[i], 
                    maxDistance, DistanceTypes[i]);
                s_dal!.Volunteer?.Create(volunteer);
            }
        }
    }

    /// <summary>
    /// The createCalls method initializes a list of Call objects with predefined 
    /// addresses, latitudes, longitudes, system types, and descriptions.
    /// It then generates random start and end times for each call and creates
    /// the Call objects using these values. Then, it adds the Call objects to the data source.
    /// </summary>
    private static void createCalls()
    {
        string[] addresses = {"10 yafo st, Jerusalem, Israel", "2 Jaffa St, Jerusalem, Israel", "3 Ben Yehuda St, Jerusalem, Israel", "4 Herzl St, Jerusalem, Israel",
                             "5 Azza st, Jerusalem, Israel", "6 King George St, Jerusalem, Israel", "50 King George St, Jerusalem, Israel", "12 King George St, Jerusalem, Israel",
                             "45 Jaffa St, Jerusalem, Israel", "78 Ben Yehuda St, Jerusalem, Israel", "23 Agron St, Jerusalem, Israel", "56 Hillel St, Jerusalem, Israel",
                             "89 Shlomzion Hamalka St, Jerusalem, Israel", "34 HaNevi'im St, Jerusalem, Israel", "67 Emek Refaim St, Jerusalem, Israel", "90 Derech Hebron St, Jerusalem, Israel",
                             "11 Keren Hayesod St, Jerusalem, Israel", "22 Ramban St, Jerusalem, Israel", "33 Gaza St, Jerusalem, Israel", "44 Azza St, Jerusalem, Israel",
                             "55 Bezalel St, Jerusalem, Israel", "66 Haneviim St, Jerusalem, Israel", "77 Shmuel Hanagid St, Jerusalem, Israel", "88 King David St, Jerusalem, Israel",
                             "99 Yaffo St, Jerusalem, Israel", "100 Agripas St, Jerusalem, Israel", "101 Haneviim St, Jerusalem, Israel", "102 Ben Maimon St, Jerusalem, Israel",
                             "103 Keren Kayemet St, Jerusalem, Israel", "104 Shatz St, Jerusalem, Israel", "105 Hillel St, Jerusalem, Israel", "106 Shlomzion Hamalka St, Jerusalem, Israel",
                             "107 King George St, Jerusalem, Israel", "108 Jaffa St, Jerusalem, Israel", "109 Ben Yehuda St, Jerusalem, Israel", "110 Agron St, Jerusalem, Israel",
                             "111 Haneviim St, Jerusalem, Israel", "112 Emek Refaim St, Jerusalem, Israel", "113 Derech Hebron St, Jerusalem, Israel", "114 Keren Hayesod St, Jerusalem, Israel",
                             "115 Ramban St, Jerusalem, Israel", "116 Azza St, Jerusalem, Israel", "117 Azza St, Jerusalem, Israel", "118 Bezalel St, Jerusalem, Israel",
                             "119 Haneviim St, Jerusalem, Israel", "120 Shmuel Hanagid St, Jerusalem, Israel", "121 King David St, Jerusalem, Israel", "122 Yaffo St, Jerusalem, Israel",
                             "123 Agripas St, Jerusalem, Israel", "124 Haneviim St, Jerusalem, Israel"};


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
            SystemType.RegularAmbulance, SystemType.ICUAmbulance, SystemType.ICUAmbulance,
            SystemType.RegularAmbulance, SystemType.ICUAmbulance, SystemType.ICUAmbulance,
            SystemType.ICUAmbulance, SystemType.RegularAmbulance, SystemType.ICUAmbulance,
            SystemType.ICUAmbulance, SystemType.RegularAmbulance, SystemType.RegularAmbulance,
            SystemType.ICUAmbulance, SystemType.RegularAmbulance, SystemType.RegularAmbulance };

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
          "Hypothermia from exposure to cold.","Collapse due to dehydration.","Boat accident rescue.","Child with a high fever."
        };

        //15 first calls in the last 5 hours
        for (int i = 0; i < 15; i++)
        {
            DateTime start = s_dal!.Config.Clock.AddMinutes(-300); // 300 minutes before the current time
            DateTime startTime = start.AddMinutes(s_rand.Next(30)); // The end time is random between 0 and 59 minutes after start
            DateTime? maxEnd;

            if (types[i] == SystemType.ICUAmbulance)
            {
                maxEnd = startTime.AddMinutes(s_rand.Next(10, 40));
            }
            else if (i % 5 == 0)// RegularAmbulance (sometimes no time limit)
            {
                maxEnd = null;
            }
            else
            {
                maxEnd = startTime.AddMinutes(s_rand.Next(10,60));

            }


            Call call = new(0,
                addresses[i],
                latitudes[i],
                longitudes[i],
                startTime,
                types[i],
                descriptions[i],
                maxEnd);
            s_dal!.Call?.Create(call);
        }

        //15 next calls in the last 4 hours

        for (int i = 15; i < 30; i++)
        {
            DateTime start = s_dal!.Config.Clock.AddMinutes(-240);
            DateTime startTime = start.AddMinutes(s_rand.Next(30)); // The end time is random between 0 and 59 minutes after start
            DateTime? maxEnd;

            if (types[i] == SystemType.ICUAmbulance)
            {
                maxEnd = startTime.AddMinutes(s_rand.Next(10,30));
            }
            else if (i % 5 == 0)// RegularAmbulance (sometimes no time limit)
            {
                maxEnd = null;
            }
            else
            {
                maxEnd = startTime.AddMinutes(s_rand.Next(10,60));

            }


            Call call = new(0,
                addresses[i],
                latitudes[i],
                longitudes[i],
                startTime,
                types[i],
                descriptions[i],
                maxEnd);
            s_dal!.Call?.Create(call);
        }

        //rest of the calls in the last 2 hours
        for (int i = 30; i < addresses.Length; i++)
        {
            DateTime start = s_dal!.Config.Clock.AddMinutes(-120);
            DateTime startTime = start.AddMinutes(s_rand.Next(120)); // The end time is random between 0 and 59 minutes after start
            DateTime? maxEnd;

            if (types[i] == SystemType.ICUAmbulance)
            {
                maxEnd = startTime.AddMinutes(s_rand.Next(40,300));
            }
            else if (i % 5 == 0)// RegularAmbulance (sometimes no time limit)
            {
                maxEnd = null;
            }
            else
            {
                maxEnd = startTime.AddMinutes(s_rand.Next(60,300));

            }


            Call call = new(0,
                addresses[i],
                latitudes[i],
                longitudes[i],
                startTime,
                types[i],
                descriptions[i],
                maxEnd);
            s_dal!.Call?.Create(call);
        }

       

    }

    /// <summary>
    /// The createAssignments method retrieves all volunteers and calls from the data source.
    /// It iterates through each call, randomly selects a volunteer, and generates random
    /// start and end times for the assignment. It determines the end status of the assignment
    /// based on whether the end time is before the call's end time. Finally, it creates an
    /// Assignment object with the generated details and adds it to the data source.
    /// </summary>

    private static void createAssignments()
    {
        //converting the IEnumerable to List
        List<Volunteer> volunteers = s_dal!.Volunteer?.ReadAll()?.ToList() ?? new List<Volunteer>();
        List<Call> calls = s_dal!.Call?.ReadAll()?.ToList() ?? new List<Call>();
        int i = 0;

        // Take the first 15 calls
        foreach (var call in calls.Take(30))
        {
            // Generate random start time for the assignment between the start of the call and 3 minutes after
            DateTime startTime = call.OpenTime.AddSeconds(s_rand.Next(120));
            DateTime? endTime;

            if (call.MaxEnd == null)
            {
                endTime = startTime.AddMinutes(s_rand.Next(30));
            }
            else { 
            // Calculate the total minutes between startTime and call.MaxEnd
            int totalMinutes = (int)(call.MaxEnd - startTime)?.TotalMinutes;

            // Generate random end time for the assignment
            endTime = startTime.AddMinutes(s_rand.Next(totalMinutes));
            }

            EndStatus? endStatus;

            if (i% 8 == 0&& call.MaxEnd!=null)
            {
                endTime = call.MaxEnd;
                endStatus = EndStatus.Expired;
            }
            else
            {
                endStatus = EndStatus.Completed; 
            }

            Volunteer volunteer = volunteers[i];

            // Create the assignment
            Assignment assignment = new(0, call.CallId, volunteer.VolunteerId, startTime, endTime, endStatus);
            s_dal!.Assignment?.Create(assignment);
            i++;
            if (i == 17)
                i = 0;
        }

        foreach (var call in calls.Skip(30).Take(10)) {

            // Generate random start time for the assignment between the start of the call and 3 minutes after
            DateTime startTime = call.OpenTime.AddSeconds(s_rand.Next(120));

            // Generate random end time for the assignment

            DateTime? endTime = startTime.AddMinutes(s_rand.Next(10));
            while (endTime >= call.MaxEnd)
                 endTime = startTime.AddMinutes(s_rand.Next(10));

            EndStatus? endStatus;
            endStatus = (EndStatus)s_rand.Next(2, 4); // Randomly choose between , SelfCancelled and ManagerCancelled (Not Completed!!!)
            

            Volunteer volunteer = volunteers[i];

            // Create the assignment
            Assignment assignment = new(0, call.CallId, volunteer.VolunteerId, startTime, endTime, endStatus);
            s_dal!.Assignment?.Create(assignment);
            i++;
            if (i == 17)
                i = 0;

        }

        //because the active volunteers is after 4 in the vec
        i = 4;

        //second assignment for those calls
        foreach (var call in calls.Skip(30).Take(10)) {
            // Generate random start time for the assignment between the start of the call and 3 minutes after
            DateTime startTime = call.OpenTime.AddMinutes(s_rand.Next(15));

            // Generate random end time for the assignment

            DateTime? endTime=null;

            EndStatus? endStatus=null;

            if ( s_dal.Config.Clock > call.MaxEnd)
            {
                endStatus = EndStatus.Expired;
                endTime = call.MaxEnd;
            }
            else if(i==7 && call.MaxEnd!=null)
            {
                // Calculate the total minutes between startTime and call.MaxEnd
                int totalMinutes = (int)(call.MaxEnd - startTime)?.TotalMinutes;

                // Generate random end time for the assignment
                endTime = startTime.AddMinutes(s_rand.Next(totalMinutes));
                endStatus = EndStatus.Completed;
            }


            Volunteer volunteer = volunteers[i];
            // Create the assignment
            Assignment assignment = new(0, call.CallId, volunteer.VolunteerId, startTime, endTime, endStatus);
            s_dal!.Assignment?.Create(assignment);

            i++;
        }

    }

    /// <summary>
    /// Initializes the data access layer (DAL) and populates the database with volunteers, calls, and assignments.
    /// </summary>
    /// <param name="dal">The data access layer object to be initialized.</param>
    /// <exception cref="NullReferenceException">Thrown when the provided DAL object is null.</exception>

    //public static void Do(IDal dal) //stage 2
    public static void Do()//stage 4
    {
        // Initialize the DAL objects
        //s_dal = dal ?? throw new NullReferenceException("DAL object can not be null!"); //stage 2
        s_dal = DalApi.Factory.Get; //stage 4

        Console.WriteLine("Reset Configuration values and List values...");
        s_dal.ResetDB();

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