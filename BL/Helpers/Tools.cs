using System;
using System.Net.Http;
using System.Text.Json;
using System.Reflection; // to use PropertyInfo
using System.Text; // to use StringBuilder


namespace Helpers;
internal static class Tools
{
    public static string ToStringProperty<T>(this T t)
    {
        if (t == null)
            return string.Empty;

        StringBuilder str = new StringBuilder();
        foreach (PropertyInfo item in typeof(T).GetProperties())
        {
            var value = item.GetValue(t, null);
            str.Append(item.Name + ": ");
            if (value is not string && value is IEnumerable<T> enumerable)
            {
                str.AppendLine();
                foreach (var it in enumerable)
                {
                    str.AppendLine(it.ToString());
                }
            }
            else
            {
                str.AppendLine(value?.ToString());
            }
        }
        return str.ToString();
    }

    
}
