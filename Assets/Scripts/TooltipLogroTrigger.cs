using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipLogroTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Configuración del Logro")]
    public string nombreLogro;
    [TextArea] public string descripcion;
    public double metaRequerida; // Cuánto dinero/PE se necesita

    // Referencia al economy para saber cuánto tenemos
    private EconomyManager economy;

    void Start()
    {
        // Buscamos el economy manager en la escena
        economy = FindObjectOfType<EconomyManager>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        string textoProgreso = "";

        if (economy != null)
        {
            // Usamos dineroTotal para que el progreso sea acumulativo y no baje al comprar
            double actual = economy.dineroTotal;
            double meta = metaRequerida;

            if (actual >= meta)
            {
                // Formato cuando ya lo ha conseguido
                textoProgreso = "<color=#00FF00>¡LOGRO COMPLETADO!</color>";
            }
            else
            {
                // Formato de progreso: 100.000 / 1.000.000
                // Usamos el formateador para que los números grandes sean legibles
                textoProgreso = "Progreso: " + FormatearNumero(actual) + " / " + FormatearNumero(meta);
            }
        }

        TooltipLogrosManager.Instance.Mostrar(nombreLogro, descripcion, textoProgreso);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipLogrosManager.Instance.Ocultar();
    }

    // Función auxiliar para que los números grandes se vean bien (mil, millón...)
    string FormatearNumero(double n)
    {
        if (n >= 1000000000000) return (n / 1000000000000).ToString("F2") + "T"; // Trillones
        if (n >= 1000000000) return (n / 1000000000).ToString("F2") + "B";    // Billones
        if (n >= 1000000) return (n / 1000000).ToString("F2") + "M";       // Millones
        if (n >= 1000) return (n / 1000).ToString("F1") + "K";          // Miles
        return n.ToString("N0"); // Números normales
    }
}