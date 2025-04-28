using System.Text.Json;


namespace Ecommerce.Frontend.Helpers
{
    public static class SessionExtensions
    {
        // Store any object as JSON
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            var jsonString = JsonSerializer.Serialize(value);
            session.SetString(key, jsonString);
        }

        // Retrieve and deserialize object
        public static T? GetObject<T>(this ISession session, string key)
        {
            var jsonString = session.GetString(key);
            return jsonString is null ? default : JsonSerializer.Deserialize<T>(jsonString);
        }

        // For convenience: store string directly
        public static void SetStringSafe(this ISession session, string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(key))
                session.SetString(key, value);
        }

        // For convenience: get string with null check
        public static string? GetStringSafe(this ISession session, string key)
        {
            return string.IsNullOrWhiteSpace(key) ? null : session.GetString(key);
        }
    }

}
