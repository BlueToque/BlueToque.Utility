using System;
namespace BlueToque.Utility
{
    //
    // Summary:
    //     Generic way to handle exceptions
    public static class ExceptionManager
    {
        //
        // Summary:
        //     Indicates if this class should re-throw exceptions
        public static bool Throw { get; set; }

        //
        // Summary:
        //     Logs the critical exception, and optionally re-throws it
        //
        // Parameters:
        //   ex:
        public static bool CriticalException(Exception ex)
        {
            Trace.TraceError(ex.ToString());
            if (Throw)
                throw ex;
            return false;
        }

        //
        // Summary:
        //     Logs the critical exception, and optionally re-throws it
        //
        // Parameters:
        //   formatString:
        //
        //   ex:
        public static bool CriticalException(string formatString, Exception ex)
        {
            Trace.TraceError(formatString, ex);
            if (Throw)
                throw ex;
            return false;
        }
    }
}
