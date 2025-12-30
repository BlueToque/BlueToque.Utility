using System;
using System.Diagnostics;

namespace BlueToque.Utility
{
    /// <summary>
    /// Activity tracing
    /// http://www.codeproject.com/Articles/185666/ActivityTracerScope-Part-I-Easy-activity-tracing-w
    /// </summary>
    public sealed class ActivityTracerScope : IDisposable
    {
        private readonly Guid m_oldActivityId;
        private readonly Guid m_newActivityId;
        private readonly string m_activityName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityName"></param>
        public ActivityTracerScope(string activityName)
        {
            System.Diagnostics.Trace.Indent();
            m_oldActivityId = Trace.CorrelationManager.ActivityId;
            m_activityName = activityName;
            m_newActivityId = Guid.NewGuid();

            if (m_oldActivityId != Guid.Empty)
                Trace.TraceTransfer(0, "Transferring to new activity...", m_newActivityId);

            Trace.CorrelationManager.ActivityId = m_newActivityId;
            Trace.TraceEvent(TraceEventType.Start, 0, activityName);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (m_oldActivityId != Guid.Empty)
                Trace.TraceTransfer(0, "Transferring back to old activity...", m_oldActivityId);

            Trace.TraceEvent(TraceEventType.Stop, 0, m_activityName);
            Trace.CorrelationManager.ActivityId = m_oldActivityId;
            System.Diagnostics.Trace.Unindent();
        }
    }

}
