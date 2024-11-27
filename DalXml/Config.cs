namespace Dal
{
    static internal class Config
    {
        internal const string s_data_config_xml = "data-config.xml";
        internal const string s_calls_xml = "calls.xml";
        internal const string s_volunteers_xml = "volunteers.xml";
        internal const string s_assignments_xml = "assignments.xml";

        // Property for the next assignment ID
        internal static int NextAssignmentId
        {
            get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAssignmentId");
            set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", value);
        }

        // Property for the next call ID
        internal static int NextCallId
        {
            get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCallId");
            set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", value);
        }

        // Property for the system clock
        internal static DateTime Clock
        {
            get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
            set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
        }

        // Property for the risk range
        internal static TimeSpan RiskRange
        {
            get => TimeSpan.FromMinutes(XMLTools.GetConfigIntVal(s_data_config_xml, "RiskRange"));
            set => XMLTools.SetConfigIntVal(s_data_config_xml, "RiskRange", (int)value.TotalMinutes);
        }

        // Method to reset configuration values
        internal static void Reset()
        {
            NextAssignmentId = 1000;
            NextCallId = 100000000;
            Clock = DateTime.Now;
            RiskRange = TimeSpan.FromMinutes(30);
        }
    }
}
