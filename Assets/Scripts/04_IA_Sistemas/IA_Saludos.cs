using UnityEngine;
using System.Collections;

public class IA_Saludos : MonoBehaviour
{
    [TextArea]
    public string fraseDeBienvenida;

    void Start()
    {
        // En lugar de saludar a lo loco, arranco una rutina para pensar primero si debo hablar o callar.
        StartCoroutine(EsperarYDecidirSaludo());
    }

    private IEnumerator EsperarYDecidirSaludo()
    {
        // Esperamos medio segundo para que la escena cargue suave y Firebase traiga los datos.
        yield return new WaitForSeconds(0.5f);

        // Me aseguro de que el cerebro de datos existe y ya tiene mis datos cargados.
        if (DatabaseManager.Instance == null || DatabaseManager.Instance.datosCargados == null)
        {
            Debug.LogWarning("IA_Saludos: Base de datos no lista. Abortando saludo.");
            yield break; 
        }

        // Recojo quién soy y dónde estoy
        string slot = PartidaActual.SlotSeleccionado;
        string mundo = PartidaActual.MundoActual;
        int reencarnaciones = DatabaseManager.Instance.datosCargados.totalReencarnaciones;


        // Si el jugador reencarna, el número del final cambiará a 1, creando una llave nueva automáticamente.
        string claveVisita = "Visitado_" + slot + "_" + mundo + "_" + reencarnaciones;

        // Le pregunto al disco duro: ¿Ya hemos estado aquí en esta vida? (0 es no, 1 es sí)
        if (PlayerPrefs.GetInt(claveVisita, 0) == 0)
        {
            // Como es la primera vez, hablo.
            if (TextToSpeechManager.Instance != null)
            {
                TextToSpeechManager.Instance.Hablar(fraseDeBienvenida);
                
                // Guardo un 1 en el disco duro para recordar que ya he dado este discurso.
                PlayerPrefs.SetInt(claveVisita, 1);
                PlayerPrefs.Save();
                
                Debug.Log("IA: Dando el discurso de bienvenida por primera vez en esta vida.");
            }
        }
        else
        {
            // Ya estuve aquí, así que me callo para no ser pesado.
            Debug.Log("IA: Ya saludé a este jugador en esta reencarnación. Mantengo silencio.");
        }
    }
}