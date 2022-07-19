using TestApp;
using Util;

public class App
{
    public static void Main(string[] args)
    {
        SerializerTest serializerTest = new SerializerTest();
        serializerTest.Start();

        Console.ReadLine();
    }
}