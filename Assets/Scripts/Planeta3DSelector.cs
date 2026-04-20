using UnityEngine;

public class Planeta3DSelector : MonoBehaviour
{
    [Header("Configuración del Planeta")]
    public int indicePlaneta; // 0 para Luna, 1 para Marte, etc.
    
    // Referencias
    public LevelSelectionManager levelManager;
    public RebotePlaneta rebote;

    private void OnMouseDown()
    {
        // 1. Hacemos el efecto de rebote visual (¡Juice!)
        if (rebote != null) rebote.PlayClick();

        // 2. Le decimos al Manager que viaje a este planeta
        if (levelManager != null)
        {
            levelManager.ViajarAlPlaneta(indicePlaneta);
        }
    }
}