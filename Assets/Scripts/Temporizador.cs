using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temporizador : MonoBehaviour
{
    public EconomyManager economy;
    public float tiempoEntrePagos = 2f;

    private float temporizador = 0f;

    void Update()
    {
        if (economy.dineroPorSeg <= 0) return;

        temporizador += Time.deltaTime;

        if (temporizador >= tiempoEntrePagos)
        {
            economy.SumarPasivo();
            temporizador = 0f;
        }
    }
}
