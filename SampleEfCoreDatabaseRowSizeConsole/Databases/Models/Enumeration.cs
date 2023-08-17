using System.Reflection;

namespace SampleEfCoreDatabaseRowSizeConsole.Databases.Models;
public abstract class Enumeration : IComparable
{
    private static Dictionary<Type, HashSet<int>> _typeIds = new();
    public string Name { get; private set; }

    public int Id { get; private set; }

    protected Enumeration(int id, string name)
    {
        var type = GetType();
        if (!_typeIds.ContainsKey(type))
            _typeIds.Add(type, new());
        if (_typeIds[type].Contains(id))
            throw new Exception("Repeat Enumeration");
        else
            _typeIds[type].Add(id);
        (Id, Name) = (id, name);
    }

    //public override string ToString() => Name;

    public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
        typeof(T).GetFields(BindingFlags.Public |
                            BindingFlags.Static |
                            BindingFlags.DeclaredOnly)
                    .Select(f => f.GetValue(null))
                    .Cast<T>();

    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;
        if (obj is not Enumeration otherValue)
        {
            return false;
        }

        var typeMatches = GetType().Equals(obj.GetType());
        var valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
    {
        var absoluteDifference = Math.Abs(firstValue.Id - secondValue.Id);
        return absoluteDifference;
    }

    public static T FromValue<T>(int value) where T : Enumeration
    {
        var matchingItem = Parse<T, int>(value, "value", item => item.Id == value);
        return matchingItem;
    }

    public static T FromDisplayName<T>(string displayName) where T : Enumeration
    {
        var matchingItem = Parse<T, string>(displayName, "display name", item => item.Name == displayName);
        return matchingItem;
    }

    private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration
    {
        var matchingItem = GetAll<T>().FirstOrDefault(predicate);

        if (matchingItem == null)
            throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");

        return matchingItem;
    }

    public int CompareTo(object? other) => other == null ? 1 : Id.CompareTo(((Enumeration)other).Id);

    public static void Valid<T>(IEnumerable<int> ids, bool isDistinct = true) where T : Enumeration
    {
        var valid = true;
        var items = GetAll<T>();
        var total = 0;
        foreach (var item in items)
        {
            var count = ids.Count(n => n == item.Id);
            if (count > 1 && isDistinct)
                valid = false;
            total += count;
        }
        if (total != ids.Count())
            valid = false;
        if (!valid)
            throw new Exception("Valid fail");
    }
    public static void Valid<T>(int id) where T : Enumeration
    {
        var items = GetAll<T>();
        var valid = items.Any(n => n.Id == id);
        if (!valid)
            throw new Exception("Valid fail");
    }
}