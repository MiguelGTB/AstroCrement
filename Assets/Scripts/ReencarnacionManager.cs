using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class ReencarnacionManager : MonoBehaviour
{
    [Header("Configuración de Prestigio")]
    public float produccionPorSegundoActual; 
    public float requisitoMinimoPeSeg = 1000f; 
    public float divisorPrestigio = 500f; 

    [Header("Monedas Especiales")]
    public int monedasDePrestigioGanadas; 

    [Header("Interfaz (UI)")]
    public Button botonReencarnacion; 
    public GameObject iconoCandado;   

    void Update()
    {
        double dineroQueTienesAhora = 0;
        if (DatabaseManager.Instance != null && DatabaseManager.Instance.economy != null)
        {
            dineroQueTienesAhora = DatabaseManager.Instance.economy.dineroActual;
        }

        if (dineroQueTienesAhora >= requisitoMinimoPeSeg)
        {
            monedasDePrestigioGanadas = Mathf.FloorToInt((float)dineroQueTienesAhora * 0.10f);
            
            if (botonReencarnacion != null) botonReencarnacion.interactable = true;
            if (iconoCandado != null) iconoCandado.SetActive(false); 
        }
        else
        {
            monedasDePrestigioGanadas = 0;
            
            if (botonReencarnacion != null) botonReencarnacion.interactable = false;
            if (iconoCandado != null) iconoCandado.SetActive(true); 
        }
    }

    public void EjecutarReencarnacion()
    {
        Debug.Log("-----> 1. EL BOTÓN FUNCIONA Y HA ENTRADO AL CÓDIGO");

        if (monedasDePrestigioGanadas > 0)
        {
            Debug.Log("-----> 2. TIENES SUFICIENTES MONEDAS: " + monedasDePrestigioGanadas);

            if (DatabaseManager.Instance == null || DatabaseManager.Instance.datosCargados == null)
            {
                Debug.LogError("-----> ERROR CRÍTICO: La base de datos no está cargada.");
                return; 
            }

            Debug.Log("-----> 3. DATOS CARGADOS. Empezando a resetear niveles...");
            PlayerData datosActuales = DatabaseManager.Instance.datosCargados;

            datosActuales.monedasPrestigio += monedasDePrestigioGanadas;
            datosActuales.dineroActual = 0;
            datosActuales.dineroTotal = 0; 
            datosActuales.dineroPorClic = 1; 
            datosActuales.dineroPorSeg = 0;

            if (datosActuales.nivelesCompras != null) 
            {
                for (int i = 0; i < datosActuales.nivelesCompras.Length; i++) 
                {
                    datosActuales.nivelesCompras[i] = 0;
                }
            }

            if (datosActuales.mejorasCompradas != null) 
            {
                for (int i = 0; i < datosActuales.mejorasCompradas.Length; i++) 
                {
                    datosActuales.mejorasCompradas[i] = false;
                }
            }

            Debug.Log("-----> 4. RESETEO TERMINADO. Guardando en nube y viajando...");
            
            // ---> ORDENAMOS A LA BASE DE DATOS QUE ACTIVE EL MODO PRESTIGIO <---
            DatabaseManager.Instance.enModoPrestigio = true; 
            DatabaseManager.Instance.GuardarPartidaEnNube();
            
            Debug.Log("-----> 5. VIAJANDO A LA ESCENA:");
            SceneManager.LoadScene("ArbolPrestigio"); 
        }
        else
        {
            Debug.LogWarning("-----> X. EL CÓDIGO SE PARA AQUÍ PORQUE TUS MONEDAS GANADAS SON 0.");
        }
    }
}