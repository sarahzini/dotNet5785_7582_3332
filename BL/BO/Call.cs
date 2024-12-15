﻿namespace BO;

public class Call
{
    public int CallId { get; init; }
    public SystemType TypeOfCall { get; set; }
    public string? Description { get; set; } = null;
    public string? CallAddress { get; set; } = null;
    public double? CallLatitude { get; set; } = null;
    public double? CallLongitude { get; set; } = null;
    public DateTime BeginTime { get; set; }
    public DateTime? MaxEndTime { get; set; } = null;
    public EndStatus ClosureType { get; set; }
    public List<BO.CallAssignInList>? CallAssigns { get; set; } = null;
    public override string ToString() => Helpers.Tools.ToStringProperty(this);
}
