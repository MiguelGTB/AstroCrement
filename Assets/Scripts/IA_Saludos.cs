using UnityEngine;

public class IA_Saludos : MonoBehaviour
{
    [TextArea]
    public string fraseDeBienvenida;

    void Start()
    {
        // Esperamos medio segundo para que la escena cargue bien y luego habla
        Invoke("Saludar", 0.5f);
    }

    void Saludar()
    {
        if (TextToSpeechManager.Instance != null)
        {
            TextToSpeechManager.Instance.Hablar(fraseDeBienvenida);
        }
    }
}