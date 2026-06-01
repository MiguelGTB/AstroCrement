using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro; 

public class TextToSpeechManager : MonoBehaviour
{
    public static TextToSpeechManager Instance;

    [Header("Configuración de ElevenLabs (Desde Firebase)")]
    // Ya no pongo mi clave aquí. Se rellenará sola al conectar con Firebase.
    [HideInInspector] public string apiKey = ""; 
    public string voiceId = "pon_aqui_tu_id_de_voz"; // Esto sí se queda porque no es privado
    
    // ESTA ES LA VARIABLE QUE LE FALTABA A UNITY:
    [HideInInspector] public bool sistemaActivo = false;

    [Header("Referencias")]
    public AudioSource altavozIA;
    
    [Header("Interfaz de Usuario")]
    public GameObject panelSubtitulos; 
    public TextMeshProUGUI textoSubtitulos; 

    void Awake()
    {
        // Me aseguro de que solo haya un cerebro de IA en todo el juego
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
        // Al empezar, apago los subtítulos para que no molesten
        if (panelSubtitulos != null) panelSubtitulos.SetActive(false);
    }

    public void Hablar(string texto)
    {
        // CERROJO DE SEGURIDAD: 
        // Si no tengo clave o he apagado el sistema desde Firebase, bloqueo la llamada para ahorrar saldo.
        if (!sistemaActivo || string.IsNullOrEmpty(apiKey))
        {
            Debug.LogWarning("La IA está apagada en la BD o falta la API Key. No gastaré saldo.");
            return;
        }

        StartCoroutine(PedirAudioAElevenLabs(texto));
    }

    private IEnumerator PedirAudioAElevenLabs(string texto)
    {
        // 1. Enciendo el panel y muestro los puntos suspensivos para dar feedback al jugador
        if (panelSubtitulos != null) panelSubtitulos.SetActive(true);
        if (textoSubtitulos != null) textoSubtitulos.text = "...";

        // 2. Preparo el paquete de datos que voy a mandar a la web
        string url = "https://api.elevenlabs.io/v1/text-to-speech/" + voiceId;
        string jsonData = "{\"text\": \"" + texto + "\", \"model_id\": \"eleven_multilingual_v2\"}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        // 3. Hago la llamada a internet usando mi llave dinámica
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
                // Si me responde con el audio, lo guardo y lo reproduzco
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                altavozIA.clip = clip;
                altavozIA.Play();
                
                // Muestro en pantalla exactamente lo que está diciendo
                if (textoSubtitulos != null) textoSubtitulos.text = texto;

                // Espero matemáticamente lo que dura el clip antes de apagar el panel
                yield return new WaitForSeconds(clip.length);

                if (panelSubtitulos != null) panelSubtitulos.SetActive(false);
            }
            else
            {
                // Si la web de ElevenLabs me da error, informo al jugador
                Debug.LogError("Error de ElevenLabs: " + www.error);
                if (textoSubtitulos != null) textoSubtitulos.text = "Error de conexión cerebral...";
                
                yield return new WaitForSeconds(2f);
                if (panelSubtitulos != null) panelSubtitulos.SetActive(false);
            }
        }
    }
}