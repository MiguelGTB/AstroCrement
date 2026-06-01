using UnityEngine;
using UnityEngine.SceneManagement;

public class ComicManager : MonoBehaviour
{
    [Header("Configuración")]
    public string nombreEscenaJuego = "Nivel_Luna";

    public void EntrarAlJuego()
    {
        Debug.Log("Fin de la introducción. Entrando en órbita...");
        SceneManager.LoadScene(nombreEscenaJuego);
    }
}