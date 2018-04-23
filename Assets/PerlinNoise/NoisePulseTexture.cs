using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoisePulseTexture : MonoBehaviour {

    [SerializeField]
    int size = 28;

    [SerializeField, Range(1, 5)]
    int _fractalLevel = 1;
    [SerializeField]
    float speed = 1.0f;
    [SerializeField]
    float frequency = 1.0f;
    [SerializeField]
    float amplitude = 1.0f;
    [SerializeField]
    float power = 1.0f;
    [SerializeField]
    bool clamp = false;

    Texture2D texture;
    public Texture2D Texture { get { return texture; } }

    // Pulses
    // distances and values are all in pixels
    [Tooltip("distance to fade in pixels")]
    public int FadeDistance = 7;
    [Range(0,1)]
    public float WavePos1;
    [Range(0, 1)]
    public float WavePos2;
    [Range(0, 1)]
    public float WavePos3;
    int _WavePos1;
    int _WavePos2;
    int _WavePos3;

    void Awake()
    {
        texture = new Texture2D(size, size, TextureFormat.RGBAFloat, false, true);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        GetComponent<Renderer>().material.mainTexture = texture;
    }

    void UpdateTexture(System.Func<float, float, float, float> generator)
    {
        var scale = frequency / size;
        var time = Time.time * speed;

        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                float n = generator.Invoke(x * scale, y * scale, time);

               // n = Mathf.Max(n, 0.0f);

                // Re-map to 0 - 1
                 n += .5f;

                n *= amplitude;

                if (clamp)
                {
                    n = Mathf.Clamp(n, 0.0f, 1.0f);
                }

                n = Mathf.Pow(n, power);

                if( n < .001f)
                {
                    // Debug.Log(n);
                }

                float d1 = 0.0f;
                float d2 = 0.0f;
                float d3 = 0.0f;

                // Pulse
                if (x < _WavePos1)
                {
                    d1 = 1.0f - Mathf.Clamp( (float)(_WavePos1 - x)/(float)FadeDistance, 0.0f, 1.0f);
                }
                if (x < _WavePos2)
                {
                    d2 = 1.0f - Mathf.Clamp((float)(_WavePos2 - x) / (float)FadeDistance, 0.0f, 1.0f);
                }
                if (x < _WavePos3)
                {
                    d2 = 1.0f - Mathf.Clamp((float)(_WavePos3 - x) / (float)FadeDistance, 0.0f, 1.0f);
                }

                Color finalC = Color.white * n;// * (d1+d2+d3);

                texture.SetPixel(x, y, finalC);
            }
        }

        texture.Apply();
    }

    public void Pulse()
    {
        ;
    }

    void Update () {

        _WavePos1 = (int)Mathf.Clamp(Mathf.Floor(WavePos1*(size+FadeDistance)), 0, size + FadeDistance);
        _WavePos2 = (int)Mathf.Clamp(Mathf.Floor(WavePos2 * (size + FadeDistance)), 0, size + FadeDistance);
        _WavePos3 = (int)Mathf.Clamp(Mathf.Floor(WavePos3 * (size + FadeDistance)), 0, size + FadeDistance);

        UpdateTexture((x, y, t) => Perlin.Noise(x, y, t));

    }
}
