using UnityEngine;

public class LaserAutoclick : MonoBehaviour
{
    public EconomyManager economy;
    public UIManager ui;

    private float temporizador = 0f;
    private float intervalo = 10f;

    void Update()
    {
        // Solo si tenemos algún láser comprado (Posición 0)
        if (economy.nivelesCompras[0] > 0)
        {
            temporizador += Time.deltaTime;
            if (temporizador >= intervalo)
            {
                // Suma tantos clics como láseres tengas
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