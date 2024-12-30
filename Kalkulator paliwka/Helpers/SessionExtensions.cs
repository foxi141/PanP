using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Kalkulator_paliwka
{
    public static class SessionExtensions
    {
        // Metoda do zapisywania obiektu w sesji
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        // Metoda do odczytywania obiektu z sesji
        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
