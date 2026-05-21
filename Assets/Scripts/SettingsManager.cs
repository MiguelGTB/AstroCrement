using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    public string nombreEscenaMenuDefault = "MenuPrincipal";

    [Header("Audio - Slider General / Música")]
    public Slider sliderVolGeneral;
    public TextMeshProUGUI textoVolGeneral;

    [Header("Audio - Slider Efectos (SFX)")]
    public Slider sliderVolEfectos;
    public TextMeshProUGUI textoVolEfectos;

    [Header("Idioma")]
    public TMP_Dropdown dropdownIdioma;

    [Header("Créditos")]
    public CreditosController creditos;

    private bool inicializado = false;

    void Start()
    {
        // 1. CARGAR DATOS DE LA MEMORIA (Por defecto 100 si es la primera vez)
        float volGeneralGuardado = PlayerPrefs.GetFloat("VolumenGeneral", 100f);
        float volEfectosGuardado = PlayerPrefs.GetFloat("VolumenEfectos", 100f);

        // 2. CONFIGURAR SLIDER GENERAL
        if (sliderVolGeneral != null)
        {
            sliderVolGeneral.onValueChanged.RemoveAllListeners();
            sliderVolGeneral.minValue = 0;
            sliderVolGeneral.maxValue = 100;
            sliderVolGeneral.wholeNumbers = true;
            sliderVolGeneral.value = volGeneralGuardado;
        }

        // 3. CONFIGURAR SLIDER EFECTOS
        if (sliderVolEfectos != null)
        {
            sliderVolEfectos.onValueChanged.RemoveAllListeners();
            sliderVolEfectos.minValue = 0;
            sliderVolEfectos.maxValue = 100;
            sliderVolEfectos.wholeNumbers = true;
            sliderVolEfectos.value = volEfectosGuardado;
        }

        // 4. APLICAR VALORES INICIALES A LOS TEXTOS Y AUDIO
        ActualizarVolumenGeneral(volGeneralGuardado);
        ActualizarVolumenEfectos(volEfectosGuardado);

        // 5. CARGAR IDIOMA
        int idiomaGuardado = PlayerPrefs.GetInt("IdiomaSeleccionado", 0); 
        if (dropdownIdioma != null) dropdownIdioma.value = idiomaGuardado;

        // 6. ENCHUFAR EVENTOS DE ESCUCHA
        inicializado = true;
        if (sliderVolGeneral != null) sliderVolGeneral.onValueChanged.AddListener(ActualizarVolumenGeneral);
        if (sliderVolEfectos != null) sliderVolEfectos.onValueChanged.AddListener(ActualizarVolumenEfectos);
    }

    // --- CONTROL DE VOLUMEN GENERAL ---
    public void ActualizarVolumenGeneral(float valor)
    {
        float volumenReal = valor / 100f;

        // El volumen general controlará el AudioListener global de Unity
        AudioListener.volume = volumenReal;

        if (inicializado)
        {
            PlayerPrefs.SetFloat("VolumenGeneral", valor);
            PlayerPrefs.Save();
        }

        if (textoVolGeneral != null)
        {
            textoVolGeneral.text = valor.ToString("0") + "%";
        }
    }

    // --- CONTROL DE VOLUMEN EFECTOS ---
    public void ActualizarVolumenEfectos(float valor)
    {
        float volumenReal = valor / 100f;

        // Buscamos el AudioManager inmortal y le cambiamos el volumen SOLO a la fuente de SFX
        if (AudioManager.Instance != null)
        {
            // Para asegurarnos de no romper nada, buscamos los AudioSource vinculados en el Manager
            // Si recuerdas, el segundo AudioSource lo creamos para los efectos.
            AudioSource[] fuentes = AudioManager.Instance.GetComponents<AudioSource>();
            if (fuentes.Length > 1 && fuentes[1] != null)
            {
                fuentes[1].volume = volumenReal; // El segundo source es el SFX Source
            }
        }

        if (inicializado)
        {
            PlayerPrefs.SetFloat("VolumenEfectos", valor);
            PlayerPrefs.Save();
        }

        if (textoVolEfectos != null)
        {
            textoVolEfectos.text = valor.ToString("0") + "%";
        }
    }

    public void CambiarIdioma(int indice)
    {
        PlayerPrefs.SetInt("IdiomaSeleccionado", indice);
        PlayerPrefs.Save();

        TextoLocalizado[] todosLosTextos = FindObjectsOfType<TextoLocalizado>();
        foreach (TextoLocalizado t in todosLosTextos)
        {
            if (t != null) t.ActualizarIdioma();
        }
    }

    public void VolverAlMenu()
    {
        if (!string.IsNullOrEmpty(PartidaActual.EscenaAnterior))
        {
            SceneManager.LoadScene(PartidaActual.EscenaAnterior);
        }
        else
        {
            SceneManager.LoadScene(nombreEscenaMenuDefault);
        }
    }

    public void AbrirCreditos()
    {
        if (creditos != null) creditos.Mostrar();
    }

    public void CerrarCreditos()
    {
        if (creditos != null) creditos.Ocultar();
    }
}