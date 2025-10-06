using UnityEngine;

public class Cuttable : MonoBehaviour
{
    private MeshFilter m_meshFilter;
    private Mesh m_mesh;

    private void Awake()
    {
        m_meshFilter = GetComponent<MeshFilter>();
        m_mesh = m_meshFilter.sharedMesh;
    }

    public Mesh GetMesh()
    {
        return m_mesh;
    }
}
