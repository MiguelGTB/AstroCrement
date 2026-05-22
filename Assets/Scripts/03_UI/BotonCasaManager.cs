using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonCasaManager : MonoBehaviour
{
    [Header("Configuración de Escena")]
    public string nombreEscenaSelector = "Seleccion_Niveles";

    public void VolverAlSelectorDeNiveles()
    {
        Debug.Log("BotonCasa: Guardando partida antes de salir...");

        // 1. Forzamos a la base de datos a guardar TODO en internet AHORA MISMO
        if (DatabaseManager.Instance != null)
        {
            DatabaseManager.Instance.GuardarPartidaEnNube();
        }

        // 2. Viajamos de forma segura al selector de niveles
        Debug.Log("BotonCasa: Viajando a " + nombreEscenaSelector);
        SceneManager.LoadScene(nombreEscenaSelector);
    }
}