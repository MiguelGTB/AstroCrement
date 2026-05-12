using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TooltipLogroTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Configuración del Logro")]
    public string nombreLogro;
    [TextArea] public string descripcion;
    public double metaRequerida; 
    
    private EconomyManager economy;

    public void OnPointerEnter(PointerEventData eventData)
{
    if (economy == null) economy = FindObjectOfType<EconomyManager>();

    if (economy != null)
    {
        double actual = economy.dineroTotal;
        double meta = metaRequerida;
        string textoFinal = "";

        // 1. Buscamos la imagen de forma segura
        Image img = GetComponent<Image>();

        if (actual >= meta && meta > 0)
        {
            textoFinal = "<color=#00FF00>¡LOGRO COMPLETADO!</color>";
            // Solo intentamos cambiar el color si la imagen EXISTE
            if (img != null) img.color = Color.white; 
        }
        else
        {
            textoFinal = "Progreso: " + Formatear(actual) + " / " + Formatear(meta);
            // Solo intentamos cambiar el color si la imagen EXISTE
            if (img != null) img.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        }

        if (TooltipLogrosManager.Instance != null)
        {
            TooltipLogrosManager.Instance.Mostrar(nombreLogro, descripcion, textoFinal);
        }
    }
}

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipLogrosManager.Instance != null)
            TooltipLogrosManager.Instance.Ocultar();
    }

    public void ComprobarEstadoVisual()
{
    if (economy == null) economy = FindObjectOfType<EconomyManager>();
    Image img = GetComponent<Image>();

    if (economy != null && img != null)
    {
        // Si el dinero actual es mayor que la meta, se pone en color normal
        if (economy.dineroTotal >= metaRequerida && metaRequerida > 0)
        {
            img.color = Color.white;
        }
        else
        {
            // Si no, se queda en gris
            img.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        }
    }
}

    string Formatear(double n)
    {
        if (n >= 1000000) return (n / 1000000).ToString("0.0") + "M";
        if (n >= 1000) return (n / 1000).ToString("0.0") + "K";
        return n.ToString("0");
    }
}