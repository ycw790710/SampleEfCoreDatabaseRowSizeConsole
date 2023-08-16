namespace SampleEfCoreDatabaseRowSizeConsole.Databases.Models;

public class FileStore
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public byte[] _data { get; set; }
    public IReadOnlyCollection<byte> Data => _data;
}