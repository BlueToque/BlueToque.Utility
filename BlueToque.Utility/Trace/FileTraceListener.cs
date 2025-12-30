using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace BlueToque.Utility
{

    //
    // Summary:
    //     A trace listener that you can use to subscribe to events from the Trace system.
    //     Basically, ad an event handlers to the static TraceMessage event, and all trace
    //     events will be sen to your event handler
    public class FileTraceListener : TraceListener
    {
        public static string? FileName { get; set; }

        internal TextWriter? m_writer;

        public string? m_fileName;

        /// <devdoc>
        /// <para>Initializes a new instance of the <see cref='BlueToque.Utility.FileTraceListener'/> class with
        /// <see cref='System.IO.TextWriter'/>
        /// as the output recipient.</para>
        /// </devdoc>
        public FileTraceListener() { }

        /// <devdoc>
        /// <para>Initializes a new instance of the <see cref='BlueToque.Utility.FileTraceListener'/> class, using the
        ///    stream as the recipient of the debugging and tracing output.</para>
        /// </devdoc>
        public FileTraceListener(Stream stream) : this(stream, string.Empty) { }

        /// <devdoc>
        /// <para>Initializes a new instance of the <see cref='BlueToque.Utility.FileTraceListener'/> class with the
        ///    specified name and using the stream as the recipient of the debugging and tracing output.</para>
        /// </devdoc>
        public FileTraceListener(Stream stream, string? name) : base(name)
        {
            ArgumentNullException.ThrowIfNull(stream);
            m_writer = new StreamWriter(stream);
        }

        /// <devdoc>
        /// <para>Initializes a new instance of the <see cref='BlueToque.Utility.FileTraceListener'/> class using the
        ///    specified writer as recipient of the tracing or debugging output.</para>
        /// </devdoc>
        public FileTraceListener(TextWriter writer) : this(writer, string.Empty) { }

        /// <devdoc>
        /// <para>Initializes a new instance of the <see cref='BlueToque.Utility.FileTraceListener'/> class with the
        ///    specified name and using the specified writer as recipient of the tracing or
        ///    debugging
        ///    output.</para>
        /// </devdoc>
        public FileTraceListener(TextWriter writer, string? name) : base(name)
        {
            ArgumentNullException.ThrowIfNull(writer);
            m_writer = writer;
        }

        /// <devdoc>
        ///    <para>Initializes a new instance of the <see cref='BlueToque.Utility.FileTraceListener'/> class with the
        ///    specified file name.</para>
        /// </devdoc>
        public FileTraceListener(string? fileName) => m_fileName = fileName;

        /// <devdoc>
        ///    <para>Initializes a new instance of the <see cref='BlueToque.Utility.FileTraceListener'/> class with the
        ///    specified name and the specified file name.</para>
        /// </devdoc>
        public FileTraceListener(string? fileName, string? name) : base(name) => m_fileName = fileName;

        /// <devdoc>
        ///    <para> Indicates the text writer that receives the tracing
        ///       or debugging output.</para>
        /// </devdoc>
        public TextWriter? Writer
        {
            get
            {
                EnsureWriter();
                return m_writer;
            }

            set
            {
                m_writer = value;
            }
        }

        /// <devdoc>
        /// <para>Closes the <see cref='BlueToque.Utility.FileTraceListener.Writer'/> so that it no longer
        ///    receives tracing or debugging output.</para>
        /// </devdoc>
        public override void Close()
        {
            if (m_writer != null)
            {
                try
                {
                    m_writer.Close();
                }
                catch (ObjectDisposedException) { }
                m_writer = null;
            }

            // We need to set the _fileName to null so that we stop tracing output, if we don't set it
            // EnsureWriter will create the stream writer again if someone writes or traces output after closing.
            m_fileName = null;
        }

        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && m_writer != null)
                    m_writer.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <devdoc>
        /// <para>Flushes the output buffer for the <see cref='BlueToque.Utility.FileTraceListener.Writer'/>.</para>
        /// </devdoc>
        public override void Flush()
        {
            EnsureWriter();
            try
            {
                m_writer?.Flush();
            }
            catch (ObjectDisposedException) { }
        }

        /// <devdoc>
        ///    <para>Writes a message
        ///       to this instance's <see cref='BlueToque.Utility.FileTraceListener.Writer'/>.</para>
        /// </devdoc>
        public override void Write(string? message)
        {
            EnsureWriter();
            if (m_writer != null)
            {
                if (NeedIndent) WriteIndent();
                try
                {
                    m_writer.Write(message);
                }
                catch (ObjectDisposedException) { }
            }
        }

        /// <devdoc>
        ///    <para>Writes a message
        ///       to this instance's <see cref='BlueToque.Utility.FileTraceListener.Writer'/> followed by a line terminator. The
        ///       default line terminator is a carriage return followed by a line feed (\r\n).</para>
        /// </devdoc>
        public override void WriteLine(string? message)
        {
            EnsureWriter();
            if (m_writer != null)
            {
                if (NeedIndent) WriteIndent();
                try
                {
                    m_writer.WriteLine(message);
                    NeedIndent = true;
                }
                catch (ObjectDisposedException) { }
            }
        }

        internal void EnsureWriter()
        {
            if (m_writer == null)
                InitializeWriter();

            void InitializeWriter()
            {
                bool success = false;

                m_fileName ??= FileName;
                if (m_fileName == null)
                    return;

                // StreamWriter by default uses UTF8Encoding which will throw on invalid encoding errors.
                // This can cause the internal StreamWriter's state to be irrecoverable. It is bad for tracing
                // APIs to throw on encoding errors. Instead, we should provide a "?" replacement fallback
                // encoding to substitute illegal chars. For ex, In case of high surrogate character
                // D800-DBFF without a following low surrogate character DC00-DFFF
                // NOTE: We also need to use an encoding that does't emit BOM which is StreamWriter's default
                var noBOMwithFallback = (UTF8Encoding)new UTF8Encoding(false).Clone();
                noBOMwithFallback.EncoderFallback = EncoderFallback.ReplacementFallback;
                noBOMwithFallback.DecoderFallback = DecoderFallback.ReplacementFallback;

                // To support multiple appdomains/instances tracing to the same file,
                // we will try to open the given file for append but if we encounter
                // IO errors, we will prefix the file name with a unique GUID value
                // and try one more time
                m_fileName = Paths.Expand(m_fileName);
                string fullPath = Path.GetFullPath(m_fileName);
                string dirPath = Path.GetDirectoryName(fullPath)!;
                string fileNameOnly = Path.GetFileName(fullPath);

                for (int i = 0; i < 2; i++)
                {
                    try
                    {
                        m_writer = new StreamWriter(fullPath, true, noBOMwithFallback, 4096);
                        success = true;
                        break;
                    }
                    catch (IOException)
                    {
                        fileNameOnly = $"{Guid.NewGuid()}{fileNameOnly}";
                        fullPath = Path.Combine(dirPath, fileNameOnly);
                        continue;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        //ERROR_ACCESS_DENIED, mostly ACL issues
                        break;
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }

                if (!success)
                {
                    // Disable tracing to this listener. Every Write will be nop.
                    // We need to think of a central way to deal with the listener
                    // init errors in the future. The default should be that we eat
                    // up any errors from listener and optionally notify the user
                    m_fileName = null;
                }
            }
        }

        internal bool IsEnabled(TraceOptions opts) => (opts & TraceOutputOptions) != 0;

        //
        // Summary:
        //     Create a TraceListener, and add it to the collection
        public static void Start() => System.Diagnostics.Trace.Listeners.Add(new FileTraceListener());

        public static void Start(string fileName, string name) => System.Diagnostics.Trace.Listeners.Add(new FileTraceListener(fileName, name));
    }
}