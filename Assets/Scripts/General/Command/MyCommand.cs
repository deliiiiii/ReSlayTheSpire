public abstract class AbstractCommand
{
    public abstract void OnExecute();
}

public class MyCommand
{
    public static void Send<T>(T command) where T : AbstractCommand
    {
        command.OnExecute();
    }
}