using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;

namespace UwpOcrSample.Core
{
    internal class BindingHelper
    {
        #region Canvas.Top
        public static readonly DependencyProperty CanvasTopBindingPathProperty =
            DependencyProperty.RegisterAttached(
                "CanvasTopBindingPath", typeof(string), typeof(BindingHelper),
                new PropertyMetadata(null, CanvasTopBindingPathPropertyChanged));

        public static string GetCanvasTopBindingPath(DependencyObject obj)
        {
            return (string)obj.GetValue(CanvasTopBindingPathProperty);
        }
        public static void SetCanvasTopBindingPath(DependencyObject obj, string value)
        {
            obj.SetValue(CanvasTopBindingPathProperty, value);
        }

        private static void CanvasTopBindingPathPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is string propertyPath)
            {
                var canvasProperty =
                    e.Property == CanvasTopBindingPathProperty
                    ? Canvas.TopProperty
                    : Canvas.LeftProperty;

                BindingOperations.SetBinding(
                    obj,
                    canvasProperty,
                    new Binding { Path = new PropertyPath(propertyPath) });
            }
        }
        #endregion

        #region Canvas.Left
        public static readonly DependencyProperty CanvasLeftBindingPathProperty =
            DependencyProperty.RegisterAttached(
                "CanvasLeftBindingPath", typeof(string), typeof(BindingHelper),
                new PropertyMetadata(null, CanvasLeftBindingPathPropertyChanged));

        public static string GetCanvasLeftBindingPath(DependencyObject obj)
        {
            return (string)obj.GetValue(CanvasLeftBindingPathProperty);
        }
        public static void SetCanvasLeftBindingPath(DependencyObject obj, string value)
        {
            obj.SetValue(CanvasLeftBindingPathProperty, value);
        }

        private static void CanvasLeftBindingPathPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is string propertyPath)
            {
                var canvasProperty =
                    e.Property == CanvasLeftBindingPathProperty
                    ? Canvas.LeftProperty
                    : Canvas.TopProperty;

                BindingOperations.SetBinding(
                    obj,
                    canvasProperty,
                    new Binding { Path = new PropertyPath(propertyPath) });
            }
        }
        #endregion
    }
}
