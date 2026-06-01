using UnityEngine;

public class CargarAjustes : MonoBehaviour
{
    // Ejecuta la recuperación de preferencias de audio durante la inicialización del objeto.
    void Awake()
    {
        // Obtiene los valores de volumen almacenados en el sistema local, aplicando valores por defecto si no existen.
        float volGeneral = PlayerPrefs.GetFloat("VolumenGeneral", 100f);
        float volEfectos = PlayerPrefs.GetFloat("VolumenEfectos", 100f);

        // Verifica la disponibilidad de la instancia del AudioManager para aplicar las configuraciones.
        if (AudioManager.Instance != null)
        {
            // Normaliza el valor de volumen (0.0 a 1.0) y lo asigna a la fuente de música.
            if (AudioManager.Instance.MusicaSource != null)
                AudioManager.Instance.MusicaSource.volume = volGeneral / 100f;

            // Normaliza el valor de volumen (0.0 a 1.0) y lo asigna a la fuente de efectos sonoros.
            if (AudioManager.Instance.SfxSource != null)
                AudioManager.Instance.SfxSource.volume = volEfectos / 100f;
        }
    }
}