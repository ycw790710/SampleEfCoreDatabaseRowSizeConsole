namespace SampleEfCoreDatabaseRowSizeConsole.Databases.Models;

public class UserNotificationDetail
{
    public int Id { get; }
    public string Content { get; private set; }
    public int UserNotificationId { get; }

    public UserNotificationDetail(string content)
    {
        Content = content;
    }
}