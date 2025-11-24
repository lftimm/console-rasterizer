namespace ConsoleRasterizer;

public class KeyboardEventArgs : EventArgs
{
    public ConsoleKey Key { get; }
    public KeyboardEventArgs(ConsoleKey key)
    {
        Key = key; 
    }
}
public class KeyboardEventHandler
{
    public EventHandler<KeyboardEventArgs>? OnKeyPress;

    public KeyboardEventHandler()
    {
        Task.Run(Listen);
    }

    private async Task Listen()
    {
        while (true)
        {
            if (Console.KeyAvailable)
                OnKeyPress?.Invoke(this, new (Console.ReadKey(true).Key));

            await Task.Delay(16);
        }
    }
}
