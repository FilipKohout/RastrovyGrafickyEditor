﻿using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Forms = System.Windows.Forms;
using Image = System.Windows.Controls.Image;

namespace RastrovyGrafickyEditor;

public static class Files
{
    public static bool Saved = true;
    
    public static void Save(InkCanvas canvas, string path)
    {
        RenderTargetBitmap rtb = new RenderTargetBitmap(
            (int)canvas.ActualWidth, 
            (int)canvas.ActualHeight, 
            96d, 
            96d, 
            // Color format
            PixelFormats.Default
        );
        rtb.Render(canvas);

        PngBitmapEncoder encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(rtb));

        // Make sure to handle the "file is being processed by another process" error
        try
        {
            using FileStream fs = new FileStream(path, FileMode.Create);
            encoder.Save(fs);
            fs.Close();
            
            Saved = true;
        }
        catch (Exception e)
        {
            Forms.MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public static void Load(InkCanvas canvas, string path)
    {
        BitmapImage bitmap = new BitmapImage();

        // Open and close the file to prevent the "file is being processed by another process" error
        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = fs;
            bitmap.EndInit();
            bitmap.Freeze();
        }

        Image img = new()
        {
            Source = bitmap,
            // Cut the image to fit the canvas
            Width = canvas.ActualWidth,
            Height = canvas.ActualHeight
        };

        Clear(canvas);
        canvas.Children.Add(img);
        
        Saved = true;
    }
    
    public static void Clear(InkCanvas canvas)
    {
        canvas.Strokes.Clear(); // Clear the drawings
        canvas.Children.Clear(); // Clear the shapes
        
        Saved = true;
    }
}