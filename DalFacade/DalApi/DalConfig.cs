namespace DalApi;
using System.Xml.Linq;

/// <summary>
/// DalConfig class is responsible for loading and parsing the DAL configuration from an XML file.
/// </summary>
static class DalConfig
{
    /// <summary>
    /// Internal record to hold DAL implementation details.
    /// </summary>
    /// <param name="Package">The package/dll name.</param>
    /// <param name="Namespace">The namespace where the DAL implementation class is contained.</param>
    /// <param name="Class">The DAL implementation class name.</param>
    internal record DalImplementation
    (
        string Package,
        string Namespace,
        string Class
    );

    // The name of the DAL to be used, extracted from the configuration file.
    internal static string s_dalName;

    // Dictionary to hold the DAL packages and their corresponding implementation details.
    internal static Dictionary<string, DalImplementation> s_dalPackages;

    /// <summary>
    /// Static constructor to load and parse the dal-config.xml file.
    /// </summary>
    static DalConfig()
    {
        // Load the dal-config.xml file
        XElement dalConfig = XElement.Load(@"..\xml\dal-config.xml") ??
                             throw new DalConfigException("dal-config.xml file is not found");

        // Extract the DAL name from the configuration
        s_dalName =
           dalConfig.Element("dal")?.Value ?? throw new DalConfigException("<dal> element is missing");

        // Extract the DAL packages and their implementation details from the configuration
        var packages = dalConfig.Element("dal-packages")?.Elements() ??
                       throw new DalConfigException("<dal-packages> element is missing");
        s_dalPackages = (
                          from item in packages
                          let pkg = item.Value
                          let ns = item.Attribute("namespace")?.Value ?? "Dal"
                          let cls = item.Attribute("class")?.Value ?? pkg
                          select (item.Name, new DalImplementation(pkg, ns, cls))
                           ).ToDictionary(p => "" + p.Name, p => p.Item2);
    }
}

/// <summary>
/// Exception class for handling DAL configuration errors.
/// </summary>
[Serializable]
public class DalConfigException : Exception
{
    public DalConfigException(string msg) : base(msg) { }
    public DalConfigException(string msg, Exception ex) : base(msg, ex) { }
}

