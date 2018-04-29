using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Given a mesh, instance an object to each point on the mesh
/// </summary>
public class InstanceObjectsToPoints : MonoBehaviour {

    public enum TemplateMode
    {
        Grid = 0,
        Mesh = 1
    };
    [SerializeField]
    private TemplateMode m_templateMode;

    [SerializeField]
    private bool m_visualizeTemplate = true;

    [Header("Template Mesh Mode")]
    [SerializeField]
    private Mesh m_templateMesh;
    [SerializeField]
    private Vector3 m_templateMeshScale = Vector3.one;

    [Header("Template Grid Mode")]
    [SerializeField]
    private int m_divisionsX;
    [SerializeField]
    private int m_divisionsY;
    [SerializeField]
    private int m_divisionsZ;
    [SerializeField]
    private bool m_fitScaleToGrid;

    [Header("Cloner")]
    [SerializeField]
    private GameObject m_cloner;
    [SerializeField]
    private Vector3 m_clonerScale = Vector3.one;
    [SerializeField]
    private bool m_alignToNormal;
    [SerializeField]
    private Vector3 m_alignDirection;

    [Header("Pass Vertex Attributes to Cloner")]
    [SerializeField]
    private string m_uvAttribute;
    [SerializeField]
    private string m_colorAttribute;
    [SerializeField]
    private string m_positionAttribute;

    private Vector3[] m_points;
    private Vector3[] m_normals;

    private Vector3 VectorMult(Vector3 a, Vector3 b)
    {
        Vector3 n = Vector3.zero;
        n.x = a.x * b.x;
        n.y = a.y * b.y;
        n.z = a.z * b.z;
        return n;
    }

    private void OnDrawGizmos()
    {
        if(m_visualizeTemplate && m_templateMesh != null)
        {
           Gizmos.color = Color.cyan;
           Gizmos.DrawWireMesh(m_templateMesh, transform.position, transform.rotation, m_templateMeshScale);
        }
    }

    void Start ()
    {
        GenerateClones();
    }
    
    public void GenerateClones()
    {
        m_alignDirection = Vector3.Normalize(m_alignDirection);

        if (m_templateMode == TemplateMode.Grid)
        {
            for(int x=0; x< m_divisionsX; x++)
            {
                for (int y = 0; y < m_divisionsY; y++)
                {
                    for (int z = 0; z < m_divisionsZ; z++)
                    {
                        GameObject clone = GameObject.Instantiate(m_cloner, transform);
                        clone.transform.localScale = VectorMult(clone.transform.localScale, m_clonerScale);

                        // Get the world space bounds after scale is applied
                        MeshRenderer mr = clone.transform.GetComponentInChildren<MeshRenderer>();

                        Vector3 bounds = Vector3.one;

                        if(m_fitScaleToGrid)
                        {
                            bounds = mr.bounds.size;
                        }

                        clone.transform.localPosition = new Vector3(bounds.x * x, bounds.y * y, bounds.z * z);

                        if (m_alignToNormal)
                        {
                            clone.transform.localRotation = Quaternion.FromToRotation(m_alignDirection, Vector3.forward);
                        }

                        // Set vertex attributes on to material
                        MaterialPropertyBlock prop = new MaterialPropertyBlock();
                        if (m_uvAttribute != "" && m_uvAttribute != null)
                        {
                            float u = (float)x / (float)m_divisionsX;
                            float v = (float)y / (float)m_divisionsY;
                            float w = (float)z / (float)m_divisionsZ;
                            prop.SetVector(m_uvAttribute, new Vector3(u, v, w) );
                        }
                        if (m_positionAttribute != "" && m_positionAttribute != null)
                        {
                            prop.SetVector(m_positionAttribute, clone.transform.localPosition);
                        }
                        mr.SetPropertyBlock(prop);
                    }
                }
            }
        }
    
        else if (m_templateMesh != null)
        {
            Dictionary<Vector3, int> m_positionIndexMap = new Dictionary<Vector3, int>();

            m_points = m_templateMesh.vertices;
            m_normals = m_templateMesh.normals;

            for (int i = 0; i < m_points.Length; i++)
            {
                // Avoid cloning objects in duplicate positions (for example when your template is a mesh without shared verticies)
                if (m_positionIndexMap.ContainsKey(m_points[i]))
                {
                    continue;
                }
                else
                {
                    m_positionIndexMap[m_points[i]] = i;
                }

                GameObject clone = GameObject.Instantiate(m_cloner, transform);
                clone.transform.localScale = VectorMult(clone.transform.localScale, m_clonerScale);
                clone.transform.localPosition = VectorMult(m_points[i], m_templateMeshScale);

                if (m_normals != null && m_alignToNormal)
                {
                    clone.transform.localRotation = Quaternion.FromToRotation(m_alignDirection, m_normals[i]);
                }

                // Set vertex attributes on to material
                MaterialPropertyBlock prop = new MaterialPropertyBlock();
                if (m_uvAttribute != "" && m_uvAttribute != null)
                {
                    prop.SetVector(m_uvAttribute, m_templateMesh.uv[i]);
                }
                if (m_colorAttribute != "" && m_colorAttribute != null)
                {
                    prop.SetColor(m_colorAttribute, m_templateMesh.colors[i]);
                }
                if (m_positionAttribute != "" && m_positionAttribute != null)
                {
                    prop.SetVector(m_positionAttribute, m_points[i]);
                }

                if (clone.GetComponent<MeshRenderer>() != null)
                {
                    clone.GetComponent<MeshRenderer>().SetPropertyBlock(prop);
                }
                else if (clone.GetComponentInChildren<MeshRenderer>() != null)
                {
                    clone.GetComponentInChildren<MeshRenderer>().SetPropertyBlock(prop);
                }
            }
        }
        
    }

    void Update ()
    {
        
    }

}
