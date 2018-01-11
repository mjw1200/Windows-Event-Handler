using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Linq;

namespace Uptime
{
    internal class EventReader : IDisposable
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
                _eventLog = new EventLog(logName);
            }
            catch (Exception e)
            {
                _constructionException = e;
            }
        }

        /// <summary>
        /// Return the most recent event in <paramref name="events"/> based on their
        /// "TimeGenerated" member.
        /// </summary>
        public static EventLogEntry GetMostRecent(IList<EventLogEntry> events)
        {
            EventLogEntry mostRecent = null;

            foreach (var @event in events)
            {
                if (mostRecent == null || mostRecent.TimeGenerated < @event.TimeGenerated)
                    mostRecent = @event;
            }

            return mostRecent;
        }

        /// <summary>
        /// Return events with <paramref name="source"/> and <paramref name="instanceId"/> from
        /// the open event log. If <paramref name="instanceId"/> is not specified, all events
        /// with <paramref name="source"/> are returned. Will throw an exception if the event
        /// log could not be initialized properly.
        /// </summary>
        public IList<EventLogEntry> GetSpecificEvents(string source, long instanceId = 0)
        {
            if (_constructionException != null)
            {
                // Couldn't initialize the requested event log for whatever reason. Throw the
                // exception that the constructor caught.
                throw _constructionException;
            }

            return (from EventLogEntry entry in _eventLog.Entries where entry.Source.Equals(source) where instanceId == 0 || entry.InstanceId == instanceId select entry).ToList();
        }

        private Exception _constructionException;
        private EventLog _eventLog;

        #region IDisposable Support
        private bool _disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing)
                _eventLog.Dispose();

            _constructionException = null;
            _eventLog = null;

            _disposedValue = true;
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
