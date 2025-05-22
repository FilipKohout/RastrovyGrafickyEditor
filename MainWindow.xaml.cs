using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace RastrovyGrafickyEditor;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Brush currentBrush = Brushes.Black;
    private Point startPoint;
    private bool isDrawingShape = false;

    public MainWindow()
    {
        InitializeComponent();
        DrawingCanvas.DefaultDrawingAttributes.Color = Colors.Black;
        DrawingCanvas.DefaultDrawingAttributes.Width = BrushSizeSlider.Value;
        DrawingCanvas.DefaultDrawingAttributes.Height = BrushSizeSlider.Value;

        BrushSizeSlider.ValueChanged += (s, e) =>
        {
            DrawingCanvas.DefaultDrawingAttributes.Width = e.NewValue;
            DrawingCanvas.DefaultDrawingAttributes.Height = e.NewValue;
        };

        ShapePicker.SelectionChanged += (s, e) =>
        {
            DrawingCanvas.EditingMode = (ShapePicker.SelectedItem as ComboBoxItem)?.Content.ToString() == "None" ? InkCanvasEditingMode.Ink : InkCanvasEditingMode.None;
        };
    }

    private void ColorPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DrawingCanvas == null) return;

        string color = (ColorPicker.SelectedItem as ComboBoxItem)?.Content.ToString();
        if (color != null)
        {
            Color selected = (Color)ColorConverter.ConvertFromString(color);
            DrawingCanvas.DefaultDrawingAttributes.Color = selected;
            currentBrush = new SolidColorBrush(selected);
        }
    }

    private void DrawingCanvas_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        string shape = (ShapePicker.SelectedItem as ComboBoxItem)?.Content.ToString();
        if (shape != "None")
        {
            startPoint = e.GetPosition(DrawingCanvas);
            isDrawingShape = true;
            DrawingCanvas.MouseMove += DrawingCanvas_MouseMove_Shape;
            DrawingCanvas.MouseUp += DrawingCanvas_MouseUp_Shape;
        }
    }

    private void DrawingCanvas_MouseMove_Shape(object sender, MouseEventArgs e)
    {
        // Optional: live preview of shape while dragging (not implemented)
    }

    private void DrawingCanvas_MouseUp_Shape(object sender, MouseButtonEventArgs e)
    {
        DrawingCanvas.MouseMove -= DrawingCanvas_MouseMove_Shape;
        DrawingCanvas.MouseUp -= DrawingCanvas_MouseUp_Shape;

        Point endPoint = e.GetPosition(DrawingCanvas);
        string shape = (ShapePicker.SelectedItem as ComboBoxItem)?.Content.ToString();

        double x = Math.Min(startPoint.X, endPoint.X);
        double y = Math.Min(startPoint.Y, endPoint.Y);
        double width = Math.Abs(startPoint.X - endPoint.X);
        double height = Math.Abs(startPoint.Y - endPoint.Y);

        Shape element = null;
        switch (shape)
        {
            case "Rectangle":
                element = new Rectangle { Width = width, Height = height };
                break;
            case "Square":
                double side = Math.Min(width, height);
                element = new Rectangle { Width = side, Height = side };
                break;
            case "Ellipse":
                element = new Ellipse { Width = width, Height = height };
                break;
        }

        if (element != null)
        {
            element.Stroke = currentBrush;
            element.StrokeThickness = BrushSizeSlider.Value;
            Canvas.SetLeft(element, x);
            Canvas.SetTop(element, y);
            DrawingCanvas.Children.Add(element);
        }

        isDrawingShape = false;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        RenderTargetBitmap rtb = new RenderTargetBitmap((int)DrawingCanvas.ActualWidth, (int)DrawingCanvas.ActualHeight, 96d, 96d, PixelFormats.Default);
        rtb.Render(DrawingCanvas);

        SaveFileDialog save = new SaveFileDialog
        {
            Filter = "PNG Image|*.png"
        };
        if (save.ShowDialog() == true)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            using FileStream fs = new FileStream(save.FileName, FileMode.Create);
            encoder.Save(fs);
        }
    }

    private void LoadButton_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog open = new OpenFileDialog
        {
            Filter = "PNG Image|*.png"
        };
        if (open.ShowDialog() == true)
        {
            Image img = new Image
            {
                Source = new BitmapImage(new Uri(open.FileName)),
                Width = DrawingCanvas.ActualWidth,
                Height = DrawingCanvas.ActualHeight
            };
            DrawingCanvas.Children.Add(img);
        }
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        DrawingCanvas.Strokes.Clear();
        DrawingCanvas.Children.Clear();
    }
}