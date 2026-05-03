using UnityEngine;
using UnityEngine.UI; // Necesario para componentes UI tradicionales
// using TMPro; // Descomenta si usas TextMeshPro

public class LogrosManager : MonoBehaviour
{
    // --- ESTO ES EL SINGLETON ---
    // Esta variable est�tica guardar� la �nica instancia del manager
    public static LogrosManager instance;

    [Header("Configuraci�n de UI")]
    public GameObject panelLogros; // Arrastra aqu� el PanelLogros (el panel entero)
    public Button botonCerrar; // El bot�n "X" o "Volver" dentro del panel

    public Button botonAbrirLogros;

    // La gesti�n de los iconos sigue igual
    [Header("Iconos de Logros (Arrastra en orden)")]
    public Image[] iconosLogros;

    // Esta variable la usar�s en tu c�digo del juego principal para saber si est�n abiertas
    [HideInInspector] public bool achievementsOpen = false;

    // --- L�GICA DEL SINGLETON EN AWAKE ---
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
        // podr�as a�adir DontDestroyOnLoad(this.gameObject);
        // Pero como estamos usando Paneles en la misma escena, no es estrictamente necesario.
    }

    void Start()
    {
        // Asegurarnos de que empiece cerrado
        panelLogros.SetActive(false);
        achievementsOpen = false;

        // Asignar funci�n al bot�n de cerrar
        botonCerrar.onClick.AddListener(CerrarLogros);

        // --- PRUEBA (puedes borrar esto luego) ---
        // Vamos a desbloquear el primer logro al empezar para probar
        DesbloquearLogroVisual(0);
    }

    void Update()
    {
        // Atajo de teclado: Tecla 'L'
        // Esto funcionar� en cualquier nivel
        if (Input.GetKeyDown(KeyCode.L))
        {
            ToggleLogros();
        }
    }

    // Funci�n que abre o cierra dependiendo del estado actual
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

    // Esta funci�n la llamar� cualquier bot�n
    public void AbrirLogros()
    {
        panelLogros.SetActive(true);
        achievementsOpen = true;

        // Opcional: Pausar el juego si es necesario
        // Time.timeScale = 0f; 
        if (botonAbrirLogros != null)
        {
            botonAbrirLogros.gameObject.SetActive(false);
        }
    }

    public void CerrarLogros()
    {
        panelLogros.SetActive(false);
        achievementsOpen = false;

        // Opcional: Reanudar el juego
        // Time.timeScale = 1f;
        if (botonAbrirLogros != null)
        {
            botonAbrirLogros.gameObject.SetActive(true);
        }

    }

    // Esta funci�n solo cambia el visual (de Negro a Color)
    // El 'indice' es la posici�n en el array (de 0 a 19)
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