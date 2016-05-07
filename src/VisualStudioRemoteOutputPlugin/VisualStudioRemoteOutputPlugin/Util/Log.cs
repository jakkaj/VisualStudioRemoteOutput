//https://github.com/mike-ward/VSColorOutput/blob/master/VSColorOutput/State/Log.cs

using System.Diagnostics;

namespace VisualStudioRemoteOutputPlugin.Util
{
    internal static class Log
    {
        internal static void LogError(string message)
        {
            try
            {
                // I'm co-opting the Visual Studio event source because I can't register
                // my own from a .VSIX installer.
                EventLog.WriteEntry("Microsoft Visual Studio",
                    "VSColorOutput: " + (message ?? "null"),
                    EventLogEntryType.Error);
            }
            catch
            {
                // Don't kill extension for logging errors
            }
        }
    }
}