using System.Drawing;
using System.Windows.Forms;

namespace BlueToque.Utility.Windows
{
    public static class FormHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="childForm"></param>
        /// <param name="parentForm"></param>
        /// <returns></returns>
        public static Rectangle CenterOnParent(this Form childForm, Form? parentForm)
        {
            if (parentForm == null)
                return Rectangle.Empty;

            Point center = new(
                parentForm.Location.X + (parentForm.Width / 2),
                parentForm.Location.Y + (parentForm.Height / 2));

            Point location = new(center.X - (childForm.Width / 2), center.Y - (childForm.Height / 2));
            return new Rectangle(location, childForm.Size);
        }
    }
}
