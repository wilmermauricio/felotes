using DGII.FE.XML.Models.Externo;
using DGII.FE.XML.Models.Externo.Comprobantes;
using FeExcel.Application.Importacion;
using FeExcel.Domain.Comprobantes;

namespace FeExcel.Application.Dgii;

/// <summary>
/// Construye el modelo de la librería DGII correspondiente a un tipo de e-CF.
/// Los setters de la librería validan los valores y lanzan excepciones si algo no es válido.
/// </summary>
public interface IConstructorComprobante
{
    TipoEcf Tipo { get; }

    Comprobante Construir(GrupoComprobante grupo, Emisor emisor);
}
