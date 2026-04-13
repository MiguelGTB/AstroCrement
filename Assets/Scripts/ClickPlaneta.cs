using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using TMPro;
using UnityEngine;

public class ClickPlaneta : MonoBehaviour
{
    public int dineroActual = 0;
    public int dineroPorClic = 1;
    public int dineroPorSeg = 0;
    public float tiempoEntrePagos = 2f;

    // Interfaz y Efectos
    public TextMeshProUGUI textoDineroUI;
    public TextMeshProUGUI textoPasivoUI;
    public ParticleSystem particulasClic;

    // Compra 1 (Rover)
    int costeRover = 10;
    public TextMeshProUGUI textoBotonRover;

    // Mejora 1 (Minero)
    int costeMinero = 10;
    public TextMeshProUGUI textoBotonMinero;


    // Efecto click planeta
    private Vector3 tamanoOriginal;
    private float velocidadRebote = 10f;
    private float cantidadEncogimiento = 0.9f;
    private float temporizador = 0f;

    private void Start()
    {
        tamanoOriginal = transform.localScale;
        ActualizarInterfaz();
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, tamanoOriginal, Time.deltaTime * velocidadRebote);
        
        if(dineroPorSeg > 0)
        {
            temporizador += Time.deltaTime;
            if(temporizador >= tiempoEntrePagos)
            {
                dineroActual += dineroPorSeg;
                ActualizarInterfaz();
                temporizador = 0f;
            }
        }

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
    public void ComprarRover()
    {
        if (dineroActual >= costeRover)
        {
            dineroActual -= costeRover; // Restamos el dinero
            dineroPorClic += 1;          // Mejoramos el poder del clic
            costeRover *= 2;            // Multiplicamos el coste para la próxima vez

            ActualizarInterfaz();
        }
    }

    public void ComprarMinero()
    {
        if (dineroActual >= costeMinero)
        {
            dineroActual -= costeMinero;
            dineroPorSeg += 2;      // Te da 2 de dinero cada segundo
            costeMinero *= 2;           // Dobla su precio
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

        if( textoPasivoUI != null)
        {
            textoPasivoUI.text = "Generando: " + dineroPorSeg + " PE/s";
        }

        if (textoBotonRover != null)
        {
            textoBotonRover.text = "Comprar Rover\n(" + costeRover + " PE)";
        }

        if (textoBotonMinero != null)
        {
            textoBotonMinero.text = "Comprar Minero\n(" + costeMinero + " PE)";
        }
    }


}
