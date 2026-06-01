using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    [Header("Conexiones UI")]
    public GameObject panelTooltip;
    public TextMeshProUGUI textoTitulo;
    public TextMeshProUGUI textoDescripcion;
    public TextMeshProUGUI textoCoste;

    [Header("Ajustes")]
    public Vector2 desplazamientoMouse = new Vector2(15f, -15f); // Para que no tape el cursor

    void Awake()
    {
        Instance = this;
        // Apagamos el panel al arrancar el juego
        if (panelTooltip != null) 
        {
            panelTooltip.SetActive(false); 
        }
    }

    void Update()
    {
        // Si el panel está encendido, que siga la posición del ratón
        if (panelTooltip != null && panelTooltip.activeSelf)
        {
            transform.position = (Vector2)Input.mousePosition + desplazamientoMouse;
        }
    }

    // Esta función la llamarán los botones cuando pases el ratón por encima
    public void Mostrar(string titulo, string descripcion, int coste)
    {
        if (textoTitulo != null) textoTitulo.text = titulo;
        if (textoDescripcion != null) textoDescripcion.text = descripcion;
        if (textoCoste != null) textoCoste.text = "Coste: " + coste + " Monedas";
        
        if (panelTooltip != null) panelTooltip.SetActive(true);
    }

    // Esta función la llamarán los botones cuando quites el ratón
    public void Ocultar()
    {
        if (panelTooltip != null) panelTooltip.SetActive(false);
    }
}