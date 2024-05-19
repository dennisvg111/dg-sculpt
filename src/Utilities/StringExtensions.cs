namespace DG.Sculpt.Utilities
{
    internal static class StringExtensions
    {
        public static bool TrySplitOn(this string s, string splitOn, out string firstOrFull, out string second)
        {
            int index = s.IndexOf(splitOn);
            if (index < 0)
            {
                firstOrFull = s;
                second = null;
                return false;
            }

            firstOrFull = s.Substring(0, index);
            second = s.Substring(index + splitOn.Length);
            return true;
        }
    }
}
