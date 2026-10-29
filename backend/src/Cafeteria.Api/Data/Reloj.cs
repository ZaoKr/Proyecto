namespace Cafeteria.Api.Data;

public static class Reloj
{
    private static readonly TimeZoneInfo ZonaBolivia = ObtenerZonaBolivia();

    public static DateTime Ahora()
    {
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ZonaBolivia);
    }

    public static DateTime Hoy()
    {
        return Ahora().Date;
    }

    private static TimeZoneInfo ObtenerZonaBolivia()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("America/La_Paz");
        }
        catch (TimeZoneNotFoundException)
        {
            return ZonaFija();
        }
        catch (InvalidTimeZoneException)
        {
            return ZonaFija();
        }
    }

    private static TimeZoneInfo ZonaFija()
    {
        return TimeZoneInfo.CreateCustomTimeZone(
            "Bolivia",
            TimeSpan.FromHours(-4),
            "Bolivia",
            "Bolivia");
    }
}
