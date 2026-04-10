using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using TMPro;
using UnityEngine;

public class ClickPlaneta : MonoBehaviour
{
    public int dineroActual = 0;
    public int dineroPorClic = 1;

    public int costeMejora = 10;

    // Interfaz
    public TextMeshProUGUI textoDineroUI;
    public TextMeshProUGUI textoBotonMejora;

    // Efecto click planeta
    public ParticleSystem particulasClic;
    private Vector3 tamanoOriginal;
    private float velocidadRebote = 10f;
    private float cantidadEncogimiento = 0.9f;


    private void Start()
    {
        tamanoOriginal = transform.localScale;
        ActualizarInterfaz();
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, tamanoOriginal, Time.deltaTime * velocidadRebote);
    }


    // Creamos la funci�n para que cada vez que clickemos en el Planeta (SphereCollider) aumente el dinero
    private void OnMouseDown()
    {
        dineroActual += dineroPorClic;
        ActualizarInterfaz();

        transform.localScale = tamanoOriginal * cantidadEncogimiento;

        if(particulasClic != null)
        {
            particulasClic.Play();
        }

    }

    // Función que se ejecuta cuando pulsas el botón de la tienda
    public void ComprarMejora()
    {
        if (dineroActual >= costeMejora)
        {
            dineroActual -= costeMejora; // Restamos el dinero
            dineroPorClic += 1;          // Mejoramos el poder del clic
            costeMejora *= 2;            // Multiplicamos el coste para la próxima vez

            ActualizarInterfaz();
        }
    }

    // Función auxiliar para no repetir código al actualizar textos
    void ActualizarInterfaz()
    {
        if (textoDineroUI != null)
        {
            textoDineroUI.text = "Polvo Estelar (PE): " + dineroActual;
        }

        if (textoBotonMejora != null)
        {
            textoBotonMejora.text = "Comprar Rover\n(" + costeMejora + " PE)";
        }
    }


}
