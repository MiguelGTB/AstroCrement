using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public int dineroActual = 0;
    public int dineroPorClic = 1;
    public int dineroPorSeg = 0;

    public int[] nivelesCompras = new int[15];
    public bool GastarDinero(int cantidad)
    {
        if (dineroActual >= cantidad)
        {
            dineroActual -= cantidad;
            return true;
        }
        return false;
    }

    public void SumarClick()
    {
        dineroActual += dineroPorClic;
    }

    public void SumarPasivo()
    {
        dineroActual += dineroPorSeg;
    }
}
