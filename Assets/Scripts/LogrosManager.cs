using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LogrosManager : MonoBehaviour
{
    public static LogrosManager instance;

    [Header("Configuración de UI")]
    public Button botonCerrar;

    [HideInInspector] public bool achievementsOpen = false;

    private bool puedeUsarTeclado = false;
    void Awake()
    {
        Debug.Log("LogrosManager creado: " + gameObject.name);
        instance = this;
    }

    void Start()
    {
        achievementsOpen = true;

        if (botonCerrar != null)
            botonCerrar.onClick.AddListener(Cerrar);

        // Al empezar, refrescamos por si ya venimos con datos cargados de la nube
        RefrescarLogros();
        StartCoroutine(HabilitarTeclado());
    }

    IEnumerator HabilitarTeclado()
    {
        // Esperamos un pequeño momento para evitar que el jugador pulse "L" antes de que el menú esté listo
        yield return new WaitForSeconds(0.2f);
        puedeUsarTeclado = true;
        Debug.Log("Teclado hablitado: " + puedeUsarTeclado);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
    {
        Debug.Log("L pulsada, puedeUsarTeclado: " + puedeUsarTeclado);
        if (puedeUsarTeclado) Cerrar();
    }

        // OPCIONAL: Si quieres que se iluminen MIENTRAS el panel está abierto 
        // sin tener que cerrarlo y abrirlo, descomenta la siguiente línea:
        // if (achievementsOpen) RefrescarLogros();
    }

    private bool cerrando = false;

    public void Cerrar()
    {
        if (cerrando) return;
        cerrando = true;

        string escenaDestino = PartidaActual.EscenaAnterior;
        PartidaActual.EscenaAnterior = ""; // La limpiamos para que no se reutilice
        
        Debug.Log("Cargando escena: " + escenaDestino);

        if (string.IsNullOrEmpty(escenaDestino))
            escenaDestino = "Seleccion_Niveles";

        SceneManager.LoadScene(escenaDestino);
    }

    public void RefrescarLogros()
    {
        TooltipLogroTrigger[] triggers = FindObjectsOfType<TooltipLogroTrigger>(true);
        foreach (var t in triggers)
            t.ComprobarEstadoVisual();
    }
}