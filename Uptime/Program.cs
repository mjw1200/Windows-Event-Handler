using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace Uptime
{
    internal class Program
    {
        private static void Main()
        {            
            var watch = new Stopwatch();
            var statusGood = true;

            watch.Start();
            using (var reader = new EventReader("System"))
            {
                IList<EventLogEntry> eventList = null;

                try
                {
                    eventList = reader.GetSpecificEvents("Microsoft-Windows-Kernel-General", 12);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to initialize event log: " + e.Message);
                    statusGood = false;
                }

                if (statusGood)
                {
                    var mostRecent = EventReader.GetMostRecent(eventList);

                    var now = DateTime.Now;
                    Console.WriteLine("Last boot: " + mostRecent.TimeGenerated);
                    var elapsed = now.Subtract(mostRecent.TimeGenerated);
                    Console.WriteLine("Uptime: " + elapsed.Days + " days, " + elapsed.Hours + " hours, " + elapsed.Minutes + " minutes");
                }
            }
            watch.Stop();

            Console.Write(Environment.NewLine + "Runtime: " + watch.Elapsed.Seconds + "." + watch.Elapsed.Milliseconds);
        }
    }
}
