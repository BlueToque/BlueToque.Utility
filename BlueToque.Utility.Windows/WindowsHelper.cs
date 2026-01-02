using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace BlueToque.Utility.Windows;

public static class WindowsHelpers
{
    /// <summary>
    /// Invoke on the UI Thread if required
    /// If the UI is disposed, return without invoking
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="control">the control or form to invoke on</param>
    /// <param name="action">the action to invoke</param>
    /// <param name="sync">synchronous or asyncronous</param>
    [System.Diagnostics.DebuggerStepThrough]
    public static void InvokeIfRequired<T>(this T control, Action<T> action, bool sync = false) where T : ISynchronizeInvoke
    {
        if (control == null) return;

        if (control is Control l_control && l_control.IsDisposed) 
            return;

        if (control.InvokeRequired)
        {
            if (sync)
                control.Invoke(action, [control]);
            else
                control.BeginInvoke(action, [control]);
        }
        else
            action(control);
    }

}
