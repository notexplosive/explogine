namespace ExplogineMonoGame;

public class ClientLuaUtilities
{
    public static void LogMessage(object[] array)
    {
        Client.Debug.Log("[lua]", array);
    }
}
