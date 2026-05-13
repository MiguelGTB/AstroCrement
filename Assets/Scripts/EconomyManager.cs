using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public double dineroActual = 0;
    public double dineroPorClic = 1;
    public double dineroPorSeg = 0;
    public double dineroTotal = 0;

    public int[] nivelesCompras = new int[15];

    public UIManager ui;
    private float temporizador = 0f;

    public void SumarClick()
    {
        AnadirDinero(dineroPorClic);
        if (ui != null) ui.ActualizarInterfaz();
    }

    void Update()
    {
        if (dineroPorSeg > 0)
        {
            temporizador += Time.deltaTime;

            if (temporizador >= 1f)
            {
                SumarPasivo();
                temporizador = 0f;
            }
        }
    }

    public void SumarPasivo()
    {
        AnadirDinero(dineroPorSeg);
        if (ui != null) ui.ActualizarInterfaz();
    }

    // 4. LA TIENDA
    public bool GastarDinero(double cantidad)
    {
        if (dineroActual >= cantidad)
        {
            dineroActual -= cantidad;
            return true;
        }
        return false;
    }

    public void AnadirDinero(double cantidad)
    {
        dineroActual += cantidad;
        dineroTotal += cantidad;

        // Si el panel de logros existe y está abierto, refrescamos los colores
        if (LogrosManager.instance != null && LogrosManager.instance.achievementsOpen)
        {
            LogrosManager.instance.RefrescarLogros();
        }

    }
}