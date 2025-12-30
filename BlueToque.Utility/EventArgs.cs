using System;
using System.Diagnostics;

namespace BlueToque.Utility
{
    public abstract class EventArgs<T> : EventArgs
    {
        protected T? Value { get; set; }

        internal T? InternalValue => Value;

        [DebuggerStepThrough]
        public EventArgs() { }

        [DebuggerStepThrough]
        public EventArgs(T value) => Value = value;
    }
}
