namespace ES.Generation;

public class NamespaceComparer : IComparer<string>
{
    public static NamespaceComparer Default { get; } = new();
    private static string[] NamespaceOrder = ["System", "Microsoft", ""];

    public int Compare(string x, string y)
    {
        var xi = Array.FindIndex(NamespaceOrder, x.StartsWith);
        var yi = Array.FindIndex(NamespaceOrder, y.StartsWith);

        if (xi > yi)
            return -1;
        if (yi < xi)
            return 1;
        return StringComparer.Ordinal.Compare(x, y);
    }
}
