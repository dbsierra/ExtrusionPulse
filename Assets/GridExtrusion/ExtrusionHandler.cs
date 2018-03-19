using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace OpticalRhythm.Visuals
{
    public class ExtrusionHandler : MonoBehaviour
    {
        [SerializeField, Tooltip("The grid instancer that we will be animating")]
        GridInstance GridInstancer;
        [SerializeField, Tooltip("Noise texture")]
        NoisePulseTexture NoiseTexture;

        public RenderTexture Movie;

        // Use this for initialization
        void Start()
        {
            GridInstancer.Init();
            if(Movie != null)
            {
                GridInstancer.CloneMaterial.SetTexture("_Map", Movie);
            }
            else
            {
                GridInstancer.CloneMaterial.SetTexture("_Map", NoiseTexture.Texture);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

    }
}