using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CuttingManager
{
    private static Cuttable _cuttable;
    private static Cutter _cutter;
    
    // Original Mesh
    private static Mesh _meshToCut;
    private static Vector3[] _vertices;
    private static int[] _triangles;
    
    // Original triangle
    private static Vector3[] _triangleVertices = new Vector3[3];
    private static bool[] _areVerticesOver = new bool[3];

    private static List<int> _verticeCorrespondingIndex = new();
    
    // New vertices
    private static List<Vector3> _overVertices = new();
    private static List<Vector3> _underVertices = new();
    
    //New triangles
    private static List<int> _overTriangles = new();
    private static List<int> _underTriangles = new();
    
    public static void CutMesh(Cuttable cuttable, Cutter cutter)
    {
        ResetCuttingManager();
        
        _cuttable = cuttable;
        _cutter = cutter;
        
        _meshToCut = cuttable.GetMesh();
        _vertices = (Vector3[])_meshToCut.vertices.Clone();
        _triangles = (int[])_meshToCut.triangles.Clone();
        
        cuttable.transform.TransformPoints(_vertices);
        _cutter.ResetCutter();
        
        foreach (Vector3 vertice in _vertices)
        {
            SeparateOriginalVertices(vertice);
        }
        
        for (int loop = 0; loop < _triangles.Length; loop += 3)
        {
            int[] triangle = _triangles[new Range(loop, loop + 3)];
            SetupTriangle(ref triangle);

            // Test if the triangle was cut
            if (_areVerticesOver.Any(x => x) && _areVerticesOver.Any(x => !x))
            {
                PlaceCutTriangle(ref triangle); 
            }
            else
            {
                PlaceUncutTriangle(ref triangle);
            }
        }
        
        Debug.Log("Cut");
        
        // Create new gameobjects for new meshes
        _cuttable.CreateSubObject(_overVertices.ToArray(), _overTriangles.ToArray());
        _cuttable.CreateSubObject(_underVertices.ToArray(), _underTriangles.ToArray());
        
        _cuttable.DestroySelf();
    }

    private static void ResetCuttingManager()
    {
        _triangleVertices = new Vector3[3];
        _areVerticesOver = new bool[3];
        
        _verticeCorrespondingIndex = new();
        
        _overVertices = new();
        _underVertices = new();
        
        _overTriangles = new();
        _underTriangles = new();
    }

    private static int GetSoloVerticeIndex(ref bool[] areVerticesOver)
    {
        if (areVerticesOver[0] == areVerticesOver[1])
            return 2;
        
        if (areVerticesOver[0] == areVerticesOver[2])
            return 1;

        return 0;
    }

    private static void SeparateOriginalVertices(Vector3 vertice)
    {
        List<Vector3> rightVerticeList = _cutter.CalculateIsOver(vertice) ? _overVertices : _underVertices;
        rightVerticeList.Add(vertice);
        _verticeCorrespondingIndex.Add(rightVerticeList.Count-1);
    }

    private static void SetupTriangle(ref int[] triangle)
    {
        for (int index = 0; index < 3; index++)
        {
            _triangleVertices[index] = _vertices[triangle[index]];
            _areVerticesOver[index] = _cutter.IsOver(_triangleVertices[index]);
        }
    }

    private static void PlaceCutTriangle(ref int[] triangle)
    {
        int soloVerticeLocalIndex = GetSoloVerticeIndex(ref _areVerticesOver);
                
        Vector3 firstVertice = _cutter.CreateNewVertice(_triangleVertices[soloVerticeLocalIndex], _triangleVertices[(soloVerticeLocalIndex+1)%3]);
        Vector3 secondVertice = _cutter.CreateNewVertice(_triangleVertices[soloVerticeLocalIndex], _triangleVertices[(soloVerticeLocalIndex+2)%3]);

        bool isOver = _cutter.IsOver(_triangleVertices[soloVerticeLocalIndex]);
        
        // Add simple Triangle
        {
            ref List<Vector3> soloVerticeVertices = ref isOver ? ref _overVertices : ref _underVertices;
            ref List<int> soloVerticeTriangles = ref isOver ? ref _overTriangles : ref _underTriangles;

            soloVerticeVertices.Add(firstVertice);
            soloVerticeVertices.Add(secondVertice);

            // Only Triangle
            soloVerticeTriangles.Add(_verticeCorrespondingIndex[triangle[soloVerticeLocalIndex]]);
            soloVerticeTriangles.Add(soloVerticeVertices.Count - 2);
            soloVerticeTriangles.Add(soloVerticeVertices.Count - 1);
        }


        // Add other Triangles
        {
            ref List<Vector3> otherVerticeVertices = ref !isOver ? ref _overVertices : ref _underVertices;
            ref List<int> otherVerticeTriangles = ref !isOver ? ref _overTriangles : ref _underTriangles;

            otherVerticeVertices.Add(firstVertice);
            otherVerticeVertices.Add(secondVertice);

            // "Left Down" Triangle
            otherVerticeTriangles.Add(_verticeCorrespondingIndex[triangle[(soloVerticeLocalIndex + 1) % 3]]);
            otherVerticeTriangles.Add(_verticeCorrespondingIndex[triangle[(soloVerticeLocalIndex + 2) % 3]]);
            otherVerticeTriangles.Add(otherVerticeVertices.Count - 2);


            // "Right Up" Triangle
            otherVerticeTriangles.Add(otherVerticeVertices.Count - 1);
            otherVerticeTriangles.Add(otherVerticeVertices.Count - 2);
            otherVerticeTriangles.Add(_verticeCorrespondingIndex[triangle[(soloVerticeLocalIndex + 2) % 3]]);
        }
    }

    private static void PlaceUncutTriangle(ref int[] triangle)
    {
        int[] newTriangle = triangle.Select(x => _verticeCorrespondingIndex[x]).ToArray();
        (_areVerticesOver[0] ? _overTriangles : _underTriangles).AddRange(newTriangle);
    }
}
