using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbrirLogros : MonoBehaviour
{
    public void ClickEnBoton()
    {
        // Llamamos directamente al Singleton sin necesidad de variables públicas
        LogrosManager.instance.AbrirLogros();
    }
}
