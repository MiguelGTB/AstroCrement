using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectionManager : MonoBehaviour
{
    [Header("Configuración de Planetas")]
    // Arrastra aquí los 8 botones de los planetas
    public Button[] botonesPlanetas;
    
    // Arrastra aquí las imágenes de los candados que van sobre los planetas
    public GameObject[] iconosCandado;
    
    // Escribe aquí los nombres exactos de las escenas ("Nivel_Luna", "Nivel_Marte"...)
    public string[] nombresEscenasPlanetas; 

    void Start()
    {
        ActualizarEstadoPlanetas();
    }

    public void ActualizarEstadoPlanetas()
    {
        // PlayerPrefs guarda información en el disco duro. 
        // Buscamos la variable "PlanetaActual". Si no existe (jugador nuevo), devuelve 0.
        // 0 = Luna, 1 = Marte, 2 = Europa...
        int nivelMaximoDesbloqueado = PlayerPrefs.GetInt("PlanetaActual", 0);

        for (int i = 0; i < botonesPlanetas.Length; i++)
        {
            if (iconosCandado[i] == null) continue;

            if (i <= nivelMaximoDesbloqueado)
            {
                // -- PLANETA DESBLOQUEADO --
                botonesPlanetas[i].interactable = true;
                botonesPlanetas[i].image.color = Color.white; // Color normal (sin sombra)
                
                // Ocultamos el candado si existe
                if (iconosCandado.Length > i && iconosCandado[i] != null)
                {
                    iconosCandado[i].SetActive(false); 
                }
            }
            else
            {
                // -- PLANETA BLOQUEADO --
                botonesPlanetas[i].interactable = false;
                botonesPlanetas[i].image.color = new Color(0.3f, 0.3f, 0.3f, 1f); // Gris oscuro / Sombreado
                
                // Mostramos el candado si existe
                if (iconosCandado.Length > i && iconosCandado[i] != null)
                {
                    iconosCandado[i].SetActive(true); 
                }
            }
        }
    }

    // Esta es la función que ejecutarán los botones al hacerles clic
    public void ViajarAlPlaneta(int indicePlaneta)
    {
        // Comprobación de seguridad
        if (indicePlaneta < nombresEscenasPlanetas.Length)
        {
            Debug.Log("Iniciando salto hiperespacial a: " + nombresEscenasPlanetas[indicePlaneta]);
            SceneManager.LoadScene(nombresEscenasPlanetas[indicePlaneta]);
        }
    }
}