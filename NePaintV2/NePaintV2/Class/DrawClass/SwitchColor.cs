using System.Windows.Media;
using System.Windows.Shapes;

namespace NePaintV2.Class.DrawClass
{
    public class SwitchColor
    {
        public Color Switch(Rectangle selectedColorDisplay)
        {
            var colorDialog = new System.Windows.Forms.ColorDialog();

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var drawingColor = colorDialog.Color;
                var selectedColor = System.Windows.Media.Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B);
                selectedColorDisplay.Fill = new SolidColorBrush(selectedColor);
                return selectedColor;
            }
            return Colors.Transparent;
        }
    }
}
