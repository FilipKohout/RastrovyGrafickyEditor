using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;
using Ellipse = System.Windows.Shapes.Ellipse;
using Polygon = System.Windows.Shapes.Polygon;
using Line = System.Windows.Shapes.Line;

namespace RastrovyGrafickyEditor;

public enum ShapeType
{
    Line,
    Rectangle,
    Square,
    Ellipse,
    Triangle
}

public class DrawShape
{
    private static int zIndex = 0;
    private readonly ShapeType type;
    private readonly Shape? element;
    private readonly Point startPoint;
    private readonly InkCanvas drawingCanvas;
    public Point EndPoint { get; set; }

    public DrawShape(ShapeType type, InkCanvas canvas, Point startPoint)
    {
        this.type = type;
        this.drawingCanvas = canvas;
        this.startPoint = startPoint;

        element = CreateShapeElement(type);
        if (element == null) return;

        element.Visibility = Visibility.Collapsed;
        Canvas.SetZIndex(element, ++zIndex);
        drawingCanvas.Children.Add(element);

        Draw();
    }

    public void Draw()
    {
        element.Stroke = new SolidColorBrush(Settings.borderColor);
        element.Fill = new SolidColorBrush(Settings.fillColor);
        element.StrokeThickness = Settings.thickness;
        element.Visibility = Visibility.Visible;

        switch (type)
        {
            case ShapeType.Rectangle:
                DrawRectangle(false);
                break;
            case ShapeType.Square:
                DrawRectangle(true);
                break;
            case ShapeType.Ellipse:
                DrawEllipse();
                break;
            case ShapeType.Line:
                DrawLine();
                break;
            case ShapeType.Triangle:
                DrawTriangle();
                break;
        }
    }

    private Shape? CreateShapeElement(ShapeType type)
    {
        return type switch
        {
            ShapeType.Line => new Line(),
            ShapeType.Rectangle => new Rectangle(),
            ShapeType.Square => new Rectangle(),
            ShapeType.Ellipse => new Ellipse(),
            ShapeType.Triangle => new Polygon(),
            _ => null
        };
    }

    private void DrawRectangle(bool square)
    {
        double width = Math.Abs(startPoint.X - EndPoint.X);
        double height = Math.Abs(startPoint.Y - EndPoint.Y);

        if (square)
        {
            double side = Math.Min(width, height);
            width = height = side;
        }

        element.Width = width;
        element.Height = height;

        InkCanvas.SetLeft(element, Math.Min(startPoint.X, EndPoint.X));
        InkCanvas.SetTop(element, Math.Min(startPoint.Y, EndPoint.Y));
    }

    private void DrawEllipse()
    {
        DrawRectangle(false); // same logic as rectangle
    }

    private void DrawLine()
    {
        if (element is not Line line) return;

        line.X1 = startPoint.X;
        line.Y1 = startPoint.Y;
        line.X2 = EndPoint.X;
        line.Y2 = EndPoint.Y;
    }

    private void DrawTriangle()
    {
        if (element is not Polygon polygon) return;

        double x1 = startPoint.X;
        double y1 = EndPoint.Y;
        double x2 = (startPoint.X + EndPoint.X) / 2;
        double y2 = startPoint.Y;
        double x3 = EndPoint.X;
        double y3 = EndPoint.Y;

        polygon.Points = new PointCollection
        {
            new Point(x1, y1),
            new Point(x2, y2),
            new Point(x3, y3)
        };
    }
}
