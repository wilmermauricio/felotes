using System.Text.RegularExpressions;

namespace FeExcel.Domain.Comprobantes;

/// <summary>
/// Tipos de comprobante fiscal electrónico (e-CF) de la DGII.
/// El valor numérico es el código oficial del tipo (el "31" de "E310000000001").
/// </summary>
public enum TipoEcf
{
    FacturaCreditoFiscal = 31,
    FacturaConsumo = 32,
    NotaDebito = 33,
    NotaCredito = 34,
    Compras = 41,
    GastosMenores = 43,
    RegimenesEspeciales = 44,
    Gubernamental = 45,
    Exportaciones = 46,
    PagosExterior = 47
}

public static partial class TipoEcfExtensions
{
    /// <summary>Formato del e-NCF: la letra E, 2 dígitos de tipo y 10 de secuencia.</summary>
    [GeneratedRegex(@"^E\d{12}$")]
    private static partial Regex FormatoEncf();

    /// <summary>
    /// Deduce el tipo de e-CF a partir del e-NCF.
    /// Devuelve <c>false</c> y un mensaje de error si el formato o el tipo no son válidos.
    /// </summary>
    public static bool TryDesdeEncf(string encf, out TipoEcf tipo, out string error)
    {
        tipo = default;
        error = string.Empty;

        if (!FormatoEncf().IsMatch(encf))
        {
            error = $"El ENCF '{encf}' no es válido: debe tener 13 caracteres, empezar con 'E' y seguir con 12 dígitos";
            return false;
        }

        var codigo = int.Parse(encf.AsSpan(1, 2));
        if (!Enum.IsDefined(typeof(TipoEcf), codigo))
        {
            error = $"Tipo de e-CF '{codigo:00}' no soportado";
            return false;
        }

        tipo = (TipoEcf)codigo;
        return true;
    }

    public static string Codigo(this TipoEcf tipo) => ((int)tipo).ToString("00");

    public static string Descripcion(this TipoEcf tipo) => tipo switch
    {
        TipoEcf.FacturaCreditoFiscal => "Factura de Crédito Fiscal",
        TipoEcf.FacturaConsumo => "Factura de Consumo",
        TipoEcf.NotaDebito => "Nota de Débito",
        TipoEcf.NotaCredito => "Nota de Crédito",
        TipoEcf.Compras => "Compras",
        TipoEcf.GastosMenores => "Gastos Menores",
        TipoEcf.RegimenesEspeciales => "Regímenes Especiales",
        TipoEcf.Gubernamental => "Gubernamental",
        TipoEcf.Exportaciones => "Exportaciones",
        TipoEcf.PagosExterior => "Pagos al Exterior",
        _ => tipo.ToString()
    };
}
