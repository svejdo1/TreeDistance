namespace Barbar.TreeDistance.Util
{
    public static class Extensions
    {
        public static string JavaSubstring(this string s, int start, int end)
        {
            return s.Substring(start, end - start);
        }
    }
}
