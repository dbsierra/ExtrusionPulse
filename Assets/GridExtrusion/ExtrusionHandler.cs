using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpticalRhythm.Visuals
{
    public class ExtrusionHandler : MonoBehaviour
    {
        [SerializeField, Tooltip("The grid instancer that we will be animating")]
        GridInstance GridInstancer;
        [SerializeField, Tooltip("Noise texture")]
        FbmTexture NoiseTexture;

        // Use this for initialization
        void Start()
        {
            GridInstancer.Init();
            GridInstancer.CloneMaterial.SetTexture("_Map", NoiseTexture.Texture);
        }

        // Update is called once per frame
        void Update()
        {

        }

    }
}