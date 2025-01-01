using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PL;


/// <summary>
/// For the filter of the volunteer list.
/// </summary>
internal class SystemTypeCollection : IEnumerable
{
    static readonly IEnumerable<BO.SystemType> s_enums = (Enum.GetValues(typeof(BO.SystemType)) as IEnumerable<BO.SystemType>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

/// <summary>
/// For the filter of the call list.
/// </summary>
internal class StatusCallCollection : IEnumerable
{
    static readonly IEnumerable<BO.Statuses> s_enums = (Enum.GetValues(typeof(BO.Statuses)) as IEnumerable<BO.Statuses>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class JobCollection : IEnumerable
{
    static readonly IEnumerable<BO.Job> s_enums = (Enum.GetValues(typeof(BO.Job)) as IEnumerable<BO.Job>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class DistanceTypeCollection : IEnumerable
{
    static readonly IEnumerable<BO.DistanceType> s_enums = (Enum.GetValues(typeof(BO.DistanceType)) as IEnumerable<BO.DistanceType>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class ClosedCallFieldCollection : IEnumerable
{
    static readonly IEnumerable<BO.ClosedCallInListField> s_enums = (Enum.GetValues(typeof(BO.ClosedCallInListField)) as IEnumerable<BO.ClosedCallInListField>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
