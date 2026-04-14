using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiendaManager : MonoBehaviour
{
    public EconomyManager economy;
    public UIManager ui;

    // Precio de cada una
    public int[] preciosBase = new int[15];

    // Dinero que nos dá cada una
    public int[] beneficios = new int[15];

    public void ComprarCompra1(int id)
    {
        int precioActual = preciosBase[id] * (economy.nivelesCompras[id] + 1);
        if (economy.GastarDinero(precioActual))
        {
            economy.nivelesCompras[id]++;

            if(id != 0)
            {
                economy.dineroPorSeg += beneficios[id];
            }

            ui.ActualizarInterfaz();
        }
    }
}
