using UnityEngine;
using UnityEditor; // <--- Obligatorio
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class AutoLoginLoader
{
    static AutoLoginLoader()
    {
        // Suscribimos el método al evento de cambio de estado del editor
        EditorApplication.playModeStateChanged += AlCambiarEstadoPlay;
    }

    private static void AlCambiarEstadoPlay(PlayModeStateChange estado)
    {
        // "AboutToPlay" solo existe si el parámetro se llama 'estado' (o como le hayas puesto)
        if (estado == PlayModeStateChange.ExitingEditMode)
        {
            // Guarda la escena actual para no perder cambios antes de saltar al Login
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        if (estado == PlayModeStateChange.EnteredPlayMode)
        {
            // Si la escena actual no es la de índice 0 (MenuPrincipal/Login), la carga
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                SceneManager.LoadScene(0);
            }
        }
    }
}