
namespace DO;
/// <summary>
/// 
/// </summary>
/// <param name="Id"></param>
/// <param name="Address"></param>
/// <param name="Latitude"></param>
/// <param name="Longitude"></param>
/// <param name="DateTime"></param>
/// <param name="Choice"></param>
/// <param name="Description"></param>
/// <param name="EndDateTime"></param>

public record Call
(
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




