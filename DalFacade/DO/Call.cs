namespace DO;

/// <summary>
/// Represents a call with various properties such as ID, address, location details, and timestamps.
/// </summary>
/// <param name="Id">Represents a unique identifier for the call, obtained from the configuration entity.</param>
/// <param name="Address">The full and valid address of the call location.</param>
/// <param name="Latitude">Latitude coordinate, updated by the logic layer when the address is updated.</param>
/// <param name="Longitude">Longitude coordinate, updated by the logic layer when the address is updated.</param>
/// <param name="DateTime">The date and time when the call was opened by the manager.</param>
/// <param name="Choice">The type of call based on the specific system (e.g., ICUAmbulance, RegularAmbulance).</param>
/// <param name="Description">A textual description providing additional details about the call.</param>
/// <param name="EndDateTime">The maximum date and time by which the call should be closed, can be null indicating no time limit.</param>
public record Call
(
  //The Call properties
  int Id,
  string Address,
  double Latitude,
  double Longitude,
  DateTime DateTime,
  SystemType Choice,
  string? Description = null,
  DateTime? EndDateTime = null

)
{
    public override string ToString()
    {
        return $"Id: {Id}, Address: {Address}, DateTime: {DateTime}, ICU or Regular: {Choice}, Description: {Description}, EndDateTime: {EndDateTime}";
    }
    public Call() : this(0, "", 0, 0, DateTime.Now, SystemType.ICUAmbulance) { } //empty ctor for stage 3
}




