using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///  Takes "Clone" and instances it on a grid
/// </summary>
/// 
namespace OpticalRhythm.Visuals
{
    public class GridInstance : MonoBehaviour
    {

        [Header("Clone Properties")]
        [Tooltip("Object to clone")]
        public GameObject Clone;
        [Tooltip("The material assigned to each clone")]
        public Material CloneMaterial;
        [Tooltip("Scale multiplier")]
        public Vector3 CloneScale;

        [Header("Grid Properties")]
        public float Width;
        public float Height;
        public int WidthDivisions;
        public enum Pivot
        {
            Center=0,
            BottomCenter=1,
            BottomLeft=2,
            BottomRight = 3
        }
        public Pivot GridPivot;

        public void Init()
        {
            Fill();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;

            // Create matrix in this order to ensure it matches properly
            Matrix4x4 t = Matrix4x4.Translate(transform.localPosition);
            Matrix4x4 r = Matrix4x4.Rotate(transform.rotation);
            Matrix4x4 offset = Matrix4x4.Translate(GetFinalOffset());
            Gizmos.matrix = t * r * offset;

            Gizmos.DrawWireCube(Vector3.zero, new Vector3(Width, Height, 0.01f));
            
        }

        private Vector3 GetFinalOffset()
        {
            if(GridPivot == Pivot.Center)
            {
                return Vector3.zero;
            }
            else if (GridPivot == Pivot.BottomCenter)
            {
                return new Vector3(0.0f, Height / 2.0f, 0.0f);
            }
            else if(GridPivot == Pivot.BottomLeft)
            {
                return new Vector3(Width / 2.0f, Height / 2.0f, 0.0f);          
            }
            else if (GridPivot == Pivot.BottomRight)
            {
                return new Vector3(Width / -2.0f, Height / 2.0f, 0.0f);
            }
            return Vector3.zero;
        }

        private Vector3 VectorMult(Vector3 a, Vector3 b)
        {
            Vector3 n = Vector3.zero;
            n.x = a.x * b.x;
            n.y = a.y * b.y;
            n.z = a.z * b.z;
            return n;
        }

        public void Fill()
        {
            int copyAmountX = WidthDivisions;

            // Find size of clones
            float cloneSize = Width / (float)copyAmountX;

            // Number of clones in y to fit the cloneSize
            int copyAmountY = (int)Mathf.Floor(Height / cloneSize);
            Debug.Log(copyAmountX + " " + copyAmountY);

            float index = 0.0f;
            for (int i = 0; i < copyAmountX; i++)
            {
                for (int j = 0; j < copyAmountY; j++)
                {
                    GameObject clone = GameObject.Instantiate(Clone, transform);
                    clone.transform.localScale = VectorMult(new Vector3(cloneSize, cloneSize, cloneSize), CloneScale);
                    clone.transform.localPosition = new Vector3(i * cloneSize - Width / 2.0f + cloneSize / 2.0f, j * cloneSize - Height / 2.0f + cloneSize / 2.0f, 0.0f) + GetFinalOffset();

                    // row index
                    float r = (float)j / (float)(copyAmountY - 1);

                    // column index
                    float g = (float)i / (float)(copyAmountX - 1);

                    // general index
                    float b = index / (float)(copyAmountX * copyAmountY - 1);

                    MaterialPropertyBlock prop = new MaterialPropertyBlock();
                    prop.SetColor("_Color", new Color(r, g, b));
                    prop.SetFloat("_Row", j);
                    prop.SetFloat("_Col", i);
                    prop.SetFloat("_Rows", copyAmountY);
                    prop.SetFloat("_Cols", copyAmountX);
                    clone.GetComponent<MeshRenderer>().SetPropertyBlock(prop);

                    index++;
                }
            }
        }

        void Start ()
        {
        }

        void Update ()
        {
        
        }

    }
}