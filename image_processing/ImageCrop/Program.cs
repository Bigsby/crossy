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
        for (var y = start_y; y < image.Height; y++)
            for (var x = start_x; x < image.Width; x++)
                if (image[x, y] == color)
                    return (x, y);
        throw new Exception($"{color} not found");
    }


    static void ExtractCharacters(string file)
    {
        WriteLine($"Processing: {file}");
        var name = Path.GetFileNameWithoutExtension(file);
        var directory = Path.GetDirectoryName(file) ?? ".";
        using var image = Image.Load<Rgba32>(file);
        var half = image.Width / 2;
        var targetColor = new Rgba32(115, 196, 246, 255);
        var (current_x, current_y, end_x, end_y) = (0, 0, 0, 0);
        var index = 0;
        while (true)
        {
            var (start_x, start_y) = (current_x, current_y);
            WriteLine($"start {start_x},{start_y} {end_x},{end_y}");
            var nextFound = false;
            var newLineAdded = false;
            for (start_y = current_y; start_y < image.Height && !nextFound; start_y++)
            {
                for (start_x = current_x; start_x < image.Width && !nextFound; start_x++)
                    if (image[start_x, start_y] == targetColor)
                        nextFound = true;
                if (!nextFound && !newLineAdded && end_y != 0)
                {
                    newLineAdded = true;
                    start_y = end_y + 10;
                    current_x = 0;
                    WriteLine($"new line {start_x},{start_y}");
                }
            }

            if (!nextFound || start_x >= image.Width || start_y >= image.Height)
                break;
            end_x = start_x;
            end_y = start_y;
            while (end_x < image.Width && image[end_x, start_y] == targetColor)
                end_x++;
            while (end_y < image.Height && image[start_x, end_y] == targetColor)
                end_y++;
            try
            {
                var cropped = image.Clone(i => i.Crop(new Rectangle(start_x, start_y, end_x - start_x, end_y - start_y)));
                var destinationPath = Path.Combine(directory, "cropped", $"{name}_{index++}.png");
                cropped.SaveAsPng(destinationPath);
                WriteLine($"{index} {start_x},{start_y} {end_x},{end_y} {end_x - start_x}x{end_y - start_y} {destinationPath}");
                WriteLine();
            }
            catch
            {
                WriteLine($"error processing {file} {index}");
            }
            (current_x, current_y) = (end_x, start_y);
        }
        WriteLine("==================///////////////////========================");
    }

    private static void Main(string[] args)
    {
        foreach (var filePath in args)
        {

            if (!File.Exists(filePath))
                WriteLine($"{filePath} not a valid file path");
            else
                ExtractCharacters(filePath);
        }
    }
}