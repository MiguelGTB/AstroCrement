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
        ActualizarIdioma();
    }

    // Esta función lee el idioma guardado y cambia el texto
    public void ActualizarIdioma()
    {
        int idioma = PlayerPrefs.GetInt("IdiomaSeleccionado", 0); // 0 ES, 1 EN
        if (idioma == 0) tmp.text = textoEspanol;
        else tmp.text = textoIngles;
    }
}
