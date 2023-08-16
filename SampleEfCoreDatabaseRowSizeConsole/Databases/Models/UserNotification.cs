namespace SampleEfCoreDatabaseRowSizeConsole.Databases.Models;

public class UserNotification
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Message { get; set; }
}