namespace ConsoleRasterizer;
public struct Pixel
{
    private static readonly char[] Map = "$@B%8&WM#*oahkbdpqwmZO0QLCJUYXzcvunxrjft/\\|()1{}[]?-_+~<>i!lI;:,\"^`'. ".Reverse().ToArray();
    private static readonly int BrightnessLevels = Enum.GetValues(typeof(Brightness)).Length -1;
    public int Brightness { get;  }
    public char Display { get; }

    public Pixel(int brightness)
    {
        Brightness = brightness;
        int brightnessPos = (int)((Brightness / (float)BrightnessLevels) * Map.Length);

        Display = Map[Math.Clamp(brightnessPos, 0, Map.Length-1)];
    }

    public static Pixel WithBrightness(Brightness brightness) => new ((int)brightness);
}
