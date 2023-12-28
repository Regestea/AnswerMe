using System.Drawing;
using System.Drawing.Imaging;

namespace ObjectStorage.Api.Test.DataGenerator
{
    public static class TextToImageStream
    {
        public static Stream ConvertTextToImageStream(string text)
        {
            // Create a Bitmap object with the desired width and height
            var width = 500;
            var height = 500;
            var bitmap = new Bitmap(width, height);

            // Create a Graphics object to draw on the bitmap
            var graphics = Graphics.FromImage(bitmap);

            // Set the background color of the bitmap to white
            graphics.Clear(Color.White);

            // Draw the text on the bitmap
            var font = new Font("Arial", 20);
            Brush brush = new SolidBrush(Color.Black);
            graphics.DrawString(text, font, brush, new PointF(20, 20));

            // Save the bitmap as a PNG image to a memory stream
            MemoryStream memoryStream = new();
            bitmap.Save(memoryStream, ImageFormat.Png);

            // Reset the position of the memory stream to the beginning
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Create a read stream from the memory stream
            Stream readStream = new MemoryStream(memoryStream.ToArray());

            // Return the read stream
            return readStream;
        }
    }
}
