using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public EconomyManager economy;
    public TiendaManager shop;

    public TextMeshProUGUI textoDineroUI;
    public TextMeshProUGUI textoPasivoUI;

    // Nombre de las Compras
    public string[] nombresInstalaciones = new string[15];
    // Aquí arrastrarás los 15 textos de tus botones
    public TextMeshProUGUI[] textosBotones = new TextMeshProUGUI[15];

    [Header("Paneles")]
    public GameObject panelCompras;
    public GameObject panelMejoras;

    public void ActualizarInterfaz()
    {
        if (textoDineroUI != null)
            textoDineroUI.text = "Polvo Estelar: " + economy.dineroActual;

        if (textoPasivoUI != null)
            textoPasivoUI.text = economy.dineroPorSeg + " PE/s";

        // Actualizamos los 15 botones
        for (int i = 0; i < 15; i++)
        {
            if (textosBotones[i] != null)
            {
                int precioActual = shop.preciosBase[i] * (economy.nivelesCompras[i] + 1);
                textosBotones[i].text = nombresInstalaciones[i] + " (" + economy.nivelesCompras[i] + ")\nCoste: " + precioActual + " PE";
            }
        }
    }

    public void AbrirPestanaCompras()
    { 
        panelCompras.SetActive(true);
        panelMejoras.SetActive(false);
    }
    public void AbrirPestanaMejoras()
    {
        panelCompras.SetActive(false);
        panelMejoras.SetActive(true);
    }
}