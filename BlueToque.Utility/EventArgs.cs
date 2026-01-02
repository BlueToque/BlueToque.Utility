using System;
using System.Diagnostics;

namespace BlueToque.Utility
{
    [method: DebuggerStepThrough]
    public abstract class EventArgs<T>(T value) : EventArgs
    {
        protected T Value { get; } = value;
    }
}
