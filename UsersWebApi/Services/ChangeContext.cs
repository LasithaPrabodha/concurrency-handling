namespace UsersWebApi.Services;

public class ChangeContext
{
    public int EntityId { get; set; }
    public byte[] Timestamp { get; set; } = Array.Empty<byte>();

    private bool timestampTakenOnce = false;

    public byte[]? GetTimestampOnce()
    {
        if (!timestampTakenOnce)
        {
            timestampTakenOnce = true;
            return Timestamp;
        }

        return null;
    }
}

