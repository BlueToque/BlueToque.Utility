using System;

namespace BlueToque.Utility
{
    //
    // Summary:
    //     Event args that contain a string message
    //[Serializable]
    public class TraceEventArgs : EventArgs
    {
        //
        // Summary:
        //     The string message
        public string? Message { get; set; }

        //
        // Summary:
        //     Default constructor (for selialization
        public TraceEventArgs() { }

        //
        // Summary:
        //     constructor
        //
        // Parameters:
        //   message:
        public TraceEventArgs(string? message) => Message = message;
    }
}
