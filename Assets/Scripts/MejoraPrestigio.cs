using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MejoraPrestigio : MonoBehaviour
{
    [Header("Configuración de la Mejora")]
    public string idMejora;      // ID único (ej: "mas_click_1")
    public double precio;        // Coste en monedas de prestigio
    public string descripcion;   // Qué hace la mejora

    [Header("Dependencias")]
    public MejoraPrestigio mejoraRequisito; // La mejora que hay que comprar ANTES que esta
    public Image lineaConexion;             // La línea visual que une este botón con el anterior

    [Header("Estado")]
    public bool comprada = false;
    public bool desbloqueada = false;

    private Button boton;
    private Image imagenFondo;

    void Awake()
    {
        boton = GetComponent<Button>();
        imagenFondo = GetComponent<Image>();
    }

    // Este método se llama cuando pulsas el botón en el juego
    public void IntentarComprar()
    {
        // AHORA SÍ usa tu DatabaseManager
        PlayerData datos = DatabaseManager.Instance.datosCargados;

        if (!comprada && desbloqueada && datos.monedasPrestigio >= precio)
        {
            datos.monedasPrestigio -= precio;
            datos.mejorasPrestigioCompradas.Add(idMejora);
            comprada = true;

            // Guardamos en tu Firebase inmediatamente usando tu función
            DatabaseManager.Instance.GuardarPartidaEnNube();
            
            // Refrescamos todo el árbol visualmente
            GameObject.FindObjectOfType<ArbolManager>().ActualizarTodoElArbol();
        }
    }

    public void RefrescarEstadoVisual(PlayerData datos)
    {
        // 1. Comprobar si ya la tenemos
        if (datos.mejorasPrestigioCompradas.Contains(idMejora)) comprada = true;

        // 2. Comprobar si se puede desbloquear (si no tiene requisito o el requisito ya se compró)
        if (mejoraRequisito == null || datos.mejorasPrestigioCompradas.Contains(mejoraRequisito.idMejora))
        {
            desbloqueada = true;
        }

        // 3. Cambiar colores
        if (comprada) {
            imagenFondo.color = Color.cyan; // Azul: Comprada
            if(lineaConexion != null) lineaConexion.color = Color.cyan;
        } else if (desbloqueada) {
            imagenFondo.color = Color.white; // Blanco: Disponible
            if(lineaConexion != null) lineaConexion.color = Color.gray;
        } else {
            imagenFondo.color = new Color(0.2f, 0.2f, 0.2f); // Oscuro: Bloqueada
            if(lineaConexion != null) lineaConexion.color = Color.black;
        }

        if(boton != null) boton.interactable = desbloqueada && !comprada;
    }
}