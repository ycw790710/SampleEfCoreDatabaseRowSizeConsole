namespace SampleEfCoreDatabaseRowSizeConsole.Databases.Models;

public class UserNotification
{
    public int Id { get; }

    public int UserId { get; private set; }

    public string Message { get; private set; }

    public UserNotification(int userId, string message)
    {
        UserId = userId;
        Message = message;
    }

}