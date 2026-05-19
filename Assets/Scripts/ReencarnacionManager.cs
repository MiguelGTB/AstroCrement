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

        // --- EL ARREGLO ESTÁ AQUÍ ---
        if (DatabaseManager.Instance != null)
        {
            // 1. Si estamos en la Luna, leemos el dinero de la pantalla en vivo
            if (DatabaseManager.Instance.economy != null)
            {
                dineroQueTienesAhora = DatabaseManager.Instance.economy.dineroActual;
            }
            // 2. Si estamos en una escena separada (como esta), leemos de la memoria inmortal
            else if (DatabaseManager.Instance.datosCargados != null)
            {
                dineroQueTienesAhora = DatabaseManager.Instance.datosCargados.dineroActual;
            }
        }

        // Comprobamos si superas la barrera
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

            // Damos el premio
            datosActuales.monedasPrestigio += monedasDePrestigioGanadas;
            
            // Reseteamos el dinero
            datosActuales.dineroActual = 0;
            datosActuales.dineroTotal = 0; 
            datosActuales.dineroPorClic = 1; 
            datosActuales.dineroPorSeg = 0;

            // Reseteamos los niveles
            if (datosActuales.nivelesCompras != null) 
            {
                for (int i = 0; i < datosActuales.nivelesCompras.Length; i++) 
                {
                    datosActuales.nivelesCompras[i] = 0;
                }
            }

            // Reseteamos las mejoras
            if (datosActuales.mejorasCompradas != null) 
            {
                for (int i = 0; i < datosActuales.mejorasCompradas.Length; i++) 
                {
                    datosActuales.mejorasCompradas[i] = false;
                }
            }

            Debug.Log("-----> 4. RESETEO TERMINADO. Guardando en nube y viajando...");
            
            // Activamos el escudo para no perder el reseteo
            DatabaseManager.Instance.enModoPrestigio = true; 
            DatabaseManager.Instance.GuardarPartidaEnNube();
            
            Debug.Log("-----> 5. VIAJANDO A LA ESCENA:");
            SceneManager.LoadScene("ArbolPrestigio"); 
        }
        else
        {
            Debug.LogWarning("-----> X. EL CÓDIGO SE PARA. ¡El juego detecta 0 monedas ganadas!");
        }
    }
}