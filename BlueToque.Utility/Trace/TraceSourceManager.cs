using System;
using System.Diagnostics;

namespace BlueToque.Utility
{
    /// <summary>
    /// A class to manage System.Diagnostics.TraceSource at runtime.
    /// </summary>
    public class TraceSourceManager
    {
        private TraceSourceManager() => m_Sources = [];

        private static TraceSourceManager? s_traceSources;

        /// <summary>
        /// Singleton
        /// </summary>
        public static TraceSourceManager Instance => s_traceSources ??= new TraceSourceManager();

        public event Action<TraceSource>? Added;

        public event Action<TraceSource>? Removed;

        /// <summary>
        /// Add a System.Diagnostics.TraceSource to the collection
        /// </summary>
        /// <param name="traceSource">System.Diagnostics.TraceSource</param>
        public void Add(TraceSource traceSource)
        {
            m_Sources.Add(traceSource);
            Added?.Invoke(traceSource);
        }

        /// <summary>
        /// Remove the TraceSource from the collection
        /// </summary>
        /// <param name="traceSource">System.Diagnostics.TraceSource</param>
        public void Remove(TraceSource traceSource)
        {
            m_Sources.Remove(traceSource);
            Removed?.Invoke(traceSource);
        }

        readonly TraceSourceCollection m_Sources;

    }
}
