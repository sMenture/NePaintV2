using System.Windows.Media.Imaging;

namespace NePaintV2.Class.DrawClass
{
    internal class SaveLogic
    {
        public void Save(DrawingCanvas drawing)
        {
            var saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.Filter = "PNG Files (*.png)|*.png";
            saveFileDialog.DefaultExt = "png";

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (var fileStream = new System.IO.FileStream(saveFileDialog.FileName, System.IO.FileMode.Create))
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(drawing.Bitmap));
                    encoder.Save(fileStream);
                }
            }
        }
    }
}
