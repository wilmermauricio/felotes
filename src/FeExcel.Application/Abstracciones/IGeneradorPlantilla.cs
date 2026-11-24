namespace FeExcel.Application.Abstracciones;

/// <summary>Genera la plantilla Excel vacía que el usuario descarga.</summary>
public interface IGeneradorPlantilla
{
    byte[] Generar();
}
