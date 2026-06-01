using UnityEngine;
using UnityEngine.SceneManagement; 
using Firebase.Auth;

public class MainMenuManager : MonoBehaviour
{

    
    [Header("Configuración de Escenas")]
    public string nombreEscenaComic = "IntroJuego";
    public string nombreEscenaJuego = "Nivel_Luna"; 

    public void NuevaPartida()
    {
        // Al darle a Nueva Partida, borramos cualquier progreso anterior

        
        Debug.Log("Iniciando nueva partida...");
        SceneManager.LoadScene(nombreEscenaComic);
    }

    public void CargarPartida()
    {
        // Mostrar el selector de slots en el menú principal si existe.
        GestorSlots gestor = Resources.FindObjectsOfTypeAll<GestorSlots>().Length > 0 
            ? Resources.FindObjectsOfTypeAll<GestorSlots>()[0] 
            : null;

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
        // el menú y muestre los datos traídos de una base de datos
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

        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

}