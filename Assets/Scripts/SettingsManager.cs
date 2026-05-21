using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    public string nombreEscenaMenuDefault = "MenuPrincipal";

    [Header("Idioma")]
    public TMP_Dropdown dropdownIdioma;

    [Header("Créditos")]
    public CreditosController creditos;

    void Start()
    {
        if (dropdownIdioma != null)
            dropdownIdioma.value = PlayerPrefs.GetInt("IdiomaSeleccionado", 0);
    }

    public void AplicarAjustes()
    {
        if (dropdownIdioma != null)
        {
            PlayerPrefs.SetInt("IdiomaSeleccionado", dropdownIdioma.value);
            PlayerPrefs.Save();
        }
    }

    public void CambiarIdioma(int indice)
    {
        PlayerPrefs.SetInt("IdiomaSeleccionado", indice);
        PlayerPrefs.Save();

        foreach (TextoLocalizado t in FindObjectsOfType<TextoLocalizado>())
            t.ActualizarIdioma();
    }

    public void VolverAlMenu()
    {
        if (!string.IsNullOrEmpty(PartidaActual.EscenaAnterior))
            SceneManager.LoadScene(PartidaActual.EscenaAnterior);
        else
            SceneManager.LoadScene(nombreEscenaMenuDefault);
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
