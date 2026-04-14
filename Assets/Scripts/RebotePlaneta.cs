using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RebotePlaneta : MonoBehaviour
{
    public ParticleSystem particulasClic;

    private Vector3 tamanoOriginal;
    private float velocidadRebote = 10f;
    private float cantidadEncogimiento = 0.9f;

    void Start()
    {
        tamanoOriginal = transform.localScale;
    }

    public void PlayClick()
    {
        transform.localScale = tamanoOriginal * cantidadEncogimiento;

        if (particulasClic != null)
        {
            particulasClic.Play();
        }
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            tamanoOriginal,
            Time.deltaTime * velocidadRebote
        );
    }
}
