using UnityEngine;

public class CargarAjustes : MonoBehaviour
{
    void Awake()
    {
        float volGeneral = PlayerPrefs.GetFloat("VolumenGeneral", 100f);
        float volEfectos = PlayerPrefs.GetFloat("VolumenEfectos", 100f);

        if (AudioManager.Instance != null)
        {
            if (AudioManager.Instance.MusicaSource != null)
                AudioManager.Instance.MusicaSource.volume = volGeneral / 100f;

            if (AudioManager.Instance.SfxSource != null)
                AudioManager.Instance.SfxSource.volume = volEfectos / 100f;
        }
    }
}