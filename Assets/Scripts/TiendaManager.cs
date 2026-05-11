using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiendaManager : MonoBehaviour
{
    public EconomyManager economy;
    public UIManager ui;

    // Precio de cada una
    public double[] preciosBase = new double[15];

    // Dinero que nos d� cada una
    public double[] beneficios = new double[15];

    // Multiplicador de compras
    public float multiplicadorPrecio = 1.15f;
    public void ComprarInstalacion(int id)
    {
        if (economy.nivelesCompras[id] < 100)
        {
            double precioActual = preciosBase[id] * Mathf.Pow(multiplicadorPrecio, economy.nivelesCompras[id]);
            if (economy.GastarDinero(precioActual))
            {
                economy.nivelesCompras[id]++;

                if (id != 0)
                {
                    economy.dineroPorSeg += beneficios[id];
                }

                ui.ActualizarInterfaz();
            }
        }
    }
}
