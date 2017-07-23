using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace Uptime
{
    class Program
    {
        static void Main(string[] args)
        {            
            Stopwatch watch = new Stopwatch();
            bool statusGood = true;

            watch.Start();
            using (EventReader reader = new EventReader("System"))
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
                    EventLogEntry mostRecent = EventReader.GetMostRecent(eventList);

                    DateTime now = DateTime.Now;
                    Console.WriteLine("Last boot: " + mostRecent.TimeGenerated);
                    TimeSpan elapsed = now.Subtract(mostRecent.TimeGenerated);
                    Console.WriteLine("Uptime: " + elapsed.Days + " days, " + elapsed.Hours + " hours, " + elapsed.Minutes + " minutes");
                }
            }
            watch.Stop();

            Console.Write(Environment.NewLine + "Runtime: " + watch.Elapsed.Seconds + "." + watch.Elapsed.Milliseconds);
        }
    }
}
