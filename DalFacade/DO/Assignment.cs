namespace DO;
public record Assignment
(
    //The assignment properties
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
