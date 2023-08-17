namespace SampleEfCoreDatabaseRowSizeConsole.Databases.Models;

public class FileStore
{
    public int Id { get; }

    public int UserId { get; private set; }

    private byte[] _data { get; set; }

    //public IReadOnlyCollection<byte> Data => _data;
    public byte[] Data => (byte[])_data.Clone();//for test

    public FileStore(int userId, byte[] data)
    {
        UserId = userId;
        _data = (byte[])data.Clone();
    }

}