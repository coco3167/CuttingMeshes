using System;
using System.Linq;
using UnityEngine;

public static class CuttingManager
{
    public static void CutMesh(Cuttable cuttable, Cutter cutter)
    {
        Mesh meshToCut = cuttable.GetMesh();

        Vector3[] vertices = (Vector3[])meshToCut.vertices.Clone();
        int[] triangles = (int[]) meshToCut.triangles.Clone();
        
        cuttable.transform.TransformPoints(vertices);
        cutter.ResetCutter();

        foreach (Vector3 vertice in vertices)
        {
            cutter.CalculateIsOver(vertice);
        }

        for (int loop = 0; loop < triangles.Length; loop += 3)
        {
            int[] triangle = triangles[new Range(loop, loop + 3)];
            Vector3[] triangleVertices =
            {
                vertices[triangle[0]],
                vertices[triangle[1]],
                vertices[triangle[2]],
            };
            bool[] areVerticesOver =
            {
                cutter.IsOver(triangleVertices[0]),
                cutter.IsOver(triangleVertices[1]),
                cutter.IsOver(triangleVertices[2]),
            };

            // Test if the triangle was cut
            if (areVerticesOver.Any(x => x) && areVerticesOver.Any(x => !x))
            {
                int soloVerticeIndex = GetSoloVerticeIndex(ref areVerticesOver);
                
                cutter.CreateNewVertice(triangleVertices[soloVerticeIndex], triangleVertices[(soloVerticeIndex+1)%3]);
                cutter.CreateNewVertice(triangleVertices[soloVerticeIndex], triangleVertices[(soloVerticeIndex+2)%3]);
            }
        }
        
        // Create new gameobjects for new meshes
    }

    private static int GetSoloVerticeIndex(ref bool[] areVerticesOver)
    {
        if (areVerticesOver[0] == areVerticesOver[1])
            return 2;
        
        if (areVerticesOver[0] == areVerticesOver[2])
            return 1;

        return 0;
    }
}
