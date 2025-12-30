using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;

namespace BlueToque.Utility
{

    //
    // Summary:
    //     A trace listener that you can use to subscribe to events from the Trace system.
    //     Basically, ad an event handlers to the static TraceMessage event, and all trace
    //     events will be sen to your event handler
    public class DebugTraceListener : TraceListener
    {
        private static readonly StringCollection m_buffer = [];

        //
        // Summary:
        //     The buffer that contains trace messages that have nt been output (messages will
        //     accumulate here untill an event is fired and empties the buffer)
        public static StringCollection Buffer => m_buffer;

        //
        // Summary:
        //     Handle this event to receive messages
        public static event EventHandler<TraceEventArgs>? TraceMessage;

        //
        // Summary:
        //     Default Constructor
        public DebugTraceListener() { }

        //
        // Summary:
        //     Write a message to the trace listener
        //
        // Parameters:
        //   message:
        public override void Write(string? message) => WriteBase(message);

        /// <summary>
        /// Write a line to the trace listener
        /// </summary>
        /// <param name="message"></param>
        public override void WriteLine(string? message) =>
            WriteBase(new StringBuilder(message)
                .Insert(0, " ", base.IndentSize * base.IndentLevel)
                .Append("\r\n").ToString());


        /// <summary>
        /// The base write method
        /// </summary>
        /// <param name="message"></param>
        public static void WriteBase(string? message)
        {
            if (message == null) return;
            if (TraceMessage == null && m_buffer != null)
            {
                if (m_buffer.Count < 10000)
                {
                    m_buffer.Add(message);
                    return;
                }

                m_buffer.Clear();
                //m_buffer = null;
                return;
            }

            if (m_buffer != null && m_buffer.Count != 0)
            {
                StringEnumerator enumerator = m_buffer.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                        EventHelper.FireEvent(TraceMessage, new TraceEventArgs(enumerator.Current), null, asyncInvoke: true);
                }
                finally
                {
                    if (enumerator is IDisposable disposable)
                        disposable.Dispose();
                }

                m_buffer.Clear();
            }

            EventHelper.FireEvent(TraceMessage, new TraceEventArgs(message), null, asyncInvoke: true);
        }

        //
        // Summary:
        //     Create a TraceListener, and add it to the collection
        public static void Start() => System.Diagnostics.Trace.Listeners.Add(new DebugTraceListener());
    }
}