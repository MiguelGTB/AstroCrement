using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using TMPro;
using UnityEngine;

public class ClickPlaneta : MonoBehaviour
{
    public int dineroActual = 0;
    public int dineroPorClic = 1;

    // Interfaz
    public TextMeshProUGUI textoDineroUI;

    // Efecto click planeta
    private Vector3 tamanoOriginal;
    private float velocidadRebote = 10f;
    private float cantidadEncogimiento = 0.9f;


    private void Start()
    {
        tamanoOriginal = transform.localScale;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, tamanoOriginal, Time.deltaTime * velocidadRebote);
    }


    // Creamos la funci�n para que cada vez que clickemos en el Planeta (SphereCollider) aumente el dinero
    private void OnMouseDown()
    {
        dineroActual += dineroPorClic;

        textoDineroUI.text = "Plasma Estelar: " + dineroActual;

        transform.localScale = tamanoOriginal * cantidadEncogimiento;

    }

}
