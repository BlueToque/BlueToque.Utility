using BlueToque.Serialization;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BlueToque.Utility.Windows
{
    partial class FormSettingsType : Serializable<FormSettingsType>
    {
        /// <summary>
        /// Ensure a form is visible on the screen
        /// </summary>
        /// <param name="form"></param>
        public static void EnsureVisible(Form form)
        {
            Rectangle bounds = Screen.GetWorkingArea(form);
            if (form.Top < bounds.Top) form.Top = bounds.Top;
            if ((form.Top + 40) > bounds.Bottom) form.Top = bounds.Bottom - 40;
            if ((form.Right - 80) < bounds.Left) form.Left = (bounds.Left - form.Width) + 80;
            if ((form.Left + 60) > bounds.Right) form.Left = bounds.Right - 60;
        }

        /// <summary>
        /// Load the form's placement settings from the given file
        /// </summary>
        /// <param name="form"></param>
        /// <param name="fileName"></param>
        public static bool LoadSettings(Form form, string fileName)
        {
            try
            {
                fileName = Paths.Expand(fileName);
                if (!File.Exists(fileName))
                {
                    Trace.TraceError("PersistWindowState.LoadSettings: file does not exist, using default window placement\r\n\"{0}\"", fileName);
                    form.WindowState = FormWindowState.Maximized;
                    return false;
                }

                FormSettingsType? settings = FromXmlFile(fileName);
                if (settings == null)
                    return false;

                form.WindowState = (FormWindowState)settings.State;
                form.Location = new Point(settings.LocationX, settings.LocationY);
                form.Size = new Size(settings.SizeWidth, settings.SizeHeight);

                EnsureVisible(form);
                return true;
            }
            catch (Exception ex)
            {
                form.WindowState = FormWindowState.Maximized;
                Trace.TraceError("WindowState.LoadSettings: error loading form settings:\r\n{0}", ex);
                return false;
            }
        }

        /// <summary>
        /// Save a form's window placement to the given file
        /// </summary>
        /// <param name="form"></param>
        /// <param name="fileName"></param>
        public static void SaveSettings(Form form, string fileName)
        {
            fileName = Paths.Expand(fileName);
            try
            {
                FormSettingsType settings = new()
                {
                    State = (int)form.WindowState,
                    LocationX = form.Location.X,
                    LocationY = form.Location.Y,
                    SizeWidth = form.Size.Width,
                    SizeHeight = form.Size.Height
                };

                settings.ToXmlFile(fileName);
            }
            catch (Exception ex)
            {
                Trace.TraceError("WindowState.SaveSettings: Error saving form settings:\r\n{0}", [ex]);
            }
        }
    }
}
