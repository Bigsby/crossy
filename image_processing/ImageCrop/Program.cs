using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using static System.Console;
using System.Linq;
using System.Collections.Generic;
using SixLabors.ImageSharp.PixelFormats;

internal class Program
{
    // size 330 435
    // top top-left 30 590 330 435
    // next row +450
    // next column +345
    const int WIDTH = 330;
    const int HEIGHT = 435;
    const int NEXT_COLUMN = 345;
    const int NEXT_ROW = 450;

    static Rectangle GetRect(int index)
        => new(
            30 + index % 3 * NEXT_COLUMN,
            590 + index / 3 * NEXT_ROW,
            WIDTH,
            HEIGHT
            );

    static void CropImage(string file, int count)
    {
        var name = Path.GetFileNameWithoutExtension(file);
        var directory = Path.GetDirectoryName(file) ?? ".";
        using var image = Image.Load(file);
        WriteLine($"{file} {image.Metadata?.DecodedImageFormat?.Name}");
        foreach (var index in Enumerable.Range(0, count))
        {
            var croppedName = Path.Combine(directory, "cropped", $"{name}_{index}.png");
            var cropped = image.Clone(i => i.Crop(GetRect(index)));
            cropped.SaveAsPng(croppedName);
            WriteLine($"{croppedName} saved.");
        }
    }

    static (int, int) GetFirst(Image<Rgba32> image, Rgba32 color, int start_x, int start_y)
    {
        for (var y = 0; y < image.Height; y++)
            for (var x = 0; x < image.Width; x++)
                if (image[x, y] == color)
                    return (x, y);
        throw new Exception($"{color} not found");
    }
   
    
    static void GetPixel(string file)
    {
        var name = Path.GetFileNameWithoutExtension(file);
        var directory = Path.GetDirectoryName(file) ?? ".";
        using var image = Image.Load<Rgba32>(file);
        var half = image.Width / 2;
        var targetColor = new Rgba32(115, 196, 246, 255);
        var (start_x, start_y) = GetFirst(image, targetColor, 0, 0);
        var end_x = start_x;
        var end_y = start_y;
        while (end_x < image.Width && image[end_x, start_y] == targetColor)
            end_x++;
        while (end_y < image.Height && image[start_x, end_y] == targetColor)
            end_y++;
        WriteLine($"{start_x},{start_y} {end_x},{end_y}");
        var cropped = image.Clone(i => i.Crop(new Rectangle(start_x, start_y, end_x - start_x, end_y - start_y)));
        cropped.SaveAsPng(Path.Combine(directory, "cropped", $"{name}_first.png"));
        
    }

    private static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            WriteLine("filePath count");
            Environment.Exit(1);
        }
        var filePath = args[0];
        if (!File.Exists(filePath))
        {
            WriteLine($"{filePath} not a valid file path");
            Environment.Exit(1);
        }
        GetPixel(filePath) ;
        return;
        if (int.TryParse(args[1], out var count))
        {
            CropImage(filePath, count);
        }
        else
        {
            WriteLine("invalid rect values");
            Environment.Exit(1);
        }
    }
}