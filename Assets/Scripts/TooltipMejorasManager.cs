using UnityEngine;
using TMPro;

public class TooltipMejorasManager : MonoBehaviour
{
    public static TooltipMejorasManager Instance;

    [Header("UI del Tooltip")]
    public GameObject panelTooltip;
    public TextMeshProUGUI txtCosto;
    public TextMeshProUGUI txtTiempo;

    [Header("Conexión con tu Script")]
    public EconomyManager economy; 
    
    private double costoActual;
    private bool visible = false;

    void Awake() 
    {
        Instance = this;
    }

    void Update()
    {
        if (visible)
        {
            // Hace que el panel persiga al ratón (con un poco de margen para que no lo tape el cursor)
            panelTooltip.transform.position = Input.mousePosition + new Vector3(15, -15, 0);
            
            // Calculamos el tiempo todo el rato para que la cuenta atrás se vea en vivo
            ActualizarCalculos();
        }
    }

    public void Mostrar(double costo) 
    {
        costoActual = costo;
        visible = true;
        panelTooltip.SetActive(true);
        ActualizarCalculos();
    }

    public void Ocultar()
    {
        visible = false;
        panelTooltip.SetActive(false);
    }

    void ActualizarCalculos()
    {
        // Ponemos el texto del costo
        txtCosto.text = "Coste: " + costoActual.ToString("N0") + " PE";

        // MIRA AQUÍ: Usamos tu variable 'dineroActual'
        if (economy.dineroActual >= costoActual)
        {
            txtTiempo.text = "<color=#00FF00>¡Disponible!</color>"; // Color verde
        }
        else if (economy.dineroPorSeg <= 0)
        {
            txtTiempo.text = "Sin producción"; // Si no ganamos nada por segundo, no podemos calcular
        }
        else
        {
            // LA FÓRMULA MÁGICA: (Costo - Lo que tengo) / Lo que gano por segundo
            double faltante = costoActual - economy.dineroActual;
            float segundos = (float)(faltante / economy.dineroPorSeg);
            
            txtTiempo.text = "Faltan: " + FormatearTiempo(segundos);
        }
    }

    // Esta función convierte los segundos en formato "05m 30s"
    string FormatearTiempo(float seg)
    {
        System.TimeSpan t = System.TimeSpan.FromSeconds(seg);
        if (t.TotalHours >= 1) 
            return string.Format("{0:D2}h {1:D2}m", (int)t.TotalHours, t.Minutes);
        
        return string.Format("{0:D2}m {1:D2}s", t.Minutes, t.Seconds);
    }
}