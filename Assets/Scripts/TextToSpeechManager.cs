using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro; // <-- NUEVO: Necesario para modificar los textos

public class TextToSpeechManager : MonoBehaviour
{
    public static TextToSpeechManager Instance;

    [Header("Configuración de ElevenLabs")]
    public string apiKey = ""; 
    public string voiceId = ""; 

    [Header("Referencias")]
    public AudioSource altavozIA;
    
    [Header("Interfaz de Usuario")]
    public GameObject panelSubtitulos; // <-- NUEVO: El panel que aparece y desaparece
    public TextMeshProUGUI textoSubtitulos; // <-- NUEVO: El texto que cambia

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Al empezar el juego, nos aseguramos de que el panel esté invisible
        if (panelSubtitulos != null) panelSubtitulos.SetActive(false);
    }

    public void Hablar(string texto)
    {
        StartCoroutine(PedirAudioAElevenLabs(texto));
    }

    private IEnumerator PedirAudioAElevenLabs(string texto)
    {
        // 1. Encendemos el panel visual y ponemos "Conectando..."
        if (panelSubtitulos != null) panelSubtitulos.SetActive(true);
        if (textoSubtitulos != null) textoSubtitulos.text = "Conectando con la IA...";

        string url = "https://api.elevenlabs.io/v1/text-to-speech/" + voiceId;
        string jsonData = "{\"text\":\"" + texto + "\",\"model_id\":\"eleven_multilingual_v2\"}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            www.method = "POST";
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("xi-api-key", apiKey);
            www.SetRequestHeader("Accept", "audio/mpeg");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                altavozIA.clip = clip;
                altavozIA.Play();
                
                // 2. Ponemos en pantalla exactamente lo que está diciendo
                if (textoSubtitulos != null) textoSubtitulos.text = texto;

                // 3. Esperamos a que termine el audio exacto que dura el clip
                yield return new WaitForSeconds(clip.length);

                // 4. Apagamos el panel cuando se calla
                if (panelSubtitulos != null) panelSubtitulos.SetActive(false);
            }
            else
            {
                if (textoSubtitulos != null) textoSubtitulos.text = "Error de comunicación.";
                yield return new WaitForSeconds(2f);
                if (panelSubtitulos != null) panelSubtitulos.SetActive(false);
            }
        }
    }
}