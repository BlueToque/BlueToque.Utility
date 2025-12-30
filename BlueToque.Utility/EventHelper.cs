using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace BlueToque.Utility
{
    //
    // Summary:
    //     The event firing class contains methods and properties for safe cross-thread
    //     event firing
    public static class EventHelper
    {
        //
        // Summary:
        //     use the asynchronous cross thread nivokation, when cross thread invoke is necessary
        public static bool AsyncInvoke { get; set; }

        //
        // Summary:
        //     Retrieve the last exception handled by this class
        public static Exception? LastException { get; set; }

        //
        // Summary:
        //     Weather or not exceptions should eb thrown by this class, false by default
        public static bool ThrowExceptions { get; set; }

        //
        // Summary:
        //     The error event can be hooked to handle exceptions handled by this class
        public static event ThreadExceptionEventHandler? Error;

        static EventHelper()
        {
            AsyncInvoke = false;
            ThrowExceptions = true;
            LastException = null;
        }

        //
        // Summary:
        //     Call the error event
        //
        // Parameters:
        //   sender:
        //
        //   ex:
        private static void OnError(object sender, Exception ex) => EventHelper.Error?.Invoke(sender, new ThreadExceptionEventArgs(ex));

        //
        // Summary:
        //     Fire an event
        //
        // Parameters:
        //   eventToFire:
        //
        //   input:
        //
        //   sender:
        //
        // Type parameters:
        //   T:
        public static bool FireEvent<T>(Delegate eventToFire, T input, object sender) => FireEvent(eventToFire, input, sender, AsyncInvoke);

        //
        // Summary:
        //     Fires the specified event, and passes the input as a parameter.
        //
        // Parameters:
        //   eventToFire:
        //     The event to fire.
        //
        //   input:
        //     The input.
        //
        //   sender:
        //     The sender
        //
        //   asyncInvoke:
        //
        // Type parameters:
        //   T:
        //     Type of the input parameter.
        public static bool FireEvent<T>(Delegate? eventToFire, T? input, object? sender, bool asyncInvoke)
        {
            if (eventToFire is null)
                return false;

            bool result = false;
            Delegate[] invocationList = eventToFire.GetInvocationList();
            foreach (Delegate @delegate in invocationList)
            {
                try
                {
                    if (@delegate.Target is ISynchronizeInvoke synchronizeInvoke && synchronizeInvoke.InvokeRequired)
                    {
                        if (asyncInvoke)
                            synchronizeInvoke.BeginInvoke(eventToFire, [sender, input]);
                        else
                            synchronizeInvoke.Invoke(eventToFire, [sender, input]);
                    }
                    else
                    {
                        @delegate.DynamicInvoke(sender, input);
                    }
                }
                catch (Exception ex)
                {
                    result = true;
                    LastException = ex;
                    OnError(sender!, ex);
                    Trace.TraceError("EventHelper.FireEvent<>: Error firing event {0}:\r\n{1}", eventToFire.Method.Name, ex);
                    if (ThrowExceptions)
                        throw;
                }
            }

            return result;
        }

        //
        // Parameters:
        //   eventToFire:
        //
        //   input:
        //
        //   sender:
        //
        // Type parameters:
        //   T:
        public static bool QuickFire<T>(Delegate eventToFire, T input, object sender)
        {
            if (eventToFire is null)
                return false;
            try
            {
                eventToFire.DynamicInvoke(sender, input);
                return true;
            }
            catch (Exception ex)
            {
                Exception ex3 = (LastException = ex);
                OnError(sender, ex3);
                Trace.TraceError("EventHelper.QuickFire<T>: Error firing event {0}:\r\n{1}", eventToFire.Method.Name, ex3);
                if (ThrowExceptions)
                    throw;

                return false;
            }
        }

        //
        // Summary:
        //     Get all events from the object and from all base classes
        //
        // Parameters:
        //   type:
        public static IEnumerable<FieldInfo> GetAllEvents(Type? type)
        {
            if (type == null)
                return [];

            IEnumerable<FieldInfo> allEvents = GetAllEvents(type.BaseType);
            return allEvents.Union(from x in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                                   where type.GetEvent(x.Name) != null
                                   select x);
        }

        //
        // Summary:
        //     Unregister all of the events in an object
        //
        // Parameters:
        //   obj:
        //
        //   baseClasses:
        public static void UnregisterAll(object? obj, bool baseClasses)
        {
            if (obj == null)
                return;

            try
            {
                Type? type = obj.GetType();
                if (type == null)
                    return;

                IEnumerable<FieldInfo> enumerable = (baseClasses ? GetAllEvents(type) : (from x in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                                                                                         where type.GetEvent(x.Name) != null
                                                                                         select x));
                foreach (FieldInfo item in enumerable)
                {
                    EventInfo? @event = type.GetEvent(item.Name);
                    if (!(@event == null) && item.GetValue(obj) is MulticastDelegate multicastDelegate)
                    {
                        Delegate[] invocationList = multicastDelegate.GetInvocationList();
                        foreach (Delegate handler in invocationList)
                            @event.RemoveEventHandler(obj, handler);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("EventHelper.UnregisterAll: Error removing event handlers from object {0}:\r\n{1}", obj, ex);
            }
        }
    }
}