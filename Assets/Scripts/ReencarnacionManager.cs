using UnityEngine;
using UnityEngine.SceneManagement; 

public class ReencarnacionManager : MonoBehaviour
{
    [Header("Configuración de Prestigio")]
    public float produccionPorSegundoActual; 
    public float requisitoMinimoPeSeg = 1000f; 
    public float divisorPrestigio = 500f; 

    [Header("Monedas Especiales")]
    public int monedasDePrestigioGanadas; 

    void Update()
    {
        // Calculamos constantemente cuántas monedas ganarías si te reencarnas AHORA
        if (produccionPorSegundoActual >= requisitoMinimoPeSeg)
        {
            monedasDePrestigioGanadas = Mathf.FloorToInt(produccionPorSegundoActual / divisorPrestigio);
        }
        else
        {
            monedasDePrestigioGanadas = 0;
        }
    }

    // Este es el método que pondrás en tu botón de "Reencarnarse" en el juego
    public void EjecutarReencarnacion()
    {
        if (monedasDePrestigioGanadas > 0)
        {
            // 1. Cargamos los datos actuales de nuestro "Cerebro" global
            PlayerData datosActuales = DatabaseManager.Instance.datosCargados;

            // 2. Sumamos las monedas celestiales (Esto NUNCA se borra)
            datosActuales.monedasPrestigio += monedasDePrestigioGanadas;

            // 3. RESETEAMOS LA PARTIDA NORMAL (Dinero a 0)
            datosActuales.dineroActual = 0;
            datosActuales.dineroTotal = 0; 
            datosActuales.dineroPorClic = 0; // Pon un 1 si empiezas ganando 1 por clic
            datosActuales.dineroPorSeg = 0;

            // Reseteamos los niveles de los edificios/planetas a 0
            if (datosActuales.nivelesCompras != null) 
            {
                for (int i = 0; i < datosActuales.nivelesCompras.Length; i++) 
                {
                    datosActuales.nivelesCompras[i] = 0;
                }
            }

            // Reseteamos las mejoras normales para que vuelvan a estar bloqueadas
            if (datosActuales.mejorasCompradas != null) 
            {
                for (int i = 0; i < datosActuales.mejorasCompradas.Length; i++) 
                {
                    datosActuales.mejorasCompradas[i] = false;
                }
            }

            // 4. GUARDAMOS EN FIREBASE (Usando tu función exacta)
            DatabaseManager.Instance.GuardarPartidaEnNube();

            // 5. Viajamos a la escena del Árbol de Habilidades
            SceneManager.LoadScene("ArbolPrestigio"); 
        }
        else
        {
            Debug.Log("Aún no tienes suficiente pe/seg para reencarnar.");
        }
    }
}