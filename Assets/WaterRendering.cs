using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterRendering : MonoBehaviour
{
    private MeshFilter meshFilter;

    private Vector3[] originalVertexPositions;

    private Vector3[] originalVertexPosWorldSpace;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();

        originalVertexPositions = meshFilter.mesh.vertices;




    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3[] vertices = meshFilter.mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            //Setting the positions to be local


            Vector3 vertexWorldPos = transform.TransformPoint(originalVertexPositions[i]);

            vertexWorldPos.y = WaterManager.instance.GetWaveHeight(vertexWorldPos.x);

            vertices[i] = transform.InverseTransformPoint(vertexWorldPos);


            //vertices[i].y = WaveManager.instance.GetWaveHeight(transform.position.x + vertices[i].x);
        }

        meshFilter.mesh.vertices = vertices;

        meshFilter.mesh.RecalculateNormals();

        meshFilter.mesh.RecalculateBounds();
    }
}
