using UnityEngine;
using TMPro;

public class ArbolManager : MonoBehaviour
{
    public TextMeshProUGUI textoMonedasPrestigio;
    private MejoraPrestigio[] todasLasMejoras;

    void Start()
    {
        // 1. Hemos QUITADO las 1000 monedas de prueba.
        // Ahora el juego leerá tus monedas reales de Firebase.

        // 2. Buscamos todos los botones en la escena
        todasLasMejoras = FindObjectsOfType<MejoraPrestigio>();
        
        ActualizarTodoElArbol();
    }

    public void ActualizarTodoElArbol()
    {
        if (DatabaseManager.Instance == null || DatabaseManager.Instance.datosCargados == null) return;

        PlayerData datos = DatabaseManager.Instance.datosCargados;
        textoMonedasPrestigio.text = "Monedas Celestiales: " + datos.monedasPrestigio.ToString("F0");

        // Actualizamos el aspecto visual de cada botón
        foreach (var mejora in todasLasMejoras)
        {
            mejora.RefrescarEstadoVisual(datos);
        }
    }

    // ESTE ES EL BOTÓN DE SALIR DEL ÁRBOL
    public void VolverAlJuego()
    {
        // 1. GUARDADO EN BATCH: Guardamos todas las compras acumuladas de una sola vez
        if (DatabaseManager.Instance != null)
        {
            Debug.Log("Guardando árbol de mejoras en Firebase antes de salir...");
            DatabaseManager.Instance.GuardarPartidaEnNube();
        }

        // 2. Viajamos al selector de niveles
        UnityEngine.SceneManagement.SceneManager.LoadScene("Seleccion_Niveles");
    }
}