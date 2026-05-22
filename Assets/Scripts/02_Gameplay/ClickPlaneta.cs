using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClickPlaneta : MonoBehaviour
{
    // Referencias a los gestores de economía, efectos visuales y actualización de interfaz.
    public EconomyManager economy;
    public RebotePlaneta rebote;
    public UIManager ui;

    // Detecta el evento de clic del ratón o toque táctil sobre el colisionador del objeto.
    private void OnMouseDown()
    {
        // Ejecuta la lógica de suma de recursos asociada al clic.
        economy.SumarClick();
        
        // Activa el efecto visual o de animación de rebote si el componente está asignado.
        if (rebote != null) rebote.PlayClick();
        
        // Refresca los elementos de la interfaz de usuario para reflejar el nuevo estado económico.
        ui.ActualizarInterfaz();
    }
}