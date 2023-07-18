namespace NorlysicalConsume.Extensions
{
    public static class StringExtensions
    {
        public static string CombineAsURL(this string baseURL, params string[] segments)
        {
            return AppendToURL(baseURL, segments);
        }

        public static string AppendToURL(this string baseURL, params string[] segments)
        {
            {
                return string.Join("/", new[] { baseURL.TrimEnd('/') }
                    .Concat(segments.Select(s => s.Trim('/'))));
            }
        }
    }

    public static class UriExtensions
    {
        public static Uri Combine(this Uri uri, params string[] segments)
        {
            return Append(uri, segments);
        }

        public static string Combine_ToAbsoluteUriString(this Uri uri, string[] segments)
        {
            return Append_ToAbsoluteUriString(uri, segments);
        }

        public static Uri Append(this Uri uri, params string[] segments)
        {
            return new Uri(Append_ToAbsoluteUriString(uri, segments));
        }

        public static string Append_ToAbsoluteUriString(this Uri uri, string[] segments)
        {
            return StringExtensions.AppendToURL(uri.AbsoluteUri, segments);
        }
    }
}
