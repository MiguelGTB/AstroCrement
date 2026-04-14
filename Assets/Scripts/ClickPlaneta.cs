using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClickPlaneta : MonoBehaviour
{
    public EconomyManager economy;
    public RebotePlaneta rebote;
    public UIManager ui;

    private void OnMouseDown()
    {
        economy.SumarClick();
        rebote.PlayClick();
        ui.ActualizarInterfaz();
    }
}
