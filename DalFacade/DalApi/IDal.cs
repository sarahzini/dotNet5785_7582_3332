﻿namespace DalApi;
public interface IDal
{
    IVolunteer Volunteer { get; }
    ICall Call { get; }
    IAssignment Assignment { get; }
    IConfig config { get; }
    void ResetDB();

}

