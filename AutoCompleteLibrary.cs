using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;

namespace AutoCompleteLibrary
{
    public class AutoComplete
    {
        public string GetEventHistory(string logPath)
        {
            StringBuilder history = new StringBuilder();
            try
            {
                EventLogQuery eventsQuery = new EventLogQuery(logPath, PathType.LogName);

                using (EventLogReader logReader = new EventLogReader(eventsQuery))
                {
                    EventRecord eventInstance;
                    while ((eventInstance = logReader.ReadEvent()) != null)
                    {
                        history.AppendLine($"Time Created: {eventInstance.TimeCreated}");
                        history.AppendLine($"Event ID: {eventInstance.Id}");
                        history.AppendLine($"Message: {eventInstance.FormatDescription()}");
                        history.AppendLine(new string('-', 50));
                    }
                }
            }
            catch (Exception ex)
            {
                history.AppendLine($"Error: {ex.Message}");
            }

            return history.ToString();
        }

        public List<string> GetEventLogPaths()
        {
            List<string> logPaths = new List<string>();

            try
            {
                foreach (var eventLog in EventLogSession.GlobalSession.GetLogNames())
                {
                    logPaths.Add(eventLog);
                }
            }
            catch (Exception ex)
            {
                logPaths.Add($"Error: {ex.Message}");
            }

            return logPaths;
        }

        public void Run(List<string> options)
        {
            StringBuilder input = new StringBuilder();
            ConsoleKeyInfo key;

            do
            {
                Console.Clear();
                var matches = options.Where(option => option.StartsWith(input.ToString(), StringComparison.OrdinalIgnoreCase)).ToList();

                if (matches.Count > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("Matches:");
                    foreach (var match in matches)
                    {
                        Console.WriteLine(match);
                    }
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Search: " + input.ToString());
                Console.ResetColor();

                key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Tab && matches.Count > 0)
                {
                    input.Clear();
                    input.Append(matches[0]);
                }
                else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input.Remove(input.Length - 1, 1);
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    DisplayAndConfigureLogProperties(input.ToString());
                    break;
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    input.Append(key.KeyChar);
                }

            } while (true);
        }

        private void DisplayAndConfigureLogProperties(string logPath)
        {
            try
            {
                EventLogConfiguration logConfig = new EventLogConfiguration(logPath);

                // Putting this here, because it's related to EventLogConfiguration() class
                // Configure log properties (example) - must run as admin for some if not all logs
                // logConfig.IsEnabled = true;
                // logConfig.LogMode = EventLogMode.Circular;

                // logConfig.SaveChanges();
                // Console.WriteLine("Log configuration updated successfully.");

                Console.WriteLine($"\nConfiguring the {logConfig.LogName} log.");
                Console.WriteLine($"Log path: {logConfig.LogFilePath}");
                Console.WriteLine($"Maximum log size (in bytes): {logConfig.MaximumSizeInBytes}");
                Console.WriteLine($"Is log enabled: {logConfig.IsEnabled}");
                Console.WriteLine($"Log mode: {logConfig.LogMode}");
                Console.WriteLine($"Provider names: {string.Join(", ", logConfig.ProviderNames)}");
                Console.WriteLine($"Security descriptor: {logConfig.SecurityDescriptor}");
                Console.WriteLine($"Isolation: {logConfig.LogIsolation}");
                Console.WriteLine($"Owning provider: {logConfig.OwningProviderName}");
                Console.WriteLine(new string('-', 50));

                EventLogInformation logInfo = EventLogSession.GlobalSession.GetLogInformation(logPath, PathType.LogName);
                Console.WriteLine($"Last write time: {logInfo.LastWriteTime}");
                Console.WriteLine($"Log file size (in bytes): {logInfo.FileSize}");
                Console.WriteLine($"Number of events: {logInfo.RecordCount}");
                Console.WriteLine($"Event History: {GetEventHistory(logPath)}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine("Error: You do not have permission to configure this log.");
                Console.WriteLine("Try running the application with administrator privileges.");
                Console.WriteLine($"Details: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accessing log '{logPath}': {ex.Message}");
            }
        }
    }
}
