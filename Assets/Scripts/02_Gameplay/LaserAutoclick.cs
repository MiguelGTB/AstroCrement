using UnityEngine;

public class LaserAutoclick : MonoBehaviour
{
    public EconomyManager economy;
    public UIManager ui;

    private float temporizador = 0f;
    private float intervalo = 10f;

    void Update()
    {
        // Solo si tenemos algun laser comprado
        if (economy.nivelesCompras[0] > 0)
        {
            temporizador += Time.deltaTime;
            if (temporizador >= intervalo)
            {
                // Suma tantos clics como laseres tengas
                for (int i = 0; i < economy.nivelesCompras[0]; i++)
                {
                    economy.SumarClick();
                }
                ui.ActualizarInterfaz();
                temporizador = 0f;
            }
        }
    }
}