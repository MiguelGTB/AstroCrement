using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClickPlaneta : MonoBehaviour
{
    public int dineroActual = 0;
    public int dineroPorClic = 1;

    // Interfaz
    public TextMeshProUGUI textoDineroUI;

    // Creamos la funci�n para que cada vez que clickemos en el Planeta (SphereCollider) aumente el dinero
    private void OnMouseDown()
    {
        dineroActual += dineroPorClic;

        textoDineroUI.text = "Plasma Estelar: " + dineroActual;
        // Enviamos un mensaje para corroborar que lo hicimos bien
        Debug.Log("�Clic en la Luna! Dinero total: " + dineroActual);
    }

}
