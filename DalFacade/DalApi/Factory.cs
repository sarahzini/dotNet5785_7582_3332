namespace DalApi;

/// <summary>
/// Factory class to get the appropriate IDal implementation based on the configuration.
/// </summary>
public static class Factory
{
    /// <summary>
    /// Gets the IDal implementation based on the configuration settings.
    /// </summary>
    /// <exception cref="DalConfigException">Thrown when there is an issue with the configuration or loading the assembly.</exception>
    public static IDal Get
    {
        get
        {
            // Retrieve the DAL type from the configuration
            string dalType = DalApi.DalConfig.s_dalName ?? throw new DalConfigException($"DAL name is not extracted from the configuration");

            // Retrieve the DAL implementation details from the configuration
            DalApi.DalConfig.DalImplementation dal = DalApi.DalConfig.s_dalPackages[dalType] ??
                throw new DalConfigException($"Package for {dalType} is not found in packages list in dal-config.xml");

            // Attempt to load the assembly containing the DAL implementation
            try
            {
                System.Reflection.Assembly.Load(dal.Package ?? throw new DalConfigException($"Package {dal.Package} is null"));
            }
            catch (Exception ex)
            { throw new DalConfigException($"Failed to load {dal.Package}.dll package", ex); }

            // Get the type of the DAL implementation class
            Type type = Type.GetType($"{dal.Namespace}.{dal.Class}, {dal.Package}") ??
                throw new DalConfigException($"Class {dal.Namespace}.{dal.Class} was not found in {dal.Package}.dll");

            // Retrieve the singleton instance of the DAL implementation
            return type.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)?.GetValue(null) as IDal ??
                throw new DalConfigException($"Class {dal.Class} is not a singleton or wrong property name for Instance");
        }
    }
}

