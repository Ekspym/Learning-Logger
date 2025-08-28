
namespace Infrastructure.Core.Logging;

public static class MachineInfo
{
    public static string Name { get; private set; }
    public static string ApplicationName { get; private set; }
    public static Guid Guid { get; private set; }
    
    private static bool initialized { get; set; }
    private static object lockObject = new { };

    public static void InitInfo(string name,  string applicationName)
    {
        lock (lockObject)
        {            
            if (initialized)
            {
                return;
            }

            initialized = true;
        }

        Name = name;
        ApplicationName = applicationName;
    }
}