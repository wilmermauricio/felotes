using FeExcel.Application.Importacion;

namespace FeExcel.Application.Abstracciones;

/// <summary>Guarda temporalmente el resultado de una importación entre la vista previa y la confirmación.</summary>
public interface IAlmacenImportaciones
{
    void Guardar(ResultadoImportacion resultado);

    ResultadoImportacion? Obtener(Guid id);

    /// <summary>
    /// Obtiene y quita la importación en una sola operación atómica.
    /// Si dos peticiones la retiran a la vez, solo una la recibe.
    /// </summary>
    ResultadoImportacion? Retirar(Guid id);
}
