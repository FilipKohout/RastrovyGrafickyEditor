using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace RastrovyGrafickyEditor;

public enum DrawingMode
{
    None,
    Shape,
}

public static class Settings
{
    public static Color borderColor = Colors.Black;
    public static Color fillColor = Colors.DodgerBlue;
    public static double thickness = 5;
    public static DrawingMode drawingMode = DrawingMode.None;
    public static DrawShape? currentShape;
}