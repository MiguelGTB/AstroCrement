using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class ReencarnacionManager : MonoBehaviour
{
    [Header("Configuración de Prestigio")]
    public float produccionPorSegundoActual; 
    public double requisitoMinimoDineroBase = 1000; 
    public float divisorPrestigio = 500f; 

    [Header("Monedas Especiales")]
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

        int indicePlanetaActual = 0;
        if (PartidaActual.MundoActual == "Luna") indicePlanetaActual = 0;
        else if (PartidaActual.MundoActual == "Marte") indicePlanetaActual = 1;
        else if (PartidaActual.MundoActual == "Europa") indicePlanetaActual = 2;
        else if (PartidaActual.MundoActual == "Titan") indicePlanetaActual = 3;
        else if (PartidaActual.MundoActual == "Kepler") indicePlanetaActual = 4;
        else if (PartidaActual.MundoActual == "Dyson") indicePlanetaActual = 5;
        else if (PartidaActual.MundoActual == "Colapso") indicePlanetaActual = 6;

        double requisitoEscalado = requisitoMinimoDineroBase * (indicePlanetaActual + 1);

        if (dineroQueTienesAhora >= requisitoEscalado)
        {
            // Tu cálculo original de monedas (con el bonus de Sabiduría Celestial si existe)
            double calculoMonedas = System.Math.Floor(dineroQueTienesAhora * 0.10d);
            
            if (DatabaseManager.Instance != null && DatabaseManager.Instance.datosCargados != null)
            {
                if (DatabaseManager.Instance.datosCargados.mejorasPrestigioCompradas.Contains("sabiduria_celestial"))
                {
                    calculoMonedas *= 1.50d; 
                }
            }

            monedasDePrestigioGanadas = System.Math.Floor(calculoMonedas);
            
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

        // 1. Añadir las monedas ganadas al monedero global
        datosGlobales.monedasPrestigio += monedasDePrestigioGanadas;

        // 2. CORRECCIÓN: Forzar el borrado tanto en el script de datos como en el EconomyManager activo
        if (planetaActual != null)
        {
            // Si tiene Ancla de Ascensión empieza con dinero, si no, a 0
            double dineroInicial = 0;
            if (datosGlobales.mejorasPrestigioCompradas.Contains("ancla_ascension"))
            {
                dineroInicial = 10000; 
            }

            // Limpiamos los datos del archivo de guardado
            planetaActual.dineroActual = dineroInicial;
            planetaActual.dineroTotal = dineroInicial;
            planetaActual.dineroPorClic = 1; 
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

            // --- ¡EL TRUCO EN TIEMPO REAL! ---
            // Si el EconomyManager de la escena está encendido, lo reseteamos a capón para que no vuelva a guardar los datos viejos encima
            if (DatabaseManager.Instance.economy != null)
            {
                DatabaseManager.Instance.economy.dineroActual = dineroInicial;
                DatabaseManager.Instance.economy.dineroTotal = dineroInicial;
                DatabaseManager.Instance.economy.dineroPorClic = 1;
                DatabaseManager.Instance.economy.dineroPorSeg = 0;
                
                if (DatabaseManager.Instance.economy.nivelesCompras != null)
                {
                    for (int i = 0; i < DatabaseManager.Instance.economy.nivelesCompras.Length; i++)
                        DatabaseManager.Instance.economy.nivelesCompras[i] = 0;
                }
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
        }

        // 4. Tu contador de reencarnaciones totales
        datosGlobales.totalReencarnaciones++;
        
        // 5. Guardar partida de forma inmediata en Firebase e ir a la escena del árbol
        DatabaseManager.Instance.GuardarPartidaEnNube();
        SceneManager.LoadScene("ArbolPrestigio");
    }
}