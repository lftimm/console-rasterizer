using ConsoleRasterizer;

try
{
    var cubefactory = () => ObjImporter.Read(@"Assets\cube.obj");
    var cube = Matrix4.Displace(-2f,0,0).Transform(cubefactory());
    var cube2 = Matrix4.Displace(2f,0,0).Transform(cubefactory());
        
    var x = Console.WindowWidth;
    var y = Console.WindowHeight-1;

    var window = new ConsoleEngine(x,y);
    window.RenderScene((t) =>
    {
        return [.. cube, ..cube2];
    });

} catch (Exception ex)
{
    Console.Clear();
    Console.Write(
        $"Exception thrown: {ex.GetType()}\n" +
        $"StackTrace:\n{ex.StackTrace}\n"
    );
}
