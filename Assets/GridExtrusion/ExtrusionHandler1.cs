using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace OpticalRhythm.Visuals
{
    public class ExtrusionHandler1 : MonoBehaviour
    {
        [SerializeField, Tooltip("The grid instancer that we will be animating")]
        Material Mat;
        [SerializeField, Tooltip("Noise texture")]
        NoisePulseTexture NoiseTexture;

        public RenderTexture Movie;

        // Use this for initialization
        void Start()
        {
            if(Movie != null)
            {
                Mat.SetTexture("_Map", Movie);
            }
            else
            {
                Mat.SetTexture("_Map", NoiseTexture.Texture);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

    }
}