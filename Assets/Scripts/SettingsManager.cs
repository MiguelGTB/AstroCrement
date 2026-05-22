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
        // Al arrancar, miro qué idioma dejé guardado la última vez en el disco.
        // Si no hay nada, por defecto pongo 0, que para mí es el Español.
        if (dropdownIdioma != null)
        {
            dropdownIdioma.value = PlayerPrefs.GetInt("IdiomaSeleccionado", 0);
        }
        
        // Me aseguro de actualizar todos los textos nada más abrir la pantalla por si acaso.
        ActualizarTextosDeLaEscena();
    }

    public void AplicarAjustes()
    {
        // Este es el método que tengo vinculado a mi botón de "Aplicar / Guardar".
        if (dropdownIdioma != null)
        {
            // Cojo el número del dropdown (0 para Español, 1 para Inglés) y lo grabo a fuego en el disco.
            PlayerPrefs.SetInt("IdiomaSeleccionado", dropdownIdioma.value);
            PlayerPrefs.Save(); // Fuerzo al sistema a guardar los datos en el disco AHORA MISMO.
            
            Debug.Log("He guardado el idioma correctamente. Índice: " + dropdownIdioma.value);

            // Lanzo la actualización para que todos los textos de esta escena cambien de golpe.
            ActualizarTextosDeLaEscena();
        }
    }

    public void CambiarIdioma(int indice)
    {
        // Por si acaso el jugador cambia el desplegable pero no le da a Aplicar, 
        // yo también registro el valor aquí para que la experiencia sea más fluida.
        PlayerPrefs.SetInt("IdiomaSeleccionado", indice);
        PlayerPrefs.Save();

        ActualizarTextosDeLaEscena();
    }

    private void ActualizarTextosDeLaEscena()
    {
        // Busco en toda la escena activa absolutamente todos los scripts de "TextoLocalizado".
        // Les ordeno uno a uno que comprueben qué idioma he guardado y que se cambien al instante.
        TextoLocalizado[] todosLosTextos = FindObjectsOfType<TextoLocalizado>();
        
        Debug.Log("He encontrado " + todosLosTextos.Length + " textos para traducir en esta escena.");

        foreach (TextoLocalizado t in todosLosTextos)
        {
            t.ActualizarIdioma();
        }
    }

    public void VolverAlMenu()
    {
        // Antes de marcharme al menú principal, me aseguro de guardar todo una última vez.
        PlayerPrefs.Save();

        if (!string.IsNullOrEmpty(PartidaActual.EscenaAnterior))
            SceneManager.LoadScene(PartidaActual.EscenaAnterior);
        else
            SceneManager.LoadScene(nombreEscenaMenuDefault);
    }

    public void AbrirCreditos()
    {
        if (creditos != null) creditos.Mostrar();
    }
}