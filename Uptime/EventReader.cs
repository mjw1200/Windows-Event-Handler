using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace Uptime
{
    class EventReader : IDisposable
    {
        /// <summary>
        /// Initialize an <seealso cref="EventLog"/> object for for <paramref name="logName"/>. 
        /// If the event log cannot be created for any reason, the exception is saved and will
        /// be thrown if the EventReader is used.
        /// </summary>
        public EventReader(string logName)
        {
            try
            {
                eventLog = new EventLog(logName);
            }
            catch (Exception e)
            {
                constructionException = e;
            }
        }

        /// <summary>
        /// Return the most recent event in <paramref name="events"/> based on their
        /// "TimeGenerated" member.
        /// </summary>
        public static EventLogEntry GetMostRecent(IList<EventLogEntry> events)
        {
            EventLogEntry mostRecent = null;

            foreach (EventLogEntry @event in events)
            {
                if (mostRecent == null || mostRecent.TimeGenerated < @event.TimeGenerated)
                    mostRecent = @event;
            }

            return mostRecent;
        }

        /// <summary>
        /// Return events with <paramref name="source"/> and <paramref name="instanceID"/> from
        /// the open event log. If <paramref name="instanceID"/> is not specified, all events
        /// with <paramref name="source"/> are returned. Will throw an exception if the event
        /// log could not be initialized properly.
        /// </summary>
        public IList<EventLogEntry> GetSpecificEvents(string source, long instanceID = 0)
        {
            if (constructionException != null)
            {
                // Couldn't initialize the requested event log for whatever reason. Throw the
                // exception that the constructor caught.
                throw constructionException;
            }

            List<EventLogEntry> qualifyingEvents = new List<EventLogEntry>();

            foreach (EventLogEntry entry in eventLog.Entries)
            {
                if (entry.Source.Equals(source))
                {
                    if (instanceID == 0 || entry.InstanceId == instanceID)
                        qualifyingEvents.Add(entry);
                }
            }

            return qualifyingEvents;
        }

        Exception constructionException;
        EventLog eventLog;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    eventLog.Dispose();
                }

                constructionException = null;
                eventLog = null;

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
