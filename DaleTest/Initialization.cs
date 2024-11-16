namespace DalTest;
using DalApi;
using DO;
using DalList;
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
    "12 Ben Yehuda Street, Jerusalem",
    "45 Hillel Street, Jerusalem",
    "67 Yafo Street, Jerusalem",
    "101 Herzl Street, Jerusalem",
    "23 King David Street, Jerusalem",
    "56 Agron Street, Jerusalem",
    "89 Shlomzion Hamalka Street, Jerusalem",
    "32 King George Street, Jerusalem",
    "78 Emek Refaim Street, Jerusalem",
    "5 Derech Hevron, Jerusalem",
    "14 Eliezer Kaplan Street, Jerusalem",
    "33 Shmuel Hanavi Street, Jerusalem",
    "8 Dov Hoz Street, Bnei Brak",
    "50 Keren Hayesod Street, Jerusalem",
    "63 Tchernichovsky Street, Jerusalem",
    "29 Menachem Begin Street, Jerusalem",
    "72 Malcha Street, Jerusalem",
    "101 Sderot Yerushalayim Street, Jerusalem",
    "120 Shalom Aleichem Street, Jerusalem",
    "57 Har Homa Street, Jerusalem"
};

        double[] latitudes = {
    31.7815951,
    31.7793341,
    31.783678,
    31.780965,
    31.7745543,
    31.7779632,
    31.779475,
    31.759882,
    31.782512 ,
    31.770842,
    35.2017399,
    31.7898752,
    31.7632657,
    31.777348,
    31.7943135,
    31.7856,
    31.7526,
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
    35.225733,
    35.2017399,
    35.2249504,
    34.837327,
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
        { "1 HaYarkon St, Tel Aviv",
        "2 Jaffa St, Jerusalem",
        "3 Ben Yehuda St, Haifa",
        "4 Herzl St, Rishon LeZion",
        "5 Rothschild Blvd, Tel Aviv",
        "6 King George St, Jerusalem",
        "7 Dizengoff St, Tel Aviv",
        "8 Allenby St, Tel Aviv",
        "9 Ibn Gabirol St, Tel Aviv",
        "10 Bograshov St, Tel Aviv",
        "11 Frishman St, Tel Aviv",
        "12 Nahalat Binyamin St, Tel Aviv",
        "13 Florentin St, Tel Aviv",
        "14 Montefiore St, Tel Aviv",
        "15 Shalom Aleichem St, Tel Aviv",
        "16 Bialik St, Tel Aviv",
        "17 Trumpeldor St, Tel Aviv",
        "18 Pinsker St, Tel Aviv",
        "19 Mapu St, Tel Aviv",
        "20 Shlomo Hamelech St, Tel Aviv",
        "21 Nordau Blvd, Tel Aviv",
        "22 Ben Gurion Blvd, Tel Aviv",
        "23 Yehoshua Bin Nun St, Tel Aviv",
        "24 Weizmann St, Tel Aviv",
        "25 Kaplan St, Tel Aviv",
        "26 Menachem Begin Rd, Tel Aviv",
        "27 HaYarkon St, Tel Aviv",
        "28 HaMasger St, Tel Aviv",
        "29 HaRakevet St, Tel Aviv",
        "30 HaShalom Rd, Tel Aviv",
        "31 HaArba'a St, Tel Aviv",
        "32 Yigal Alon St, Tel Aviv",
        "33 HaAliya St, Tel Aviv",
        "34 Levontin St, Tel Aviv",
        "35 Mikveh Israel St, Tel Aviv",
        "36 Yehuda Margoza St, Tel Aviv",
        "37 Shabazi St, Tel Aviv",
        "38 Abarbanel St, Tel Aviv",
        "39 Eilat St, Tel Aviv",
        "40 HaMered St, Tel Aviv",
        "41 Neve Tzedek St, Tel Aviv",
        "42 Shalma Rd, Tel Aviv",
        "43 Herzl St, Tel Aviv",
        "44 Lilienblum St, Tel Aviv",
        "45 HaYarkon St, Tel Aviv",
        "46 Jaffa St, Jerusalem",
        "47 Ben Yehuda St, Haifa",
        "48 Herzl St, Rishon LeZion",
        "49 Rothschild Blvd, Tel Aviv",
        "50 King George St, Jerusalem"
    };

        double[] latitudeArray = {
        32.080480, 31.783333, 32.815556, 31.964167, 32.065610, 31.783333, 32.075100, 32.073185, 32.080000,
        32.079000, 32.080000, 32.062000, 32.056000, 32.062000, 32.079000, 32.072000, 32.075000, 32.075000,
        32.080000, 32.080000, 32.090000, 32.085000, 32.085000, 32.085000, 32.070000, 32.070000, 32.080000,
        32.065000, 32.065000, 32.070000, 32.070000, 32.070000, 32.060000, 32.062000, 32.060000, 32.055000,
        32.062000, 32.056000, 32.056000, 32.063000, 32.061000, 32.056000, 32.055000, 32.062000, 32.080480,
        31.783333, 32.815556, 31.964167, 32.065610, 31.783333
    };

        double[] longitudeArray = {
        34.763660, 35.216667, 34.989167, 34.804167, 34.777819, 35.216667, 34.774800, 34.768134, 34.781000,
        34.768000, 34.769000, 34.770000, 34.767000, 34.774000, 34.768000, 34.769000, 34.768000, 34.770000,
        34.769000, 34.780000, 34.774000, 34.774000, 34.782000, 34.789000, 34.784000, 34.789000, 34.767000,
        34.782000, 34.780000, 34.792000, 34.784000, 34.791000, 34.772000, 34.774000, 34.770000, 34.756000,
        34.765000, 34.767000, 34.761000, 34.764000, 34.764000, 34.764000, 34.769000, 34.769000, 34.763660,
        35.216667, 34.989167, 34.804167, 34.777819, 35.216667
    };

        string[] mdaCallDescriptions =
{
    "Response to a road accident.",
    "Assistance for a fainting incident.",
    "Cardiac arrest case.",
    "Helping a dehydrated individual.",
    "Child choking on food.",
    "Severe allergic reaction.",
    "Elderly person collapsed.",
    "Labor contractions reported.",
    "Sports injury during a game.",
    "Overdose situation.",
    "Smoke inhalation victims.",
    "Drowning rescue.",
    "Fever and weakness in a patient.",
    "Cyclist injured in an accident.",
    "Electric shock incident.",
    "Severe bleeding from a knife accident.",
    "Car crash involving multiple passengers.",
    "Fractured leg during an activity.",
    "Burn injuries from a fire.",
    "Diabetic patient with low blood sugar.",
    "Asthma attack during an event.",
    "Teenager injured in an accident.",
    "Suspected stroke reported.",
    "Seizure episode.",
    "Gunshot wound victim.",
    "Motorcyclist injured in a collision.",
    "Pregnant woman experiencing complications.",
    "Accidental poisoning in a child.",
    "Severe headache and confusion in a patient.",
    "Bystander assistance for a fall.",
    "Child injured on playground equipment.",
    "Unconscious person found.",
    "Severe burns from a chemical accident.",
    "Dog bite incident requiring medical attention.",
    "Heatstroke case.",
    "Rescue for a stranded motorist.",
    "Broken arm during an activity.",
    "Choking incident.",
    "Respiratory distress in a patient.",
    "Pedestrian hit by a vehicle.",
    "Fall from a ladder.",
    "Electrocution from faulty wiring.",
    "Allergic reaction to an insect sting.",
    "Gunfire victim.",
    "Emergency childbirth.",
    "Cyclist hit by a vehicle.",
    "Fainting during an event.",
    "Hypothermia from exposure to cold.",
    "Collapse due to dehydration.",
    "Boat accident rescue."
};

        Array systemTypeValues = Enum.GetValues(typeof(SystemType));
        for (int i = 0; i < 50; i++)
        {
            Call call = new(0,
                addresses[i],
                latitudeArray[i],
                longitudeArray[i],
                (DateTime)GenerateRandomDateTime(0),
                (DO.SystemType)systemTypeValues.GetValue(s_rand.Next(systemTypeValues.Length)),
                mdaCallDescriptions[i],
                GenerateRandomDateTime(1));
            s_dalCall?.Create(call);
        }

        /*////j ai rajoute dans le enum les messimot pr mada 
nistration };


//for (int i = 0; i < addresses.Length; i++)
//{
//    Call call = new(i + 1, addresses[i], s_rand.NextDouble() * 90, s_rand.NextDouble() * 180, DateTime.Now, choices[i]);
//    if (s_dalCall?.Read(call.Id) == null)
//    {
//        s_dalCall?.Create(call);
//    }
//}
*/



    }

    /// <summary>
    /// Create a list of assignments by randomly assigning volunteers to calls.
    /// </summary>
    private static void createAssignments()
    {
        List<Volunteer> volunteers = s_dalVolunteer!.ReadAll().ToList();
        List<Call> calls = s_dalCall!.ReadAll().ToList();

        foreach (var volunteer in volunteers)
        {
            foreach (var call in calls)
            {
                // Generating random start and end times for the assignment
                DateTime begin = (DateTime)GenerateRandomDateTime(0);
                DateTime? end = GenerateRandomDateTime(1);

                // Create a new assignment object
                Assignment assignment = new(0, call.Id, volunteer.Id, begin, end);

                // Add the assignment to the data source
                s_dalAssignement?.Create(assignment);
            }
        }


    }
    /// <summary>
    /// the function generates a random DateTime either in the past or future relative to the current time, 
    /// depending on the option parameter. If option is 0, it generates a past DateTime.
    /// If option is not 0, it generates a future DateTime with a 20% chance of returning null.
    /// </summary>
    /// <param name="option">variable that either takes care of the opening time and maximum end time</param>
    /// <returns></returns>
    public static DateTime? GenerateRandomDateTime(int option = 0)
    {
        // Define the maximum range for earlier DateTime (e.g., up to 1 year ago)
        int daysAgo = s_rand.Next(1, 11); // Random number of days (1 to 10 days)
        int hoursAgo = s_rand.Next(0, 24); // Random number of hours
        int minutesAgo = s_rand.Next(0, 60); // Random number of minutes
        int secondsAgo = s_rand.Next(0, 60); // Random number of seconds
        DateTime? result = s_dalConfig?.Clock;
        DateTime randomDateTime = (DateTime)result;

        if (option == 0)
        {// Subtract the random time span from the current time
            randomDateTime.AddDays(-daysAgo)
                    .AddHours(-hoursAgo)
                    .AddMinutes(-minutesAgo)
                    .AddSeconds(-secondsAgo);
            result = randomDateTime;
        }
        else
        {// add the random time span from the current time
            //20 percent of the calls have no end date
            if (s_rand.Next(0, 5) == 0)
            {
                return null;
            }
            randomDateTime.AddDays(daysAgo)
                    .AddHours(hoursAgo)
                    .AddMinutes(minutesAgo)
                    .AddSeconds(secondsAgo);
            result = randomDateTime;

        }
        return result;
    }
}


