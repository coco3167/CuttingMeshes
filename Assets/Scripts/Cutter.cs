using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cutter : MonoBehaviour
{
    private float m_planeLastParameter;

    private Dictionary<Vector3, bool> m_calculatedVertices = new();
    private List<Vector3> m_newVertices = new();
    
    private void OnDrawGizmos()
    { 
        foreach (KeyValuePair<Vector3, bool> keyValuePair in m_calculatedVertices)
        { 
            Gizmos.color = keyValuePair.Value ? Color.blue : Color.red; 
            Gizmos.DrawSphere(keyValuePair.Key, .1f); 
        }
        //
        // Gizmos.color = Color.green;
        // foreach (Vector3 vertice in m_newVertices)
        // {
        //     Gizmos.DrawSphere(vertice, gizmosSize);
        // }
    }

    private void FixedUpdate()
    {
        m_planeLastParameter = Vector3.Dot(transform.position, transform.up);
        // Debug.Log($"d : {m_planeLastParameter}");
    }

    public void ResetCutter()
    {
        m_calculatedVertices.Clear();;
        m_newVertices.Clear();
    }

    public bool CalculateIsOver(Vector3 vertice)
    {
        if (m_calculatedVertices.TryGetValue(vertice, out bool value))
            return value;
        
        Vector3 verticeToCutterOrigin = transform.position - vertice;
        bool isOver = Vector3.Dot(verticeToCutterOrigin, transform.up) > 0;
        
        m_calculatedVertices.Add(vertice, isOver);
        return isOver;
    }

    public bool IsOver(Vector3 vertice)
    {
        return m_calculatedVertices[vertice];
    }

    public Vector3 CreateNewVertice(Vector3 firstVertice, Vector3 secondVertice)
    {
        float unNullableValue = Vector3.Dot(transform.up, secondVertice - firstVertice);
        // Debug.Log($"Unnullable Value : {unNullableValue}");
        if(unNullableValue == 0)
            return firstVertice;


        float interpolationValue = (-Vector3.Dot(transform.up, firstVertice) + m_planeLastParameter) / unNullableValue;
        // Debug.Log($"Interpolation Value : {interpolationValue}");
        interpolationValue = Math.Clamp(interpolationValue, 0, 1);
        
        Vector3 newVertice = Vector3.Lerp(firstVertice, secondVertice, interpolationValue);
        m_newVertices.Add(newVertice);

        return newVertice;
    }
}
