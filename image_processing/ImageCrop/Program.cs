using Microsoft.VisualBasic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using static System.Console;

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
            var croppedName = Path.Combine(directory, $"{name}_{index}.png");
            var cropped = image.Clone(i => i.Crop(GetRect(index)));
            cropped.SaveAsPng(croppedName);
            WriteLine($"{croppedName} saved.");
        }
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