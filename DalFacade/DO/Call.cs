namespace DO;
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

    public Call() : this(0, "", 0, 0, DateTime.Now, SystemType.ICUAmbulance) { } //empty ctor for stage 3
}




