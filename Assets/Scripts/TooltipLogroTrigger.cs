using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public enum TipoLogro
{
    PorDineroTotal,
    PorPlanetasDesbloqueados,
    PorCompras
}

public class TooltipLogroTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Configuración del Logro")]
    public string nombreLogro;
    [TextArea] public string descripcion;
    public TipoLogro tipoLogro;
    public double metaRequerida; 
    
    // Solo necesario si tipoLogro == PorCompras
    public int indiceInstalacion;

    void Start()
    {
        Image img = GetComponent<Image>();
        if (img != null) img.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        StartCoroutine(ComprobarConDelay());
    }

    IEnumerator ComprobarConDelay()
    {
        // Esperamos un momento para asegurarnos de que los datos se han cargado
        yield return new WaitForSeconds(0.5f);
        ComprobarEstadoVisual();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        double actual = ObtenerValorActual();
        string textoFinal;
        Image img = GetComponent<Image>();

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

        double actual = ObtenerValorActual();
        if (metaRequerida > 0 && actual >= metaRequerida)
            img.color = Color.white;
        else
            img.color = new Color(0.2f, 0.2f, 0.2f, 1f);
    }

    private double ObtenerValorActual()
    {
        if (DatabaseManager.Instance == null || DatabaseManager.Instance.datosCargados == null)
            return 0;

        PlayerData datos = DatabaseManager.Instance.datosCargados;

        switch (tipoLogro)
        {
            case TipoLogro.PorDineroTotal:
                // Suma el dineroTotal de todos los planetas de la partida
                return datos.progresoLuna.dineroTotal + datos.progresoMarte.dineroTotal +
                       datos.progresoEuropa.dineroTotal + datos.progresoTitan.dineroTotal +
                       datos.progresoKepler.dineroTotal + datos.progresoDyson.dineroTotal +
                       datos.progresoColapso.dineroTotal;
            case TipoLogro.PorPlanetasDesbloqueados:
                return datos.planetasDesbloqueados;
            
            case TipoLogro.PorCompras:
                return SumarComprasGlobales(datos, indiceInstalacion);
            
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