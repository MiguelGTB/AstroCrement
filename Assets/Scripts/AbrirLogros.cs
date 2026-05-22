using UnityEngine;
using UnityEngine.SceneManagement;

public class AbrirLogros : MonoBehaviour
{
    public void ClickEnBoton()
    {
        PartidaActual.EscenaAnterior = SceneManager.GetActiveScene().name;
        Debug.Log("EscenaAnterior guardada: " + PartidaActual.EscenaAnterior);
        SceneManager.LoadScene("Logros");
    }
}
