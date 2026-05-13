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

        if (botonCerrar != null)
            botonCerrar.onClick.AddListener(CerrarLogros);

        // Al empezar, refrescamos por si ya venimos con datos cargados de la nube
        RefrescarLogros();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ToggleLogros();
        }

        // OPCIONAL: Si quieres que se iluminen MIENTRAS el panel está abierto 
        // sin tener que cerrarlo y abrirlo, descomenta la siguiente línea:
        // if (achievementsOpen) RefrescarLogros();
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

        // Refrescamos al abrir
        RefrescarLogros();
    }

    public void RefrescarLogros()
    {
        // IMPORTANTE: Buscamos en panelLogros específicamente para ir a lo seguro
        TooltipLogroTrigger[] triggers = panelLogros.GetComponentsInChildren<TooltipLogroTrigger>(true);

        foreach (TooltipLogroTrigger t in triggers)
        {
            t.ComprobarEstadoVisual();
        }
    }

    public void CerrarLogros()
    {
        panelLogros.SetActive(false);
        achievementsOpen = false;

        if (botonAbrirLogros != null) botonAbrirLogros.gameObject.SetActive(true);
    }
}