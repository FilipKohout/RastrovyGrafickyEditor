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
    private enum ShapeType
    {
        Rectangle,
        Square,
        Ellipse
    }
    
    private class DrawShape
    {
        public ShapeType Type { get; set; }
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        
        public Shape element { get; set; }
        
        public DrawShape(ShapeType type, Point startPoint)
        {
            Type = type;
            StartPoint = startPoint;
            
            switch (type)
            {
                case ShapeType.Rectangle:
                    element = new Rectangle();
                    break;
                case ShapeType.Square:
                    element = new Rectangle();
                    break;
                case ShapeType.Ellipse:
                    element = new Ellipse();
                    break;
            }
            
            Draw();
        }

        public void Draw()
        {
            if (element != null)
            {
                element.Stroke = currentBrush;
                element.StrokeThickness = BrushSizeSlider.Value;
                InkCanvas.SetLeft(element, x);
                InkCanvas.SetTop(element, y);
                DrawingCanvas.Children.Add(element);
            }
        }
    }
}