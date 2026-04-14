using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public EconomyManager economy;
    public TiendaManager shop;

    public TextMeshProUGUI textoDineroUI;
    public TextMeshProUGUI textoPasivoUI;
    public TextMeshProUGUI textoBotonCompra1;
    public TextMeshProUGUI textoBotonMejora1;

    public GameObject panelCompras;
    public GameObject panelMejoras;
    public Image imagenTabCompras;
    public Image imagenTabMejoras;

    public Color colorActivo;
    public Color colorInactivo;

    public void ActualizarInterfaz()
    {
        if (textoDineroUI != null)
            textoDineroUI.text = "Polvo Estelar (PE): " + economy.dineroActual;

        if (textoPasivoUI != null)
            textoPasivoUI.text = "Generando: " + economy.dineroPorSeg + " PE/s";

        if (textoBotonCompra1 != null)
            textoBotonCompra1.text = "Puntero Láser\n(" + shop.costeCompra1 + " PE)";

        if (textoBotonMejora1 != null)
            textoBotonMejora1.text = "Comprar Mejora 1\n(" + shop.costeMejora1 + " PE)";
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
