using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    public string nombreEscenaMenu = "MenuPrincipal";

    [Header("Audio (0-100)")]
    public Slider sliderVolumen;
    public TextMeshProUGUI textoPorcentajeVolumen;

    [Header("Idioma")]
    public TMP_Dropdown dropdownIdioma;

    [Header("Créditos")]
    public GameObject objetoPanelCreditos;

    void Start()
    {
        // 1. CARGAR AJUSTES DE AUDIO
        // Cargamos el volumen guardado (por defecto 100 si es la primera vez)
        float volGuardado = PlayerPrefs.GetFloat("VolumenMaster", 100f);

        // Configuramos el Slider y el AudioListener al empezar
        if (sliderVolumen != null)
        {
            sliderVolumen.minValue = 0;
            sliderVolumen.maxValue = 100;
            sliderVolumen.wholeNumbers = true; // Para que use números enteros (1, 2, 3...)
            sliderVolumen.value = volGuardado;
        }

        ActualizarVolumenSistema(volGuardado);

        // 2. CARGAR AJUSTES DE IDIOMA
        int idiomaGuardado = PlayerPrefs.GetInt("IdiomaSeleccionado", 0); // 0 = Español, 1 = Inglés
        if (dropdownIdioma != null)
        {
            dropdownIdioma.value = idiomaGuardado;
        }

        // 3. ASIGNAR EVENTOS POR CÓDIGO (Opcional, pero más seguro)
        if (sliderVolumen != null)
            sliderVolumen.onValueChanged.AddListener(ActualizarVolumenSistema);
    }

    // Función que se activa al mover el slider
    public void ActualizarVolumenSistema(float valor)
    {
        // Convertimos el 0-100 del Slider al 0-1 que usa Unity internamente
        float volumenReal = valor / 100f;
        AudioListener.volume = volumenReal;

        // Guardamos el ajuste para que no se pierda al cerrar el juego
        PlayerPrefs.SetFloat("VolumenMaster", valor);

        // Actualizamos el texto visual (ej: "75%")
        if (textoPorcentajeVolumen != null)
        {
            textoPorcentajeVolumen.text = valor.ToString("0") + "%";
        }

        Debug.Log("Volumen del sistema ajustado a: " + valor + "%");
    }

    // Función para el Dropdown de idioma
    public void CambiarIdioma(int indice)
    {
        // 1. Guardamos el índice (0 o 1)
        PlayerPrefs.SetInt("IdiomaSeleccionado", indice);

        // 2. ¡IMPORTANTE! Forzamos el guardado en el disco
        PlayerPrefs.Save();

        if (indice == 0)
            Debug.Log("Idioma seleccionado y guardado: Español");
        else
            Debug.Log("Language selected and saved: English");

        // 3. Avisamos a todos los textos de la escena que deben cambiar AHORA
        TextoLocalizado[] todosLosTextos = FindObjectsOfType<TextoLocalizado>();
        foreach (TextoLocalizado t in todosLosTextos)
        {
            t.ActualizarIdioma();
        }
    }

    // Función para el botón de la X o "Volver"
    public void VolverAlMenu()
    {
        Debug.Log("Regresando al puente de mando...");
        SceneManager.LoadScene(nombreEscenaMenu);
    }

    public void AbrirCreditos()
    {
        if (objetoPanelCreditos != null) objetoPanelCreditos.SetActive(true);
    }

    public void CerrarCreditos()
    {
        if (objetoPanelCreditos != null) objetoPanelCreditos.SetActive(false);
    }
}