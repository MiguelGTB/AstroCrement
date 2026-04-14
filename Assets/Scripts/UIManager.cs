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
    public TextMeshProUGUI textoBotonLaser;
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

        if (textoBotonLaser != null)
        {
            int precio = shop.preciosBase[0] * (economy.nivelesCompras[0] + 1);
            textoBotonLaser.text = "Puntero Láser (" + economy.nivelesCompras[0] + ")\nCoste: " + precio;
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
