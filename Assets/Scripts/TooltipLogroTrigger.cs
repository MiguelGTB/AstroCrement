using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public enum TipoLogro
{
    PorDineroTotal,
    PorPlanetasDesbloqueados,
    PorCompras,
    PorMejoraComprada,
    PorMejoraProduccionComprada,
    PorDineroPorSegundo,
    PorTodasInstalaciones,
    PorReencarnaciones
}

public class TooltipLogroTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Configuración del Logro")]
    public string nombreLogro;
    [TextArea] public string descripcion;
    public TipoLogro tipoLogro;
    public double metaRequerida; 
    public int indiceInstalacion;

    void Start()
    {
        Image img = GetComponent<Image>();
        if (img != null) img.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        StartCoroutine(ComprobarConDelay());
    }

    IEnumerator ComprobarConDelay()
    {
        yield return new WaitForSeconds(0.5f);
        ComprobarEstadoVisual();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Image img = GetComponent<Image>();
        string textoFinal;

        // Primero comprobamos si ya está guardado como completado
        if (DatabaseManager.Instance != null && 
            DatabaseManager.Instance.datosCargados != null &&
            DatabaseManager.Instance.datosCargados.logrosCompletados != null &&
            DatabaseManager.Instance.datosCargados.logrosCompletados.Contains(nombreLogro))
        {
            textoFinal = "<color=#00FF00>¡LOGRO COMPLETADO!</color>";
            if (img != null) img.color = Color.white;
        }
        else
        {
            double actual = ObtenerValorActual();
            if (metaRequerida > 0 && actual >= metaRequerida)
            {
                textoFinal = "<color=#00FF00>¡LOGRO COMPLETADO!</color>";
                if (img != null) img.color = Color.white;
            }
            else
            {
                textoFinal = "Progreso: " + Formatear(actual) + " / " + Formatear(metaRequerida);
                if (img != null) img.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            }
        }

        if (TooltipLogrosManager.Instance != null)
            TooltipLogrosManager.Instance.Mostrar(nombreLogro, descripcion, textoFinal);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipLogrosManager.Instance != null)
            TooltipLogrosManager.Instance.Ocultar();
    }

    public void ComprobarEstadoVisual()
    {
        Image img = GetComponent<Image>();
        if (img == null) return;

        if (DatabaseManager.Instance == null || DatabaseManager.Instance.datosCargados == null)
            return;

        PlayerData datos = DatabaseManager.Instance.datosCargados;

        // Si ya está guardado como completado, lo marcamos directamente
        if (datos.logrosCompletados != null && datos.logrosCompletados.Contains(nombreLogro))
        {
            img.color = Color.white;
            return;
        }

        double actual = ObtenerValorActual();
        if (metaRequerida > 0 && actual >= metaRequerida)
        {
            if (datos.logrosCompletados == null)
                datos.logrosCompletados = new System.Collections.Generic.List<string>();
            
            datos.logrosCompletados.Add(nombreLogro);
            DatabaseManager.Instance.GuardarPartidaEnNube();
            img.color = Color.white;
        }
        else
        {
            img.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        }
    }

    private double ObtenerValorActual()
    {
        if (DatabaseManager.Instance == null || DatabaseManager.Instance.datosCargados == null)
            return 0;

        PlayerData datos = DatabaseManager.Instance.datosCargados;

        switch (tipoLogro)
        {
            case TipoLogro.PorDineroTotal:
                return datos.progresoLuna.dineroTotal + datos.progresoMarte.dineroTotal +
                       datos.progresoEuropa.dineroTotal + datos.progresoTitan.dineroTotal +
                       datos.progresoKepler.dineroTotal + datos.progresoDyson.dineroTotal +
                       datos.progresoColapso.dineroTotal;

            case TipoLogro.PorPlanetasDesbloqueados:
                return datos.planetasDesbloqueados;

            case TipoLogro.PorCompras:
                return SumarComprasGlobales(datos, indiceInstalacion);

            case TipoLogro.PorMejoraComprada:
                DatosPlaneta[] todosPlanetas = {
                    datos.progresoLuna, datos.progresoMarte, datos.progresoEuropa,
                    datos.progresoTitan, datos.progresoKepler, datos.progresoDyson, datos.progresoColapso
                };
                foreach (var planeta in todosPlanetas)
                {
                    if (planeta?.mejorasCompradas != null &&
                        planeta.mejorasCompradas.Length > indiceInstalacion &&
                        planeta.mejorasCompradas[indiceInstalacion] == true)
                        return 1;
                }
                return 0;

            case TipoLogro.PorMejoraProduccionComprada:
                DatosPlaneta[] todosPlanetas2 = {
                    datos.progresoLuna, datos.progresoMarte, datos.progresoEuropa,
                    datos.progresoTitan, datos.progresoKepler, datos.progresoDyson, datos.progresoColapso
                };
                foreach (var planeta in todosPlanetas2)
                {
                    if (planeta?.mejorasCompradas != null)
                    {
                        for (int i = 1; i < planeta.mejorasCompradas.Length; i++)
                        {
                            if (planeta.mejorasCompradas[i] == true)
                                return 1;
                        }
                    }
                }
                return 0;

            case TipoLogro.PorDineroPorSegundo:
                double totalDPS = 0;
                DatosPlaneta[] planetasDPS = {
                    datos.progresoLuna, datos.progresoMarte, datos.progresoEuropa,
                    datos.progresoTitan, datos.progresoKepler, datos.progresoDyson, datos.progresoColapso
                };
                foreach (var planeta in planetasDPS)
                {
                    if (planeta != null)
                        totalDPS += planeta.dineroPorSeg;
                }
                return totalDPS;

            case TipoLogro.PorTodasInstalaciones:
                DatosPlaneta[] planetasInst = {
                    datos.progresoLuna, datos.progresoMarte, datos.progresoEuropa,
                    datos.progresoTitan, datos.progresoKepler, datos.progresoDyson, datos.progresoColapso
                };
                for (int i = 1; i <= 14; i++)
                {
                    bool tieneEsta = false;
                    foreach (var planeta in planetasInst)
                    {
                        if (planeta?.nivelesCompras != null &&
                            planeta.nivelesCompras.Length > i &&
                            planeta.nivelesCompras[i] >= 1)
                        {
                            tieneEsta = true;
                            break;
                        }
                    }
                    if (!tieneEsta) return 0;
                }
                return 1;

            case TipoLogro.PorReencarnaciones:
                return datos.totalReencarnaciones;

            default:
                return 0;
        }
    }

    private double SumarComprasGlobales(PlayerData datos, int indice)
    {
        double total = 0;
        DatosPlaneta[] planetas = {
            datos.progresoLuna, datos.progresoMarte, datos.progresoEuropa,
            datos.progresoTitan, datos.progresoKepler, datos.progresoDyson, datos.progresoColapso
        };
        foreach (var p in planetas)
        {
            if (p?.nivelesCompras != null && p.nivelesCompras.Length > indice)
                total += p.nivelesCompras[indice];
        }
        return total;
    }

    string Formatear(double n)
    {
        if (n >= 1000000) return (n / 1000000).ToString("0.0") + "M";
        if (n >= 1000) return (n / 1000).ToString("0.0") + "K";
        return n.ToString("0");
    }
}