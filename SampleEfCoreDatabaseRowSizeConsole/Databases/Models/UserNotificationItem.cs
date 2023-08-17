namespace SampleEfCoreDatabaseRowSizeConsole.Databases.Models;

public class UserNotificationItem
{
    public int Id { get; }
    public string Detail { get; private set; }
    public int UserNotificationId { get; }

    public UserNotificationItem(string detail)
    {
        Detail = detail;
    }

}