using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MejoraPrestigio : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Configuración de la Mejora")]
    public string idMejora;      
    public double precio;        
    public string descripcion;   

    [Header("Dependencias")]
    public MejoraPrestigio mejoraRequisito; 
    public Image lineaConexion;             

    [Header("Estado")]
    public bool comprada = false;
    public bool desbloqueada = false;

    private Button boton;
    private Image imagenFondo;

    [Header("Datos para el Tooltip")]
    public string nombreDeLaMejora;
    [TextArea] public string descripcionDeLaMejora;

    void Awake()
    {
        boton = GetComponent<Button>();
        imagenFondo = GetComponent<Image>();
    }

    public void IntentarComprar()
    {
        PlayerData datos = DatabaseManager.Instance.datosCargados;

        // Si ya la compré o todavía está bloqueada, aborto la operación inmediatamente.
        if (comprada || !desbloqueada) return;

        // Compruebo si tengo suficientes monedas de prestigio en mi cuenta global.
        if (datos.monedasPrestigio >= precio)
        {
            datos.monedasPrestigio -= precio;
            datos.mejorasPrestigioCompradas.Add(idMejora);
            comprada = true;
            
            // Refresco todo el árbol de inmediato para que se actualicen los colores y los candados.
            ArbolManager arbol = FindObjectOfType<ArbolManager>();
            if (arbol != null) arbol.ActualizarTodoElArbol();
        }
    }

    public void RefrescarEstadoVisual(PlayerData datos)
    {
        // 1. Reviso en mi base de datos si esta mejora concreta ya la tengo en propiedad.
        if (datos.mejorasPrestigioCompradas.Contains(idMejora))
        {
            comprada = true;
        }

        // 2. Aquí decido si se desbloquea: si no pide ningún requisito (es la primera) 
        // o si el requisito exigido ya existe en mi lista de compradas.
        if (mejoraRequisito == null || datos.mejorasPrestigioCompradas.Contains(mejoraRequisito.idMejora))
        {
            desbloqueada = true;
        }

        // 3. Gestión de colores para que mi profesora vea el estado visual del árbol.
        if (comprada) 
        {
            imagenFondo.color = Color.cyan; 
            if (lineaConexion != null) lineaConexion.color = Color.cyan;
        } 
        else if (desbloqueada) 
        {
            imagenFondo.color = Color.white; 
            if (lineaConexion != null) lineaConexion.color = Color.gray;
        } 
        else 
        {
            imagenFondo.color = new Color(0.2f, 0.2f, 0.2f); 
            if (lineaConexion != null) lineaConexion.color = Color.black;
        }

        // Hago que el botón sea pulsable únicamente si está desbloqueado y libre de compras.
        if (boton != null) boton.interactable = desbloqueada && !comprada;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
        // Si la mejora está bloqueada (no está desbloqueada ni comprada aún),
        // salgo de la función sin mostrar nada.
        if (!desbloqueada && !comprada) return;

        if (TooltipManager.Instance != null)
        {
            // Le paso el precio casteado a (int) para quitarle los decimales feos en la interfaz.
            TooltipManager.Instance.Mostrar(nombreDeLaMejora, descripcionDeLaMejora, (int)precio);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // En cuanto el ratón sale del botón, ordeno ocultar el panel flotante.
        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.Ocultar();
        }
    }
}