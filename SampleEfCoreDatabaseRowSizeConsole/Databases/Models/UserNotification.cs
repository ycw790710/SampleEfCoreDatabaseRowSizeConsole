namespace SampleEfCoreDatabaseRowSizeConsole.Databases.Models;

public class UserNotification
{
    public int Id { get; }

    public int UserId { get; private set; }

    public string Message { get; private set; }

    public UserNotificationType Type { get; private set; }

    private UserNotificationDetail _userNotificationDetail;
    public UserNotificationDetail UserNotificationDetail => _userNotificationDetail;

    private List<UserNotificationItem> _userNotificationItems;
    public IEnumerable<UserNotificationItem> UserNotificationItems => _userNotificationItems;

    public UserNotification(int userId, string message)
    {
        UserId = userId;
        Message = message;

        Type = UserNotificationType.TypeB;

        _userNotificationDetail = new("TestContent");

        _userNotificationItems = new();
        _userNotificationItems.Add(new("TestDetail"));
    }

}