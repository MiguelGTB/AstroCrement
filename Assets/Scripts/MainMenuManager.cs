using UnityEngine;
using UnityEngine.SceneManagement; // ¡Vital para cambiar de pantalla!
using Firebase.Auth;

public class MainMenuManager : MonoBehaviour
{

    
    [Header("Configuración de Escenas")]
    public string nombreEscenaComic = "IntroJuego"; // Nueva variable para el cómic
    public string nombreEscenaJuego = "Nivel_Luna"; 

    public void NuevaPartida()
    {
        // Al darle a Nueva Partida, borramos cualquier progreso anterior
        // (Descomenta la siguiente línea cuando tengáis el sistema de guardado)
        // PlayerPrefs.DeleteAll(); 
        
        Debug.Log("Iniciando nueva partida...");
        SceneManager.LoadScene(nombreEscenaComic);
    }

    public void CargarPartida()
    {
        // Mostrar el selector de slots en el menú principal si existe.
        GestorSlots gestor = null;
        GestorSlots[] gestores = Resources.FindObjectsOfTypeAll<GestorSlots>();
        if (gestores.Length > 0)
        {
            gestor = gestores[0];
        }

        if (gestor != null)
        {
            Debug.Log("Abriendo Panel_SelectorPartidas...");
            gestor.InicializarSelector();
            return;
        }

        // Si no existe selector, mantenemos el comportamiento antiguo.
        Debug.LogWarning("No se encontró GestorSlots en la escena. Cargando juego directo...");
        SceneManager.LoadScene(nombreEscenaJuego);
    }

    public void CerrarSesion()
    {
        if (FirebaseAuth.DefaultInstance != null)
        {
            FirebaseAuth.DefaultInstance.SignOut();
            Debug.Log("Sesión cerrada correctamente.");
        }
        else
        {
            Debug.LogWarning("FirebaseAuth no está inicializado al cerrar sesión.");
        }
        SceneManager.LoadScene("MenuLogin");
    }

    public void RankingGlobal()
    {
        // Aquí activaremos un panel (GameObject.SetActive(true)) que tape 
        // el menú y muestre los datos traídos de una base de datos (Firebase/MySQL).
        Debug.Log("Abriendo Ranking Global...");
        SceneManager.LoadScene("RankingGlobal");
    }

    public void Ajustes()
    {
        SceneManager.LoadScene("Ajustes");
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