using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Pon este script directamente en el GameObject del Slider (Volumen_General y Volumen_Efectos).
// Asigna en el Inspector: clave ("VolumenGeneral" o "VolumenEfectos") y textoAsociado.
public class SliderVolumen : MonoBehaviour
{
    [Tooltip("Clave de PlayerPrefs: VolumenGeneral o VolumenEfectos")]
    public string clave;

    [Tooltip("El TextMeshProUGUI que muestra el porcentaje (TextoPorcentaje o TextoPorcentajeEfectos)")]
    public TextMeshProUGUI textoAsociado;

    private Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();

        if (textoAsociado == null)
        {
            string nombreTexto = (clave == "VolumenGeneral") ? "TextoPorcentaje" : "TextoPorcentajeEfectos";
            GameObject obj = GameObject.Find(nombreTexto);
            if (obj != null) textoAsociado = obj.GetComponent<TextMeshProUGUI>();
        }
    }

    void Start()
    {
        if (slider == null) return;

        slider.minValue = 0;
        slider.maxValue = 100;
        slider.wholeNumbers = true;

        float valor = CargarValor();
        slider.value = valor;
        AplicarAudio(valor);

        slider.onValueChanged.AddListener(OnCambioValor);
    }

    void Update()
    {
        if (slider != null && textoAsociado != null)
            textoAsociado.text = (int)slider.value + "%";
    }

    private float CargarValor()
    {
        float valor = PlayerPrefs.GetFloat(clave, 100f);
        if (valor <= 0f) return 100f;
        if (valor <= 1f) valor *= 100f;
        return Mathf.Clamp(valor, 0f, 100f);
    }

    private void OnCambioValor(float valor)
    {
        PlayerPrefs.SetFloat(clave, valor);
        PlayerPrefs.Save();
        AplicarAudio(valor);
    }

    private void AplicarAudio(float valor)
    {
        if (AudioManager.Instance == null) return;
        float volumen = valor / 100f;
        if (clave == "VolumenGeneral" && AudioManager.Instance.MusicaSource != null)
            AudioManager.Instance.MusicaSource.volume = volumen;
        else if (clave == "VolumenEfectos" && AudioManager.Instance.SfxSource != null)
            AudioManager.Instance.SfxSource.volume = volumen;
    }
}
