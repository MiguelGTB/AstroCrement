using UnityEngine;
using TMPro;

public class ArbolManager : MonoBehaviour
{
    // Referencia al componente de texto que muestra el saldo actual de monedas.
    public TextMeshProUGUI textoMonedasPrestigio;
    
    // Almacena todos los componentes de mejora presentes en la escena.
    private MejoraPrestigio[] todasLasMejoras;

    void Start()
    {
        // Inicializa la lista de mejoras buscando todos los objetos de tipo MejoraPrestigio en la jerarquía.
        todasLasMejoras = FindObjectsOfType<MejoraPrestigio>();
        
        // Ejecuta la actualización inicial del árbol.
        ActualizarTodoElArbol();
    }

    // Sincroniza la interfaz y el estado de los componentes con los datos del perfil del usuario.
    public void ActualizarTodoElArbol()
    {
        // Verifica la existencia del gestor de datos para evitar errores de referencia nula.
        if (DatabaseManager.Instance == null || DatabaseManager.Instance.datosCargados == null) return;

        // Extrae el objeto de datos del usuario y actualiza la visualización del saldo.
        PlayerData datos = DatabaseManager.Instance.datosCargados;
        textoMonedasPrestigio.text = "Monedas Celestiales: " + datos.monedasPrestigio.ToString("F0");

        // Itera sobre cada mejora para refrescar su estado visual según el progreso del jugador.
        foreach (var mejora in todasLasMejoras)
        {
            mejora.RefrescarEstadoVisual(datos);
        }
    }

    // Gestiona la salida de la escena del árbol hacia el selector de niveles.
    public void VolverAlJuego()
    {
        // Persiste los cambios realizados en el árbol de mejoras en la base de datos remota.
        if (DatabaseManager.Instance != null)
        {
            Debug.Log("Guardando árbol de mejoras en Firebase antes de salir...");
            DatabaseManager.Instance.GuardarPartidaEnNube();
        }

        // Carga la escena de selección de niveles.
        UnityEngine.SceneManagement.SceneManager.LoadScene("Seleccion_Niveles");
    }
}