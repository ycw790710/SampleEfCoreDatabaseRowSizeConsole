namespace SampleEfCoreDatabaseRowSizeConsole.Databases.Models;

public class UserNotificationType : Enumeration
{
    public static UserNotificationType TypeA { get; } = new(1, nameof(TypeA));
    public static UserNotificationType TypeB { get; } = new(2, nameof(TypeB));

    public UserNotificationType(int id, string name) : base(id, name) { }
}