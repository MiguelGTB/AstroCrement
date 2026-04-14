using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiendaManager : MonoBehaviour
{
    public EconomyManager economy;
    public UIManager ui;

    public int costeCompra1 = 10;
    public int costeMejora1 = 10;

    public void ComprarCompra1()
    {
        if (economy.GastarDinero(costeCompra1))
        {
            economy.dineroPorClic += 1;
            costeCompra1 *= 2;
            ui.ActualizarInterfaz();
        }
    }

    public void ComprarMejora1()
    {
        if (economy.GastarDinero(costeMejora1))
        {
            economy.dineroPorSeg += 2;
            costeMejora1 *= 2;
            ui.ActualizarInterfaz();
        }
    }
}
