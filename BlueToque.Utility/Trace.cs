using System;
using System.Diagnostics;

namespace BlueToque.Utility
{

    //
    // Summary:
    //     A Facade class to control trace This allows us to turn on or off trace for this
    //     module only, and to control what level of trace comes from this module (Info,
    //     Warning, Error, etc) For instance, to configure this class to show all Informaional
    //     messages do the following: Configure the source, and the switch that we will
    //     be using Also adds a listener
    internal static class Trace
    {
        static Trace() => TraceSourceManager.Instance.Add(s_traceSource);

        private static readonly TraceSource s_traceSource = new(typeof(Trace).Namespace!);

        public static CorrelationManager CorrelationManager => System.Diagnostics.Trace.CorrelationManager;

        [Conditional("TRACE")]
        public static void Assert(bool condition) => System.Diagnostics.Trace.Assert(condition);

        [Conditional("TRACE")]
        public static void Assert(bool condition, string message) => System.Diagnostics.Trace.Assert(condition, message);

        [Conditional("TRACE")]
        public static void TraceInformation(string str) => s_traceSource.TraceInformation(str);

        [Conditional("TRACE")]
        public static void TraceInformation(string format, params object[] args) => s_traceSource.TraceInformation(format, args);

        [Conditional("TRACE")]
        public static void TraceError(string format) => s_traceSource.TraceEvent(TraceEventType.Error, 0, format);

        [Conditional("TRACE")]
        public static void TraceError(string format, params object[] args) => s_traceSource.TraceEvent(TraceEventType.Error, 0, format, args);

        [Conditional("TRACE")]
        public static void TraceWarning(string format) => s_traceSource.TraceEvent(TraceEventType.Warning, 0, format);

        [Conditional("TRACE")]
        public static void TraceWarning(int id, string format) => s_traceSource.TraceEvent(TraceEventType.Warning, id, format);

        [Conditional("TRACE")]
        public static void TraceWarning(string format, params object[] args) => s_traceSource.TraceEvent(TraceEventType.Warning, 0, format, args);

        [Conditional("TRACE")]
        public static void TraceWarning(int id, string format, params object[] args) => s_traceSource.TraceEvent(TraceEventType.Warning, id, format, args);

        [Conditional("TRACE")]
        public static void TraceEvent(TraceEventType eventType, int id, string format) => s_traceSource.TraceEvent(eventType, id, format);

        [Conditional("TRACE")]
        public static void TraceEvent(TraceEventType eventType, int id, string format, params object[] args) => s_traceSource.TraceEvent(eventType, id, format, args);

        [Conditional("TRACE")]
        public static void TraceVerbose(string message) => s_traceSource.TraceEvent(TraceEventType.Verbose, 0, message);

        [Conditional("TRACE")]
        public static void TraceVerbose(string format, params object[] args) => s_traceSource.TraceEvent(TraceEventType.Verbose, 0, format, args);

        [Conditional("TRACE")]
        public static void WriteLine(string message) => s_traceSource.TraceEvent(TraceEventType.Verbose, 0, message);

        [Conditional("TRACE")]
        public static void WriteLine(string message, string format) => s_traceSource.TraceEvent(TraceEventType.Verbose, 0, message, format);

        public static void TraceTransfer(int id, string message, Guid relatedActivityId) => s_traceSource.TraceTransfer(id, message, relatedActivityId);
    }
}
