namespace DO;
/// <summary>
/// 
/// </summary>
/// <param name="Id"></param>
/// <param name="CallId"></param>
/// <param name="VolunteerId"></param>
/// <param name="Begin"></param>
/// <param name="End"></param>
/// <param name="MyEndStatus"></param>
public record Assignment
(
    int Id,
    int CallId,
    int VolunteerId,
    DateTime Begin,
    DateTime? End=null,
    EndStatus MyEndStatus= EndStatus.Completed

)
{
    public Assignment() : this(0, 0, 0, DateTime.Now) { } //empty ctor for stage 3
}
