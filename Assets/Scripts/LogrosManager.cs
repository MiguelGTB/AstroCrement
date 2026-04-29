using UnityEngine;
using UnityEngine.UI; // Necesario para componentes UI tradicionales
// using TMPro; // Descomenta si usas TextMeshPro

public class LogrosManager : MonoBehaviour
{
    // --- ESTO ES EL SINGLETON ---
    // Esta variable estática guardará la única instancia del manager
    public static LogrosManager instance;

    [Header("Configuración de UI")]
    public GameObject panelLogros; // Arrastra aquí el PanelLogros (el panel entero)
    public Button botonCerrar; // El botón "X" o "Volver" dentro del panel

    // La gestión de los iconos sigue igual
    [Header("Iconos de Logros (Arrastra en orden)")]
    public Image[] iconosLogros;

    // Esta variable la usarás en tu código del juego principal para saber si están abiertas
    [HideInInspector] public bool achievementsOpen = false;

    // --- LÓGICA DEL SINGLETON EN AWAKE ---
    void Awake()
    {
        // Si ya existe una instancia y no soy yo, me destruyo
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        // Si soy la primera instancia, me guardo como la instancia global
        instance = this;

        // Opcional: Si este script controla logros que persisten entre escenas,
        // podrías añadir DontDestroyOnLoad(this.gameObject);
        // Pero como estamos usando Paneles en la misma escena, no es estrictamente necesario.
    }

    void Start()
    {
        // Asegurarnos de que empiece cerrado
        panelLogros.SetActive(false);
        achievementsOpen = false;

        // Asignar función al botón de cerrar
        botonCerrar.onClick.AddListener(CerrarLogros);

        // --- PRUEBA (puedes borrar esto luego) ---
        // Vamos a desbloquear el primer logro al empezar para probar
        DesbloquearLogroVisual(0);
    }

    void Update()
    {
        // Atajo de teclado: Tecla 'L'
        // Esto funcionará en cualquier nivel
        if (Input.GetKeyDown(KeyCode.L))
        {
            ToggleLogros();
        }
    }

    // Función que abre o cierra dependiendo del estado actual
    public void ToggleLogros()
    {
        if (achievementsOpen)
        {
            CerrarLogros();
        }
        else
        {
            AbrirLogros();
        }
    }

    // Esta función la llamará cualquier botón
    public void AbrirLogros()
    {
        panelLogros.SetActive(true);
        achievementsOpen = true;

        // Opcional: Pausar el juego si es necesario
        // Time.timeScale = 0f; 
    }

    public void CerrarLogros()
    {
        panelLogros.SetActive(false);
        achievementsOpen = false;

        // Opcional: Reanudar el juego
        // Time.timeScale = 1f;
    }

    // Esta función solo cambia el visual (de Negro a Color)
    // El 'indice' es la posición en el array (de 0 a 19)
    public void DesbloquearLogroVisual(int indice)
    {
        if (indice >= 0 && indice < iconosLogros.Length)
        {
            // Cambiamos el color de Negro a Blanco (Blanco = Mostrar colores originales)
            iconosLogros[indice].color = Color.white;
            Debug.Log("Logro visual " + indice + " desbloqueado!");
        }
    }
}