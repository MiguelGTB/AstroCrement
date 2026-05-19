using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Necesario para usar Botones

public class ReencarnacionManager : MonoBehaviour
{
    [Header("Configuración de Prestigio")]
    public float produccionPorSegundoActual; 
    public float requisitoMinimoPeSeg = 1000f; 
    public float divisorPrestigio = 500f; 

    [Header("Monedas Especiales")]
    public int monedasDePrestigioGanadas; 

    [Header("Interfaz (UI)")]
    public Button botonReencarnacion; // Arrastra tu botón aquí
    public GameObject iconoCandado;   // Arrastra la imagen del candado aquí

    void Update()
    {
        // 1. Conseguimos el DINERO ACTUAL (en lugar de la producción por segundo)
        double dineroQueTienesAhora = 0;
        if (DatabaseManager.Instance != null && DatabaseManager.Instance.economy != null)
        {
            dineroQueTienesAhora = DatabaseManager.Instance.economy.dineroActual;
        }

        // 2. Comprobamos si tienes el dinero mínimo para poder reencarnar
        // (Nota: Asegúrate de que 'requisitoMinimoPeSeg' en el Inspector ahora sea una cantidad lógica de dinero, no de PE/s)
        if (dineroQueTienesAhora >= requisitoMinimoPeSeg)
        {
            // LA NUEVA FÓRMULA MÁGICA: Calculamos el 10% del dinero actual
            monedasDePrestigioGanadas = Mathf.FloorToInt((float)dineroQueTienesAhora * 0.10f);
            
            // ¡Desbloqueamos el botón visualmente!
            if (botonReencarnacion != null) botonReencarnacion.interactable = true;
            if (iconoCandado != null) iconoCandado.SetActive(false); 
        }
        else
        {
            monedasDePrestigioGanadas = 0;
            
            // Lo mantenemos bloqueado
            if (botonReencarnacion != null) botonReencarnacion.interactable = false;
            if (iconoCandado != null) iconoCandado.SetActive(true); 
        }
    }

    // Este es el método que ejecuta el clic
    public void EjecutarReencarnacion()
    {
        // CHIVATO 1: Comprueba si el botón realmente está conectado al código
        Debug.Log("-----> 1. EL BOTÓN FUNCIONA Y HA ENTRADO AL CÓDIGO");

        if (monedasDePrestigioGanadas > 0)
        {
            // CHIVATO 2: Comprueba si pasas la barrera del dinero
            Debug.Log("-----> 2. TIENES SUFICIENTES MONEDAS: " + monedasDePrestigioGanadas);

            // CHIVATO 3: Comprueba si tu cerebro (Base de datos) existe
            if (DatabaseManager.Instance == null || DatabaseManager.Instance.datosCargados == null)
            {
                Debug.LogError("-----> ERROR CRÍTICO: La base de datos no está cargada. Por eso se congela.");
                return; // Cortamos la ejecución aquí para que no de error rojo
            }

            Debug.Log("-----> 3. DATOS CARGADOS. Empezando a resetear niveles...");
            PlayerData datosActuales = DatabaseManager.Instance.datosCargados;

            datosActuales.monedasPrestigio += monedasDePrestigioGanadas;
            datosActuales.dineroActual = 0;
            datosActuales.dineroTotal = 0; 
            datosActuales.dineroPorClic = 0; 
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
            DatabaseManager.Instance.GuardarPartidaEnNube();
            
            // CHIVATO FINAL
            Debug.Log("-----> 5. VIAJANDO A LA ESCENA:");
            SceneManager.LoadScene("ArbolPrestigio"); 
        }
        else
        {
            // CHIVATO ALTERNATIVO: Si no tienes dinero
            Debug.LogWarning("-----> X. EL CÓDIGO SE PARA AQUÍ PORQUE TUS MONEDAS GANADAS SON 0.");
        }
    }
}