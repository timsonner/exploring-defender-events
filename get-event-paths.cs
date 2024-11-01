using System;
using System.Diagnostics.Eventing.Reader;
using System.Text;

namespace SecurityTools
{
    public class DefenderHistory
    {
        public string GetEventLogPaths()
        {
            StringBuilder history = new StringBuilder();

            try
            {
                // Loop through all available event log paths
                foreach (var eventLog in EventLogSession.GlobalSession.GetLogNames())
                {
                    history.AppendLine(eventLog); // Append each log path to the StringBuilder
                }
            }
            catch (Exception ex)
            {
                history.AppendLine($"Error: {ex.Message}");
            }

            return history.ToString();
        }
    }
}
