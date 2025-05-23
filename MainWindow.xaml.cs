using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;
using Forms = System.Windows.Forms;
using Color = System.Windows.Media.Color;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;


namespace RastrovyGrafickyEditor;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // Init shapes
        ShapePicker.Items.Add(new ComboBoxItem { Content = "None", IsSelected = true});
        foreach (var type in ShapeType.GetValues(typeof(ShapeType)))
            ShapePicker.Items.Add(new ComboBoxItem { Content = type.ToString() });
        
        DrawingCanvas.DefaultDrawingAttributes.Width = Settings.thickness;
        DrawingCanvas.DefaultDrawingAttributes.Height = Settings.thickness;
        BrushSizeSlider.Value = Settings.thickness;

        DrawingCanvas.DefaultDrawingAttributes.Color = Settings.borderColor;
        BorderColorButton.BorderBrush = new SolidColorBrush(Settings.borderColor);
        FillColorButton.Background = new SolidColorBrush(Settings.fillColor);
        
        // Handle thickness change
        BrushSizeSlider.ValueChanged += (s, e) =>
        {
            Settings.thickness = e.NewValue;
            DrawingCanvas.DefaultDrawingAttributes.Width = Settings.thickness;
            DrawingCanvas.DefaultDrawingAttributes.Height = Settings.thickness;
        };

        // Handle draw mode change
        ShapePicker.SelectionChanged += (s, e) =>
        {
            Settings.drawingMode = (ShapePicker.SelectedItem as ComboBoxItem)?.Content.ToString() == "None" ? DrawingMode.None : DrawingMode.Shape;
            DrawingCanvas.EditingMode = Settings.drawingMode == DrawingMode.None ? InkCanvasEditingMode.Ink : InkCanvasEditingMode.None;
        };
        
        this.SizeChanged += (s, e) =>
        {
            SizeLabel.Text = $"{Math.Floor(DrawingCanvas.ActualWidth)} x {Math.Floor(DrawingCanvas.ActualHeight)}";
        };
    }
    
    private void OpenColorPicker(object sender, RoutedEventArgs e)
    {
        var colorDialog = new ColorDialog();

        if (colorDialog.ShowDialog() == Forms.DialogResult.OK)
        {
            // Convert the selected color to MediaColor
            Color mediaColor = Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
            if (sender == BorderColorButton)
            {
                DrawingCanvas.DefaultDrawingAttributes.Color = mediaColor;
                Settings.borderColor = mediaColor;
                BorderColorButton.BorderBrush = new SolidColorBrush(mediaColor);
            }
            else if (sender == FillColorButton)
            {
                Settings.fillColor = mediaColor;
                FillColorButton.Background = new SolidColorBrush(mediaColor);
            }
        }
    }

    private void StartDrawing(object sender, MouseButtonEventArgs e)
    {
        string shape = (ShapePicker.SelectedItem as ComboBoxItem)?.Content.ToString();
        if (shape != "None")
        {
            Settings.currentShape = new DrawShape(
                (ShapeType)Enum.Parse(typeof(ShapeType), shape!), 
                DrawingCanvas, 
                e.GetPosition(DrawingCanvas)
            );
            
            void MouseMove(object sender, MouseEventArgs e)
            {
                Settings.currentShape.EndPoint = e.GetPosition(DrawingCanvas);
                Settings.currentShape.Draw();
            }
            
            void MouseUp(object sender, MouseButtonEventArgs e)
            {
                DrawingCanvas.MouseMove -= MouseMove;
                DrawingCanvas.MouseUp -= MouseUp;

                Settings.currentShape.EndPoint = e.GetPosition(DrawingCanvas);
                Settings.currentShape.Draw();
                
                // Stop updating the shape
                Settings.currentShape = null;
            }

            DrawingCanvas.MouseMove += MouseMove;
            DrawingCanvas.MouseUp += MouseUp;
        }
    }

    private void Clear(object sender, RoutedEventArgs e)
    {
        Files.Clear(DrawingCanvas);
    }

    private void Save(object sender, RoutedEventArgs e)
    {
        SaveFileDialog save = new SaveFileDialog { Filter = "PNG Image|*.png" };
        
        if (save.ShowDialog() == Forms.DialogResult.OK)
            Files.Save(DrawingCanvas, save.FileName);
    }

    private void Load(object sender, RoutedEventArgs e)
    {
        if (!Files.Saved)
        {
            DialogResult result = Forms.MessageBox.Show(
                "You have unsaved changes. Do you want to save them?",
                "Unsaved Changes",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning
            );

            if (result == Forms.DialogResult.Yes)
                Save(sender, e);
            else if (result == Forms.DialogResult.Cancel)
                return;
        }
        
        OpenFileDialog open = new OpenFileDialog { Filter = "PNG Image|*.png" };
        
        if (open.ShowDialog() == Forms.DialogResult.OK)
            Files.Load(DrawingCanvas, open.FileName);
    }

    private void OnDraw(object sender, InkCanvasStrokeCollectedEventArgs e)
    {
        Files.Saved = false;
    }

    private void SetTransparentFill(object sender, RoutedEventArgs e)
    {
        Settings.fillColor = Settings.fillColor == Colors.Transparent ? Colors.DodgerBlue : Colors.Transparent;
        FillColorButton.Visibility = Settings.fillColor == Colors.Transparent ? Visibility.Collapsed : Visibility.Visible;
        TransparentFillIcon.Foreground = Settings.fillColor == Colors.Transparent ? Brushes.DodgerBlue : Brushes.White;
    }
}