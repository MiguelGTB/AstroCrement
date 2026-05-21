using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class ReencarnacionManager : MonoBehaviour
{
    [Header("Configuración de Prestigio")]
    public float produccionPorSegundoActual; 
    public double requisitoMinimoDineroBase = 1000; // Cambiado a double para evitar la multiplicación infinita
    public float divisorPrestigio = 500f; 

    [Header("Monedas Especiales")]
    // CORRECCIÓN VITAL: Cambiado de 'int' a 'double' para soportar números gigantescos sin reventar el Int32
    public double monedasDePrestigioGanadas; 

    [Header("Interfaz (UI)")]
    public Button botonReencarnacion; 
    public GameObject iconoCandado;   

    void Update()
    {
        double dineroQueTienesAhora = 0;

        if (DatabaseManager.Instance != null)
        {
            if (DatabaseManager.Instance.economy != null)
                dineroQueTienesAhora = DatabaseManager.Instance.economy.dineroActual;
            else if (DatabaseManager.Instance.datosCargados != null)
                dineroQueTienesAhora = DatabaseManager.Instance.ObtenerDatosPlanetaActual().dineroActual;
        }

        // 1. Calcular el índice del planeta actual de forma segura
        int indicePlanetaActual = 0;
        if (PartidaActual.MundoActual == "Luna") indicePlanetaActual = 0;
        else if (PartidaActual.MundoActual == "Marte") indicePlanetaActual = 1;
        else if (PartidaActual.MundoActual == "Europa") indicePlanetaActual = 2;
        else if (PartidaActual.MundoActual == "Titan") indicePlanetaActual = 3;
        else if (PartidaActual.MundoActual == "Kepler") indicePlanetaActual = 4;
        else if (PartidaActual.MundoActual == "Dyson") indicePlanetaActual = 5;
        else if (PartidaActual.MundoActual == "Colapso") indicePlanetaActual = 6;

        // 2. Calculamos el requisito escalado en una variable temporal por frame
        double requisitoEscalado = requisitoMinimoDineroBase * (indicePlanetaActual + 1);

        // 3. Comprobamos la meta de forma segura usando operaciones puras de 'double'
        if (dineroQueTienesAhora >= requisitoEscalado)
        {
            // Usamos System.Math.Floor directamente guardándolo en un double. ¡Adiós al OverflowException!
            monedasDePrestigioGanadas = System.Math.Floor(dineroQueTienesAhora * 0.10d);
            
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
        if (DatabaseManager.Instance == null || DatabaseManager.Instance.datosCargados == null) return;

        PlayerData datosGlobales = DatabaseManager.Instance.datosCargados;
        DatosPlaneta planetaActual = DatabaseManager.Instance.ObtenerDatosPlanetaActual();

        // 1. Añadir las monedas ganadas (ahora double) al monedero global que también es double en PlayerData
        datosGlobales.monedasPrestigio += monedasDePrestigioGanadas;

        // 2. Resetear el progreso ÚNICAMENTE del planeta en el que estamos actualmente
        if (planetaActual != null)
        {
            planetaActual.dineroActual = 0;
            planetaActual.dineroTotal = 0;
            planetaActual.dineroPorClic = 1; // Volver al clic base de nivel
            planetaActual.dineroPorSeg = 0;

            if (planetaActual.nivelesCompras != null)
            {
                for (int i = 0; i < planetaActual.nivelesCompras.Length; i++)
                    planetaActual.nivelesCompras[i] = 0;
            }

            if (planetaActual.mejorasCompradas != null)
            {
                for (int i = 0; i < planetaActual.mejorasCompradas.Length; i++)
                    planetaActual.mejorasCompradas[i] = false;
            }
        }

        // 3. Lógica de desbloqueo del siguiente planeta
        int indicePlanetaActual = 0;
        if (PartidaActual.MundoActual == "Luna") indicePlanetaActual = 0;
        else if (PartidaActual.MundoActual == "Marte") indicePlanetaActual = 1;
        else if (PartidaActual.MundoActual == "Europa") indicePlanetaActual = 2;
        else if (PartidaActual.MundoActual == "Titan") indicePlanetaActual = 3;
        else if (PartidaActual.MundoActual == "Kepler") indicePlanetaActual = 4;
        else if (PartidaActual.MundoActual == "Dyson") indicePlanetaActual = 5;
        else if (PartidaActual.MundoActual == "Colapso") indicePlanetaActual = 6;

        if (datosGlobales.planetasDesbloqueados == indicePlanetaActual)
        {
            datosGlobales.planetasDesbloqueados++;
            Debug.Log("¡Siguiente planeta desbloqueado! Total: " + datosGlobales.planetasDesbloqueados);
        }

        // 4. Tu contador de reencarnaciones totales para los logros
        datosGlobales.totalReencarnaciones++;
        
        // 5. Guardar partida e ir a la escena del árbol
        DatabaseManager.Instance.GuardarPartidaEnNube();
        SceneManager.LoadScene("ArbolPrestigio");
    }
}