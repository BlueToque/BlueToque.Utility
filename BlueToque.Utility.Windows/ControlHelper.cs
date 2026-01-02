using System;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.Control;


namespace BlueToque.Utility.Windows
{
    public static class ControlHelper
    {
        public static void Show(this DataGridViewColumn col) => col.Visible = true;

        public static void Hide(this DataGridViewColumn col) => col.Visible = false;

        public static DataGridViewColumn? Find(this DataGridViewColumnCollection collection, string text)
        {
            foreach (DataGridViewColumn col in collection)
                if (col.HeaderText == text)
                    return col;
            return null;
        }

        /// <summary>
        /// set all the controls to readonly
        /// </summary>
        /// <param name="controls"></param>
        /// <param name="val"></param>
        public static void SetReadOnly(this ControlCollection controls, bool val)
        {
            foreach (var control in controls)
            {
                if (control is TextBox tb)
                    tb.ReadOnly = val;

                (control as Control)?.Controls.SetReadOnly(val);
            }
        }

        public static void PaintIconColumn(this DataGridViewCellPaintingEventArgs e, Bitmap image)
        {
            if (e.Graphics == null) return;
            e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);
            DrawColumnIcon(e.Graphics, e.CellBounds, image);
        }

        public static void DrawColumnIcon(Graphics g, Rectangle bounds, Image image)
        {
            var size = image.Size;

            int px = (bounds.Width > size.Width) ? (bounds.Width - size.Width) / 2 : 0;
            int py = (bounds.Height > size.Height) ? (bounds.Height - size.Height) / 2 : 0;

            var width = Math.Min(bounds.Width, size.Width);
            var height = Math.Min(bounds.Height, size.Height);

            g.DrawImage(image, bounds.X + px, bounds.Y + py, width, height);
        }

        public static int TotalHeight(this DataGridView dgv, int maximum = 600, int minumum = 60)
        {
            int height = dgv.ColumnHeadersHeight;

            foreach (DataGridViewRow row in dgv.Rows)
                height += row.Height;

            height = Math.Max(height, minumum);

            return Math.Min(maximum, height);
        }
    }
}
