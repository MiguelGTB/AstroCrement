using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargarAjustes : MonoBehaviour
{
    void Awake()
    {
        // Al entrar en cualquier escena, leemos el volumen guardado (0-100)
        float vol = PlayerPrefs.GetFloat("VolumenMaster", 100f);
        // Lo aplicamos al sistema de Unity (0-1)
        AudioListener.volume = vol / 100f;
    }
}
