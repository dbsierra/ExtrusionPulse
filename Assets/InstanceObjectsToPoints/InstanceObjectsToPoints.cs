using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Given a mesh, instance an object to each point on the mesh
/// </summary>
public class InstanceObjectsToPoints : MonoBehaviour {

    [Header("Template")]
    [SerializeField]
    private Mesh m_templateMesh;
    [SerializeField]
    private Vector3 m_templateScale = Vector3.one;
    [SerializeField]
    private bool m_visualizeTemplate = true;

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
           Gizmos.DrawWireMesh(m_templateMesh, transform.position, transform.rotation, m_templateScale);
        }
    }

    void Start ()
    {
        GenerateClones();
    }
    
    public void GenerateClones()
    {
        m_alignDirection = Vector3.Normalize(m_alignDirection);
        Dictionary<Vector3, int> m_positionIndexMap = new Dictionary<Vector3, int>();
        if (m_templateMesh != null)
        {
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
                clone.transform.localPosition = VectorMult(m_points[i], m_templateScale);

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
