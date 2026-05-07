using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class TransicionVideo : MonoBehaviour
{
    public VideoPlayer reproductor;
    public string nombreEscenaNiveles = "Seleccion_Niveles";
    public GameObject panelComicUI;
    
    [Header("Botón a ocultar")]
    public GameObject botonSiguiente; // <--- NUEVA VARIABLE PARA TU BOTÓN

    public void IniciarCinematica()
    {
        Debug.Log("Botón pulsado: Apagando botón y cómic...");
        
        // 1. APAGAMOS EL BOTÓN INMEDIATAMENTE para evitar dobles clics
        if (botonSiguiente != null) botonSiguiente.SetActive(false);

        // 2. Apagamos el resto del cómic
        if (panelComicUI != null) panelComicUI.SetActive(false);

        // 3. Iniciamos el vídeo
        StartCoroutine(ReproducirYEsperar());
    }

    private IEnumerator ReproducirYEsperar()
    {
        reproductor.Prepare();
        
        while (!reproductor.isPrepared)
        {
            yield return null;
        }

        reproductor.Play();
        yield return new WaitForSeconds(0.5f);

        while (reproductor.isPlaying)
        {
            yield return null;
        }

        SceneManager.LoadScene(nombreEscenaNiveles);
    }
}