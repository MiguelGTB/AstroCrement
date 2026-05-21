using UnityEngine;
using UnityEngine.SceneManagement;

public class AtajoLogros : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            PartidaActual.EscenaAnterior = SceneManager.GetActiveScene().name;
            Debug.Log("EscenaAnterior guardada: " + PartidaActual.EscenaAnterior);
            SceneManager.LoadScene("Logros");
        }
    }
}