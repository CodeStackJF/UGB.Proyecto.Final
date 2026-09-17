using System.Globalization;
using System.Security.Claims;

namespace UGB.Proyecto.Final.Helper
{
    /// <summary>
    /// Extensión para la entidad User para leer las propiedades
    /// </summary>
    public static class SessionHelper
    {
        public static dynamic GetProperty(this ClaimsPrincipal claimsPrincipal, string propertyName, Type T)
        {
            if (T == typeof(DateTime))
            {
                string value = claimsPrincipal.FindFirstValue(propertyName)!;
                DateTime dateTime = new DateTime();
                if (DateTime.TryParseExact(value, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                {
                    return dateTime;
                }

                if (DateTime.TryParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                {
                    return dateTime;
                }

                return DateTime.Now;
            }
            return Convert.ChangeType(claimsPrincipal.FindFirstValue(propertyName), T)!;
        }

        public static dynamic GetProperty(this ClaimsPrincipal claimsPrincipal, string propertyName)
        {
            return claimsPrincipal.FindFirstValue(propertyName)!.ToString();
        }
    }
}