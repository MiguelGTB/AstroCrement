using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    // Compra 1
    int costeCompra1 = 10;
    public TextMeshProUGUI textoBotonCompra1;

    // Mejora 1
    int costeMejora1 = 10;
    public TextMeshProUGUI textoBotonMejora1;


    // Efecto click planeta
    private Vector3 tamanoOriginal;
    private float velocidadRebote = 10f;
    private float cantidadEncogimiento = 0.9f;
    private float temporizador = 0f;

    // Paneles Compras y Mejoras
    public GameObject panelCompras;
    public GameObject panelMejoras;
    public Image imagenTabCompras;
    public Image imagenTabMejoras;

    // Colores Paneles
    public Color colorActivo = new Color(1f, 1f, 1f, 1f);
    public Color colorInactivo = new Color(0.5f, 0.5f, 0.5f, 0.8f);


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
    public void ComprarCompra1()
    {
        if (dineroActual >= costeCompra1)
        {
            dineroActual -= costeCompra1; // Restamos el dinero
            dineroPorClic += 1;          // Mejoramos el poder del clic
            costeCompra1 *= 2;            // Multiplicamos el coste para la próxima vez

            ActualizarInterfaz();
        }
    }

    public void ComprarMejora1()
    {
        if (dineroActual >= costeMejora1)
        {
            dineroActual -= costeMejora1;
            dineroPorSeg += 2;      // Te da 2 de dinero cada segundo
            costeMejora1 *= 2;           // Dobla su precio
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

        if (textoBotonCompra1 != null)
        {
            textoBotonCompra1.text = "Puntero Láser\n(" + costeCompra1 + " PE)";
        }

        if (textoBotonMejora1 != null)
        {
            textoBotonMejora1.text = "Comprar Mejora 1\n(" + costeMejora1 + " PE)";
        }
    }
    public void AbrirPestanaCompras()
    {
        panelCompras.SetActive(true);
        panelMejoras.SetActive(false);

        imagenTabCompras.color = colorActivo;
        imagenTabMejoras.color = colorInactivo;
    }
    public void AbrirPestanaMejoras()
    {
        panelCompras.SetActive(false);
        panelMejoras.SetActive(true);

        imagenTabCompras.color = colorInactivo;
        imagenTabMejoras.color = colorActivo;
    }

}
