using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UwpOcrSample.Core;
using UwpOcrSample.Models;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Xaml.Media.Imaging;

namespace UwpOcrSample.ViewModels
{
    /// <summary>
    /// MainPage.xamlのViewModelクラス
    /// </summary>
    internal class MainPageViewModel : BindableBase
    {
        #region メンバ変数
        /// <summary>
        /// 
        /// </summary>
        private DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        /// <summary>
        /// OCRモデルクラス
        /// </summary>
        private WindowsOcr _windowsOcr = new WindowsOcr();
        #endregion

        #region プロパティ
        /// <summary>
        /// ファイルパス
        /// </summary>
        public ReactiveProperty<string> FilePath { get; }
        /// <summary>
        /// OCR結果文字列
        /// </summary>
        public ReactiveProperty<string> OcrResultText { get; }
        /// <summary>
        /// OCR画像
        /// </summary>
        public ReactiveProperty<SoftwareBitmapSource> OcrImageSource { get; }
        /// <summary>
        /// 
        /// </summary>
        public ReactiveProperty<ObservableCollection<Rect>> Rects { get; }
        #endregion

        #region コマンド
        /// <summary>
        /// 「ファイル選択」コマンド
        /// </summary>
        public ReactiveCommand OpenFileCommand { get; }
        /// <summary>
        /// 「OCR実行」コマンド
        /// </summary>
        public ReactiveCommand OcrCommand { get; }
        #endregion

        #region コンストラクタ
        /// <summary>
        /// 
        /// </summary>
        public MainPageViewModel()
        {
            FilePath = new ReactiveProperty<string>(string.Empty);
            OcrResultText = new ReactiveProperty<string>(string.Empty);
            OcrImageSource = new ReactiveProperty<SoftwareBitmapSource>();
            Rects = new ReactiveProperty<ObservableCollection<Rect>>(new ObservableCollection<Rect>());

            OpenFileCommand = new ReactiveCommand();
            OpenFileCommand.Subscribe(_ => ExecuteOpenFileCommand());

            OcrCommand = new ReactiveCommand();
            OcrCommand.Subscribe(_ => ExecuteOcrCommand());
        }
        #endregion

        #region コマンドの実装
        /// <summary>
        /// ファイルを開く
        /// </summary>
        private async void ExecuteOpenFileCommand()
        {
            var picker = new FileOpenPicker()
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                FileTypeFilter = { ".jpg", ".jpeg", ".png", ".bmp" }
            };

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // 画像ファイルの読み込み
                // cf. https://learn.microsoft.com/ja-jp/windows/uwp/audio-video-camera/imaging
                using (var s = await file.OpenReadAsync())
                {
                    var decoder = await BitmapDecoder.CreateAsync(s);

                    var softwareBitmap = await decoder.GetSoftwareBitmapAsync();
                    if (softwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 || softwareBitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
                    {
                        softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                    }
                    var source = new SoftwareBitmapSource();
                    await source.SetBitmapAsync(softwareBitmap);

                    OcrImageSource.Value = source;
                    _windowsOcr.OcrImage = softwareBitmap;
                }
                FilePath.Value = file.Path;
            }
        }

        /// <summary>
        /// OCRを実行する
        /// </summary>
        private async void ExecuteOcrCommand()
        {
            await _windowsOcr.ExecuteOcr();
            OcrResultText.Value = _windowsOcr.OcrResult.Text;
            Rects.Value = new ObservableCollection<Rect>(_windowsOcr.DetectRects);
        }
        #endregion
    }
}
