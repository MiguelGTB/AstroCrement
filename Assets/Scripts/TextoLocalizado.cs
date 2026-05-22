using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextoLocalizado : MonoBehaviour
{
    [TextArea] public string textoEspanol;
    [TextArea] public string textoIngles;

    private TextMeshProUGUI tmp;

    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        
        // Esto es clave: en cuanto este texto aparece en pantalla (en cualquier nivel o menú),
        // se auto-configura leyendo el archivo de ajustes local. Así evito que aparezca en el idioma bugeado.
        ActualizarIdioma();
    }

    public void ActualizarIdioma()
    {
        // Me aseguro de pillar mi componente de texto antes de escribir en él,
        // por si acaso esta función es llamada desde el SettingsManager antes del Start.
        if (tmp == null) tmp = GetComponent<TextMeshProUGUI>();
        if (tmp == null) return;

        // Leo el registro: 0 es Español, 1 es Inglés.
        int idioma = PlayerPrefs.GetInt("IdiomaSeleccionado", 0); 
        
        if (idioma == 0) 
        {
            tmp.text = textoEspanol;
        }
        else 
        {
            tmp.text = textoIngles;
        }
    }
}