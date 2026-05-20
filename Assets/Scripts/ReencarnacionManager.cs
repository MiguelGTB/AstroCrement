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

        if (DatabaseManager.Instance != null)
        {
            if (DatabaseManager.Instance.economy != null)
                dineroQueTienesAhora = DatabaseManager.Instance.economy.dineroActual;
            else if (DatabaseManager.Instance.datosCargados != null)
                dineroQueTienesAhora = DatabaseManager.Instance.ObtenerDatosPlanetaActual().dineroActual;
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
        if (monedasDePrestigioGanadas > 0)
        {
            if (DatabaseManager.Instance == null || DatabaseManager.Instance.datosCargados == null) return; 

            PlayerData datosGlobales = DatabaseManager.Instance.datosCargados;
            DatosPlaneta planetaActual = DatabaseManager.Instance.ObtenerDatosPlanetaActual(); // Solo reiniciamos este

            // 1. Damos la recompensa global a la cuenta
            datosGlobales.monedasPrestigio += monedasDePrestigioGanadas;
            
            // 2. Reseteamos SOLO la economía del planeta actual
            planetaActual.dineroActual = 0;
            planetaActual.dineroTotal = 0; 
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

            // 3. Logica de Desbloqueo del siguiente planeta
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
                Debug.Log("¡Planeta " + datosGlobales.planetasDesbloqueados + " Desbloqueado!");
            }

            // 4. Guardamos y viajamos
            DatabaseManager.Instance.enModoPrestigio = true; 
            DatabaseManager.Instance.GuardarPartidaEnNube();
            SceneManager.LoadScene("ArbolPrestigio"); 
        }
    }
}