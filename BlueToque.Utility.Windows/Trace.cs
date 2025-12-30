using System.Diagnostics;

namespace BlueToque.Utility.Windows
{
    /// <summary>
    /// A Facade class to control trace
    /// This allows us to turn on or off trace for this module only,
    /// and to control what level of trace comes from this module (Info, Warning, Error, etc)
    /// 
    /// For instance, to configure this class to show all Informaional messages do the following:
    /// 
    /// Configure the source, and the switch that we will be using
    /// Also adds a listener
    ///  <source name="BlueToque.Utility" switchName="SourceSwitch" switchType="System.Diagnostics.SourceSwitch" >
    ///    <listeners>
    ///      <add name="BriyanteConsole" />
    ///    </listeners>
    ///  </source>
    /// 
    ///  <switches>
    ///  <!-- You can set the level at which tracing is to occur or you can turn tracing off -->
    ///  <!--
    ///  These kind of events are published:
    ///    TraceEventType.Critical:    Critical error, probably misconfigured program or requires restart
    ///    TraceEventType.Error:       An error that the system can handle but needs to report
    ///    TraceEventType.Information: Informational message about processing 
    ///    TraceEventType.Verbose:     Messages about everything
    ///   TraceEventType.Warning:     Minor conditions that the system can handle
    ///    TraceEventType.Start, Stop, Suspend, Transfer, Resume: Activity tracing events
    ///
    ///  Set the following flag to one of these values to capture events:
    ///    ActivityTracing  Allows the Stop, Start, Suspend, Transfer, and Resume events through.  
    ///    All              Allows all events through.  
    ///    Critical         Allows only Critical events through.  
    ///    Error            Allows Critical and Error events through.  
    ///    Information      Allows Critical, Error, Warning, and Information events through.  
    ///    Off              Does not allow any events through.  
    ///    Verbose          Allows Critical, Error, Warning, Information, and Verbose events through.  
    ///    Warning          Allows Critical, Error, and Warning events through.  
    ///    
    ///      SourceLevel is set to 3 ("Error") by default
    ///  -->
    ///  <add name="SourceSwitch" value="Information" />
    ///</switches>
    /// 
    /// </summary>    
    internal static class Trace
    {
        static Trace() => TraceSourceManager.Instance.Add(s_traceSource);

        static readonly TraceSource s_traceSource = new(typeof(Trace).Namespace!);

        #region assert
        [Conditional("TRACE")]
        public static void Assert(bool condition) => System.Diagnostics.Trace.Assert(condition);

        [Conditional("TRACE")]
        public static void Assert(bool condition, string message) => System.Diagnostics.Trace.Assert(condition, message);
        #endregion

        public static CorrelationManager CorrelationManager => System.Diagnostics.Trace.CorrelationManager;

        #region information
        [Conditional("TRACE")]
        public static void TraceInformation(string str) => s_traceSource.TraceInformation(str);

        [Conditional("TRACE")]
        public static void TraceInformation(string format, params object[] args) => s_traceSource.TraceInformation(format, args);
        #endregion

        #region error
        [Conditional("TRACE")]
        public static void TraceError(string format) => s_traceSource.TraceEvent(TraceEventType.Error, 0, format);

        [Conditional("TRACE")]
        public static void TraceError(string format, params object[] args) => s_traceSource.TraceEvent(TraceEventType.Error, 0, format, args);
        #endregion

        #region warning
        [Conditional("TRACE")]
        public static void TraceWarning(string format) => s_traceSource.TraceEvent(TraceEventType.Warning, 0, format);

        [Conditional("TRACE")]
        public static void TraceWarning(int id, string format) => s_traceSource.TraceEvent(TraceEventType.Warning, id, format);

        [Conditional("TRACE")]
        public static void TraceWarning(string format, params object[] args) => s_traceSource.TraceEvent(TraceEventType.Warning, 0, format, args);

        [Conditional("TRACE")]
        public static void TraceWarning(int id, string format, params object[] args) => s_traceSource.TraceEvent(TraceEventType.Warning, id, format, args);
        #endregion

        #region trace event
        [Conditional("TRACE")]
        public static void TraceEvent(
            TraceEventType eventType,
            int id,
            string format
        ) => s_traceSource.TraceEvent(eventType, id, format);

        [Conditional("TRACE")]
        public static void TraceEvent(
            TraceEventType eventType,
            int id,
            string format,
            params object[] args
        ) => s_traceSource.TraceEvent(eventType, id, format, args);
        #endregion

        #region verbose
        [Conditional("TRACE")]
        public static void TraceVerbose(string message) => s_traceSource.TraceEvent(TraceEventType.Verbose, 0, message);

        [Conditional("TRACE")]
        public static void TraceVerbose(string format, params object[] args) => s_traceSource.TraceEvent(TraceEventType.Verbose, 0, format, args);
        #endregion

        #region writeline
        [Conditional("TRACE")]
        public static void WriteLine(string message) => s_traceSource.TraceEvent(TraceEventType.Verbose, 0, message);

        [Conditional("TRACE")]
        public static void WriteLine(string message, string format) => s_traceSource.TraceEvent(TraceEventType.Verbose, 0, message, format);
        #endregion
    }
}
