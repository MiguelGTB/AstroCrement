using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SonidoBoton : MonoBehaviour
{
    [Tooltip("Si activas esto, sonará el efecto de Login en vez del general")]
    public bool esBotonLoginORegistrar = false;

    void Start()
    {
        Button boton = GetComponent<Button>();
        
        // Le programamos automáticamente por código que suene al hacerle click
        boton.onClick.AddListener(ReproducirSonido);
    }

    void ReproducirSonido()
    {
        if (AudioManager.Instance == null) return;

        if (esBotonLoginORegistrar)
        {
            AudioManager.Instance.PlayEfectoLogin();
        }
        else
        {
            AudioManager.Instance.PlayEfectoBotonGeneral();
        }
    }
}
