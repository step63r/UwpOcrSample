using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace UwpOcrSample.Models
{
    /// <summary>
    /// 
    /// </summary>
    internal class WindowsOcr
    {
        #region プロパティ
        /// <summary>
        /// OCR結果
        /// </summary>
        public OcrResult OcrResult { get; set; }
        /// <summary>
        /// OCR画像
        /// </summary>
        public SoftwareBitmap OcrImage { get; set; }
        /// <summary>
        /// 検出した行を囲む四角形
        /// </summary>
        public List<Rect> DetectRects { get; set; }
        #endregion

        #region メンバ変数
        private readonly OcrEngine _engine = OcrEngine.TryCreateFromUserProfileLanguages();
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task ExecuteOcr()
        {
            OcrResult = await _engine.RecognizeAsync(OcrImage);

            if (OcrResult != null)
            {
                DetectRects = GetDetectRects();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private List<Rect> GetDetectRects()
        {
            var rects = new List<Rect>();

            // X座標, Y座標の最大値と最小値を求める
            foreach (var line in OcrResult.Lines)
            {
                //foreach (var word in line.Words)
                //{
                //    Debug.WriteLine($"{word.Text}: ({word.BoundingRect.X}, {word.BoundingRect.Y}), ({word.BoundingRect.Top}, {word.BoundingRect.Height}), ({word.BoundingRect.Width}, {word.BoundingRect.Height})");
                //}
                double minX = line.Words.Min(item => item.BoundingRect.X);
                double maxX = line.Words.Max(item => item.BoundingRect.X + item.BoundingRect.Width);

                double minY = line.Words.Min(item => item.BoundingRect.Y);
                double maxY = line.Words.Max(item => item.BoundingRect.Y + item.BoundingRect.Height);

                rects.Add(new Rect(minX, minY, maxX, maxY));
            }

            return rects;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private async Task<SoftwareBitmap> LoadImage(string path)
        {
            using (var fs = File.OpenRead(path))
            {
                var buf = new byte[fs.Length];
                fs.Read(buf, 0, (int)fs.Length);
                using (var mem = new MemoryStream(buf))
                {
                    mem.Position = 0;

                    var stream = await ConvertToRandomAccessStream(mem);
                    var bitmap = await LoadImage(stream);
                    return bitmap;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private async Task<SoftwareBitmap> LoadImage(IRandomAccessStream stream)
        {
            var decoder = await BitmapDecoder.CreateAsync(stream);
            var bitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            return bitmap;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memoryStream"></param>
        /// <returns></returns>
        private async Task<IRandomAccessStream> ConvertToRandomAccessStream(MemoryStream memoryStream)
        {
            using (var randomAccessStream = new InMemoryRandomAccessStream())
            {
                using (var outputStream = randomAccessStream.GetOutputStreamAt(0))
                {
                    var dw = new DataWriter(outputStream);
                    var task = new Task(() => dw.WriteBytes(memoryStream.ToArray()));
                    task.Start();
                    await task;
                    await dw.StoreAsync();
                    await outputStream.FlushAsync();
                }

                return randomAccessStream;
            }
        }
    }
}
