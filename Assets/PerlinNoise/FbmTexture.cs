using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FbmTexture : MonoBehaviour {

    public enum TestTarget
    {
        Noise1D, Noise2D, Noise3D
    }

    [SerializeField]
    TestTarget _target;
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

                texture.SetPixel(x, y, Color.white * (n / 1.4f + 0.5f) );
            }
        }

        texture.Apply();
    }

    void Update () {
        if (_target == TestTarget.Noise1D)
            UpdateTexture((x, y, t) => Perlin.Noise(x + t));
        else if (_target == TestTarget.Noise2D)
            UpdateTexture((x, y, t) => Perlin.Noise(x + t, y));
        else
            UpdateTexture((x, y, t) => Perlin.Noise(x, y, t));
    }
}
