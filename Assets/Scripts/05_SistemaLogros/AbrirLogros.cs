using UnityEngine;
using UnityEngine.SceneManagement;

public class AbrirLogros : MonoBehaviour
{
    // Ejecuta la transición hacia la escena de logros.
    public void ClickEnBoton()
    {
        // Almacena la escena actual para permitir el retorno mediante la clase PartidaActual.
        PartidaActual.EscenaAnterior = SceneManager.GetActiveScene().name;
        
        // Registra en consola el estado actual de la navegación.
        Debug.Log("Escena anterior registrada: " + PartidaActual.EscenaAnterior);
        
        // Realiza la carga de la escena destino.
        SceneManager.LoadScene("Logros");
    }
}