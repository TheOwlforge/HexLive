using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class toh_mesh
{
    public static Mesh getMesh()
    {
        List<List<int>> p = Permutation.Permute(new int[] { 0, -1, -2 });
        p.AddRange(Permutation.Permute(new int[] { 0, -1, 2 }));
        p.AddRange(Permutation.Permute(new int[] { 0, 1, -2 }));
        p.AddRange(Permutation.Permute(new int[] { 0, 1, 2 }));

        List<Vector3> vertices = new List<Vector3>();
        for (int i = 0; i < 24; i++)
        {
            vertices.Add(new Vector3(p[i][0], p[i][1], p[i][2]));
        }
        vertices.Sort(CompareVectors);

        Vector3[] newVertices = new Vector3[24];
        //Vector3 position = this.transform.position;
        Vector3 position = new Vector3(0,0,0);

        for (int i = 0; i < 24; i++)
        {
            newVertices[i] = position + vertices[i];
        }

        int[]  newTriangles = new int[6*2*3 + 8*4*3];
        int[] offset = new int[12*3] { 0,2,1,1,2,3,4,5,6,6,5,7,
                                       0,1,2,2,1,3,4,6,5,5,6,7,
                                       0,2,1,1,2,3,4,5,6,6,5,7};
        for(int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 12; j++)
            {
                newTriangles[i * 12 + j] = i * 8 + offset[i * 12 + j];
            }
        }
        int[] offset_hex = new int[12] { 0,2,1,0,3,2,0,5,3,5,4,3 };
        List<int[]> hexagons = new List<int[]>();
        hexagons.Add(new int[6] { 2, 3, 12, 14, 22, 20 });
        hexagons.Add(new int[6] { 23, 22, 14, 15, 7, 6 });
        hexagons.Add(new int[6] { 5, 7, 15, 13, 18, 19 });
        hexagons.Add(new int[6] { 3, 1, 16, 18, 13, 12 });
        hexagons.Add(new int[6] { 8, 0, 2, 20, 21, 10 });
        hexagons.Add(new int[6] { 10, 21, 23, 6, 4, 11 });
        hexagons.Add(new int[6] { 11, 4, 5, 19, 17, 9 });
        hexagons.Add(new int[6] { 9, 17, 16, 1, 0, 8 });
        
        for (int i = 0; i < hexagons.Count; i++)
        {
            for(int j = 0; j < 12; j++)
            {
                newTriangles[36 + i * 12 + j] = hexagons[i][offset_hex[j]];
            }
        }

        Vector3[] vertices_wo_sharing = new Vector3[newTriangles.Length];
        for (int i = 0; i < newTriangles.Length; i++)
        {
            vertices_wo_sharing[i] = newVertices[newTriangles[i]];
            newTriangles[i] = i;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices_wo_sharing;
        mesh.triangles = newTriangles;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        return mesh;
    }

    public static int CompareVectors(Vector3 value1, Vector3 value2)
    {
        Vector3 diff1 = new Vector3(1,-1,-2);
        Vector3 diff2 = new Vector3(2, 1, -1);

        int idx1 = -1;
        idx1 = Math.Abs(value1.x) == 2 ? 0 : idx1;
        idx1 = Math.Abs(value1.y) == 2 ? 1 : idx1;
        idx1 = Math.Abs(value1.z) == 2 ? 2 : idx1;
        int idx2 = -1;
        idx2 = Math.Abs(value2.x) == 2 ? 0 : idx2;
        idx2 = Math.Abs(value2.y) == 2 ? 1 : idx2;
        idx2 = Math.Abs(value2.z) == 2 ? 2 : idx2;

        if(idx1 != idx2)
        {
            return idx1.CompareTo(idx2);
        }
        if (value1[idx1] != value2[idx2])
        {
            return value1[idx1].CompareTo(value2[idx2]);
        }
        if (value1[idx1+(int)diff1[idx1]] != value2[idx2 + (int)diff1[idx2]])
        {
            return value1[idx1 + (int)diff1[idx1]].CompareTo(value2[idx2 + (int)diff1[idx2]]);
        }
        return value1[idx1 + (int)diff2[idx1]].CompareTo(value2[idx2 + (int)diff2[idx2]]);
    }
}
