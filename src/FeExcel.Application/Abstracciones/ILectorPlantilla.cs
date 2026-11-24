using FeExcel.Application.Plantilla;

namespace FeExcel.Application.Abstracciones;

/// <summary>Lee el archivo Excel y lo convierte en filas tipadas.</summary>
public interface ILectorPlantilla
{
    LecturaPlantilla Leer(Stream archivo);
}
