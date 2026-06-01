using UnityEngine;
using UnityEngine.SceneManagement;

public class AtajoLogros : MonoBehaviour
{
    // Escucha eventos de teclado en cada frame para detectar atajos de teclado.
    void Update()
    {
        // Detecta la pulsación de la tecla 'L' para activar el acceso rápido a logros.
        if (Input.GetKeyDown(KeyCode.L))
        {
            // Registra la escena actual en el sistema de navegación global antes de cambiar de estado.
            PartidaActual.EscenaAnterior = SceneManager.GetActiveScene().name;
            
            // Registro de consola para confirmar la persistencia de la escena de origen.
            Debug.Log("Escena anterior registrada: " + PartidaActual.EscenaAnterior);
            
            // Ejecuta la carga de la escena de logros.
            SceneManager.LoadScene("Logros");
        }
    }
}