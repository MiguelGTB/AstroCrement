using UnityEngine;
using UnityEngine.SceneManagement; // ¡Vital para cambiar de pantalla!

public class MainMenuManager : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    
    public string nombreEscenaJuego = "Nivel_Luna"; 

    public void NuevaPartida()
    {
        // Al darle a Nueva Partida, borramos cualquier progreso anterior
        // (Descomenta la siguiente línea cuando tengáis el sistema de guardado)
        // PlayerPrefs.DeleteAll(); 
        
        Debug.Log("Iniciando nueva partida...");
        SceneManager.LoadScene(nombreEscenaJuego);
    }

    public void CargarPartida()
    {
        // Por ahora solo carga la escena. Más adelante, el GameManager 
        // leerá los datos guardados al entrar a la escena.
        Debug.Log("Cargando partida guardada...");
        SceneManager.LoadScene(nombreEscenaJuego);
    }

    public void RankingGlobal()
    {
        // Aquí activaremos un panel (GameObject.SetActive(true)) que tape 
        // el menú y muestre los datos traídos de una base de datos (Firebase/MySQL).
        Debug.Log("Abriendo Ranking Global...");
    }

    public void CerrarJuego()
    {
        Debug.Log("Cerrando los sistemas de la nave...");
        
        // Esto cierra el juego cuando está exportado en PC/Android
        Application.Quit();

        // Este bloque de código hace que también se pare la ejecución cuando le dais 
        // al "Play" dentro del propio editor de Unity, ¡súper cómodo para testear!
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}