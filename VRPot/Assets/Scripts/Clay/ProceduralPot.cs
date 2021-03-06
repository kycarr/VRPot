﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralPot : MonoBehaviour {

	[SerializeField] float radius = 1f;         // radius of cylinder
	[SerializeField] float height = 1f;         // height of cylinder
	[SerializeField] float thickness = 0.05f;         // height of cylinder
	[SerializeField] int numRings = 50;          // number of rings in cylinder
	[SerializeField] int numRingPoints = 50;    // number of points to use in ring
	[SerializeField] Vector3 baseCenter;        // center location of base

	private Vector3[] vertices;
	private GameObject[] vertexObjects;
	private Mesh mesh;
	private MeshFilter meshFilter;

	public void SetDimensions(float radius, float height, int rings, int ringPoints, Vector3 center)
	{
		this.radius = radius;
		this.height = height;
		numRings = rings;
		numRingPoints = ringPoints;
		baseCenter = center;
	}

	// Generate the cylinder mesh
	public void GenerateMesh()
	{
		mesh = GetComponent<MeshFilter>().mesh = new Mesh();
		mesh.name = "Procedural Cylinder Mesh";
		meshFilter = gameObject.GetComponent<MeshFilter>();
		meshFilter.mesh = mesh;

		GenerateVertices();
		GenerateTriangles();

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		//calculateMeshTangents(mesh);
	}

	private void Awake()
	{
		GenerateMesh();
	}

	// Generate the cylinder's vertices
	private void GenerateVertices()
	{
		int numRingVerts = numRingPoints + 1;
		int numVertices = numRingVerts * numRings;
		float ringHeight = height / (numRings - 1);
		float ringAngleDist = 2 * Mathf.PI / numRingPoints;
		vertices = new Vector3[numVertices * 2 + 2];

		// Outer side
		for (int r = 0; r < numRings; r++)
		{
			Vector3 center = new Vector3(baseCenter.x, baseCenter.y + r * ringHeight, baseCenter.z);
			for (int v = 0; v < numRingVerts; v++)
			{
				float angle = (v == numRingVerts - 1) ? 0 : v * ringAngleDist;
				Vector3 vertPos = new Vector3(center.x + radius * Mathf.Cos(angle), center.y, center.z + radius * Mathf.Sin(angle));
				vertices[r * numRingVerts + v] = vertPos;
			}
		}

		// Inner side 
		int offset = numVertices;
		for (int r = 0; r < numRings; r++) {
			Vector3 center = new Vector3 (baseCenter.x, baseCenter.y + r * ringHeight, baseCenter.z);
			for (int v = 0; v < numRingVerts; v++) {
				float angle = (v == numRingVerts - 1) ? 0 : v * ringAngleDist;
				Vector3 vertPos = new Vector3 (center.x + (radius - thickness) * Mathf.Cos (angle), center.y, center.z + (radius - thickness) * Mathf.Sin(angle));
				vertices [r * numRingVerts + v + offset] = vertPos;
			}
		}

		// Two bottom centers
		offset *= 2;
		Vector3 center_outer = new Vector3(baseCenter.x, baseCenter.y, baseCenter.z);
		Vector3 center_inner = new Vector3(baseCenter.x, baseCenter.y + thickness, baseCenter.z);
		vertices [offset] = center_outer;
		vertices [offset + 1] = center_inner;

		mesh.vertices = vertices;
	}

	// Generate the cylinder's triangle faces
	private void GenerateTriangles()
	{
		int numRingVerts = numRingPoints + 1;
		int numTris = numRingPoints * (numRings - 1) * 2;
		int numTriPoints = numTris * 3;
		int[] triangles = new int[numTriPoints * 2 + numRingVerts * 12];

		// Outer side
		for (int r = 0; r < numRings; r++)
		{
			for (int v = 0; v < numRingVerts; v++)
			{
				int ti = (r - 1) * numRingPoints * 6 + v * 6;
				if (r == 0 || v >= numRingPoints)
					continue;
				triangles[ti + 0] = r * numRingVerts + v;
				triangles[ti + 1] = r * numRingVerts + v + 1;
				triangles[ti + 2] = (r - 1) * numRingVerts + v;
				triangles[ti + 3] = (r - 1) * numRingVerts + v;
				triangles[ti + 4] = r * numRingVerts + v + 1;
				triangles[ti + 5] = (r - 1) * numRingVerts + v + 1;
			}
		}

		// Inner side
		int offset_vtx = numRingVerts * numRings; 
		int offset_tri = numTriPoints;
		for (int r = 0; r < numRings; r++)
		{
			for (int v = 0; v < numRingVerts; v++)
			{
				int ti = (r - 1) * numRingPoints * 6 + v * 6;
				if (r == 0 || v >= numRingPoints)
					continue;
				triangles[ti + 0 + offset_tri] = r * numRingVerts + v + offset_vtx;
				triangles[ti + 2 + offset_tri] = r * numRingVerts + v + 1 + offset_vtx;
				triangles[ti + 1 + offset_tri] = (r - 1) * numRingVerts + v + offset_vtx;
				triangles[ti + 3 + offset_tri] = (r - 1) * numRingVerts + v + offset_vtx;
				triangles[ti + 5 + offset_tri] = r * numRingVerts + v + 1 + offset_vtx;
				triangles[ti + 4 + offset_tri] = (r - 1) * numRingVerts + v + 1 + offset_vtx;
			}
		}

		// Top side
		offset_tri *= 2;
		int offset = (numRings - 1) * numRingVerts;
		for (int v = 0; v < numRingPoints; v++) {
			triangles [offset_tri] = v + offset;
			triangles [offset_tri + 1] = v + offset_vtx + offset;
			triangles [offset_tri + 2] = v + 1 + offset;
			triangles [offset_tri + 3] = v + offset_vtx + offset;
			triangles [offset_tri + 4] = v + offset_vtx + 1 + offset;
			triangles [offset_tri + 5] = v + 1 + offset;
			offset_tri += 6;
		}

		// Bottom side 
		for (int v = 0; v < numRingPoints; v++) {
			triangles [offset_tri] = v;
			triangles [offset_tri + 1] = v + 1;
			triangles [offset_tri + 2] = offset_vtx * 2;
			offset_tri += 3;
		}

		for (int v = 0; v < numRingPoints; v++) {
			triangles [offset_tri] = v + offset_vtx + 1;
			triangles [offset_tri + 1] = v + offset_vtx;
			triangles [offset_tri + 2] = offset_vtx * 2 + 1;
			offset_tri += 3;
		}


		mesh.triangles = triangles;
	}

	private void calculateMeshTangents(Mesh mesh)
	{
		//speed up math by copying the mesh arrays
		int[] triangles = mesh.triangles;
		Vector3[] vertices = mesh.vertices;
		Vector2[] uv = mesh.uv;
		Vector3[] normals = mesh.normals;

		//variable definitions
		int triangleCount = triangles.Length;
		int vertexCount = vertices.Length;

		Vector3[] tan1 = new Vector3[vertexCount];
		Vector3[] tan2 = new Vector3[vertexCount];

		Vector4[] tangents = new Vector4[vertexCount];

		for (long a = 0; a < triangleCount; a += 3)
		{
			long i1 = triangles[a + 0];
			long i2 = triangles[a + 1];
			long i3 = triangles[a + 2];

			Vector3 v1 = vertices[i1];
			Vector3 v2 = vertices[i2];
			Vector3 v3 = vertices[i3];

			Vector2 w1 = uv[i1];
			Vector2 w2 = uv[i2];
			Vector2 w3 = uv[i3];

			float x1 = v2.x - v1.x;
			float x2 = v3.x - v1.x;
			float y1 = v2.y - v1.y;
			float y2 = v3.y - v1.y;
			float z1 = v2.z - v1.z;
			float z2 = v3.z - v1.z;

			float s1 = w2.x - w1.x;
			float s2 = w3.x - w1.x;
			float t1 = w2.y - w1.y;
			float t2 = w3.y - w1.y;

			float r = 1.0f / (s1 * t2 - s2 * t1);

			Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
			Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

			tan1[i1] += sdir;
			tan1[i2] += sdir;
			tan1[i3] += sdir;

			tan2[i1] += tdir;
			tan2[i2] += tdir;
			tan2[i3] += tdir;
		}

		for (long a = 0; a < vertexCount; ++a)
		{
			Vector3 n = normals[a];
			Vector3 t = tan1[a];

			//Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
			//tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);
			Vector3.OrthoNormalize(ref n, ref t);
			tangents[a].x = t.x;
			tangents[a].y = t.y;
			tangents[a].z = t.z;

			tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
		}

		mesh.tangents = tangents;
	}
}
