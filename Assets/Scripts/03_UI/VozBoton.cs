using UnityEngine;

public class VozBoton : MonoBehaviour
{
    public void DecirFrase(string frase)
    {
        Debug.Log("1. Botón pulsado. Intentando decir: " + frase);
        
        if (TextToSpeechManager.Instance != null)
        {
            TextToSpeechManager.Instance.Hablar(frase);
        }
        else
        {
            Debug.LogError("ERROR: ¡No encuentro al IAManager! ¿Le pusiste el script TextToSpeechManager?");
        }
    }
}