namespace ConsoleRasterizer;

public class PostProcess
{
    public int KernelSize = 3;
    public int HalfKernelSize => (int)(KernelSize / 2);


    public void Apply(FrameBuffer buffer)
    {
        var temp = new FrameBuffer(buffer.Width, buffer.Height);

        ApplyHorizontalBuffer(buffer, temp);

        ApplyVerticalBuffer(temp, buffer);

    }

    private void ApplyHorizontalBuffer(FrameBuffer source, FrameBuffer target)
    {
        for (int y = 0; y < source.Height; y++)
        {
            for (int x = 0; x < source.Width; x++)
            {
                var brightness = CalculateBrightness(x, source.Width, k => source.GetBrightnessIn(k, y));
                target.SetPixel(x, y, new Pixel(brightness));
            }
        }
    }
    
    private void ApplyVerticalBuffer(FrameBuffer source, FrameBuffer target)
    {
        for (int x = 0; x < source.Width; x++)
        {
            for (int y = 0; y < source.Height; y++)
            {
                var brightness = CalculateBrightness(y, source.Height, k => source.GetBrightnessIn(x, k));
                target.SetPixel(x, y, new Pixel(brightness));
            }
        }
    }

    private int CalculateBrightness(int index, int maxSize, Func<int, double> brightnessGet) 
    {
        var sum = 0.0;
        for(int k = -HalfKernelSize; k <= HalfKernelSize; k++)
        {
            var indexToCheck = k + index;
            if (indexToCheck < 0 || indexToCheck >= maxSize)  
                continue;

            sum += brightnessGet(indexToCheck) * (1.0 / KernelSize);
        }
    
        return (int)Math.Floor(sum);
    }
}
