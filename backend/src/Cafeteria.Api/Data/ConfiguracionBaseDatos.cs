using Npgsql;

namespace Cafeteria.Api.Data;

public static class ConfiguracionBaseDatos
{
    public static string CadenaPostgres(string url)
    {
        if (!url.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase)
            && !url.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            return url;
        }

        Uri uri = new Uri(url);
        string[] datosUsuario = uri.UserInfo.Split(':', 2);

        NpgsqlConnectionStringBuilder cadena = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Username = Uri.UnescapeDataString(datosUsuario[0]),
            Password = datosUsuario.Length > 1 ? Uri.UnescapeDataString(datosUsuario[1]) : "",
            Database = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/'))
        };

        foreach (string parte in uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            string[] par = parte.Split('=', 2);

            if (par.Length == 2
                && par[0].Equals("sslmode", StringComparison.OrdinalIgnoreCase)
                && Enum.TryParse(par[1].Replace("-", ""), true, out SslMode modo))
            {
                cadena.SslMode = modo;
            }
        }

        return cadena.ConnectionString;
    }
}
