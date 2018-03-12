using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MidiPlayDebug : MonoBehaviour
{

    Animator AC;

    private int colorIndex;
    public Color[] colors;

    public Light pointLight;

    public void Init()
    {
        AC = GetComponent<Animator>();
    }

    public void Update()
    {
        pointLight.color = colors[colorIndex];
        GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", colors[colorIndex]);
    }

    public void Play()
    {
        if(AC != null)
        {
            colorIndex = (colorIndex + 1) % colors.Length;
            AC.SetTrigger("Pulse");
        }
    }

}
