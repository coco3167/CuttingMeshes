using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(MeshCollider))]
public class Cuttable : MonoBehaviour
{
    private MeshFilter m_meshFilter;

    private void Awake()
    {
        m_meshFilter = GetComponent<MeshFilter>();

        Debug.Log("awake");
    }

    public Mesh GetMesh()
    {
        return m_meshFilter.mesh;
    }

    private void SetUp(Vector3[] vertices, int[] triangles)
    {
        m_meshFilter.mesh.vertices = vertices;
        m_meshFilter.mesh.triangles = triangles;

        m_meshFilter.mesh.RecalculateNormals();
        m_meshFilter.mesh.RecalculateBounds();
        
        Debug.Log(m_meshFilter.mesh.triangles.Length/3);

        GetComponent<MeshCollider>().sharedMesh = m_meshFilter.mesh;
    }

    public void CreateSubObject(Vector3[] vertices, int[] triangles)
    {
        Cuttable newCuttable = Instantiate(this, transform.parent);
        newCuttable.SetUp(vertices, triangles);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if(m_meshFilter == null)
            return;
        
        Gizmos.color = Color.blueViolet;
        foreach (Vector3 vertice in m_meshFilter.mesh.vertices)
        {
            Gizmos.DrawSphere(transform.TransformPoint(vertice), .1f);
        }
    }
}
