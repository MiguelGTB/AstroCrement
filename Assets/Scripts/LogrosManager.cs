using UnityEngine;
using UnityEngine.UI;

public class LogrosManager : MonoBehaviour
{
    public static LogrosManager instance;

    [Header("Configuración de UI")]
    public GameObject panelLogros; 
    public Button botonCerrar; 
    public Button botonAbrirLogros;

    [Header("Iconos de Logros (Arrastra en orden)")]
    public Image[] iconosLogros;

    [HideInInspector] public bool achievementsOpen = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        panelLogros.SetActive(false);
        achievementsOpen = false;
        botonCerrar.onClick.AddListener(CerrarLogros);

        // Al empezar, ponemos todos en gris por defecto
        foreach (Image img in iconosLogros)
        {
            if (img != null) img.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ToggleLogros();
        }
    }

    public void ToggleLogros()
    {
        if (achievementsOpen) CerrarLogros();
        else AbrirLogros();
    }

    public void AbrirLogros()
    {
        panelLogros.SetActive(true);
        achievementsOpen = true;

        if (botonAbrirLogros != null) botonAbrirLogros.gameObject.SetActive(false);

        // --- ESTO ES LO NUEVO ---
        // Cada vez que abrimos el panel, refrescamos el color de todos los iconos
        RefrescarLogros();
    }

    // Nueva función que busca los scripts de los iconos y les pide que se actualicen
    public void RefrescarLogros()
    {
        // Buscamos todos los scripts "TooltipLogroTrigger" que hay dentro del panel
        TooltipLogroTrigger[] triggers = GetComponentsInChildren<TooltipLogroTrigger>(true);
        
        foreach (TooltipLogroTrigger t in triggers)
        {
            t.ComprobarEstadoVisual(); // Llamamos a la función que crearemos en el otro script
        }
    }

    public void CerrarLogros()
    {
        panelLogros.SetActive(false);
        achievementsOpen = false;

        if (botonAbrirLogros != null) botonAbrirLogros.gameObject.SetActive(true);
    }

    public void DesbloquearLogroVisual(int indice)
    {
        if (indice >= 0 && indice < iconosLogros.Length)
        {
            iconosLogros[indice].color = Color.white;
        }
    }
}