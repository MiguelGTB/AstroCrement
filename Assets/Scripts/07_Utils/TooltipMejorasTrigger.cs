using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipMejorasTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int indiceMejora; 
    public MejorasManager mejorasManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // AHORA SÍ: Usamos double para que encaje con tu economía
        double costo = mejorasManager.listaMejoras[indiceMejora].costePE;
        TooltipMejorasManager.Instance.Mostrar(costo);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipMejorasManager.Instance.Ocultar();
    }
}