string StrRange(char start, char stop)
{
    StringBuilder sb = new StringBuilder(stop - start + 2);
    for (char i = start; i <= stop; ++i)
        sb.Append(i);
    return sb.ToString();
}

string SortDistinct(string str) => string.Concat(new SortedSet<char>(str));

string Except(string str, params string[] arr)
{
    var set = new HashSet<char>(str);
    foreach (var s in arr)
        set.ExceptWith(s);
    return string.Concat(set);
}

string Intersect(string str, params string[] arr)
{
    var set = new HashSet<char>(str);
    foreach (var s in arr)
        set.RemoveWhere(c => !s.Contains(c));
    return string.Concat(set);
}
