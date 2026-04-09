using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickPlaneta : MonoBehaviour
{
    public int dineroActual = 0;
    public int dineroPorClic = 1;

    // Creamos la función para que cada vez que clickemos en el Planeta (SphereCollider) aumente el dinero
    private void OnMouseDown()
    {
        dineroActual += dineroPorClic;

        // Enviamos un mensaje para corroborar que lo hicimos bien
        Debug.Log("¡Clic en la Luna! Dinero total: " + dineroActual);
    }

}
