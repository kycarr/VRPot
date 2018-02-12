using UnityEngine;

/**
 * Procedurally generates a simple cylinder
 **/
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CylinderMesh : MonoBehaviour {

    [SerializeField] float radius = 1f;         // radius of cylinder
    [SerializeField] float height = 1f;         // height of cylinder
    [SerializeField] int numRings = 2;          // number of rings in cylinder
    [SerializeField] int numRingPoints = 10;    // number of points to use in ring

    private Vector3[] vertices;
    private GameObject[] vertexObjects;
    private Mesh mesh;
    private MeshFilter meshFilter;

    // Set the dimensions of the cylinder then update the mesh
    public void SetDimensions(float radius, float height, int rings, int ringPoints)
    {
        this.radius = radius;
        this.height = height;
        numRings = rings;
        numRingPoints = ringPoints;
        GenerateMesh();
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
        GenerateUVs();
        GenerateColliders();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        calculateMeshTangents(mesh);
    }

    // Generate the mesh on play
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
        vertices = new Vector3[numVertices];

        for (int r = 0; r < numRings; r++)
        {
            Vector3 center = new Vector3(0, + r * ringHeight, 0);
            for (int v = 0; v < numRingVerts; v++)
            {
                float angle = (v == numRingVerts - 1) ? 0 : v * ringAngleDist;
                Vector3 vertPos = new Vector3(center.x + radius * Mathf.Cos(angle), center.y, center.z + radius * Mathf.Sin(angle));
                vertices[r * numRingVerts + v] = vertPos;
            }
        }
        mesh.vertices = vertices;
    }

    // Generate the cylinder's triangle faces
    private void GenerateTriangles()
    {
        int numRingVerts = numRingPoints + 1;
        int numTris = numRingPoints * (numRings - 1) * 2;
        int numTriPoints = numTris * 3;
        int[] triangles = new int[numTriPoints];

        for (int r = 0; r < numRings; r++)
        {
            for (int v = 0; v < numRingVerts; v++)
            {
                int ti = (r - 1) * numRingPoints * 6 + v * 6;
                if (r == 0 || v >= numRingPoints)
                    continue;
                triangles[ti + 0] = r * numRingVerts + v;
                triangles[ti + 1] = r * numRingVerts + v + 1;
                triangles[ti + 2] = triangles[ti + 3] = (r - 1) * numRingVerts + v;
                triangles[ti + 3] = (r - 1) * numRingVerts + v;
                triangles[ti + 4] = r * numRingVerts + v + 1;
                triangles[ti + 5] = (r - 1) * numRingVerts + v + 1;
            }
        }
        mesh.triangles = triangles;
    }

    // Generate texture mapping coordinates
    private void GenerateUVs()
    {
        int numRingVerts = numRingPoints + 1;
        int numUVs = numRingVerts * numRings;
        float uvStepH = 1.0f / numRingPoints;
        float uvStepV = 1.0f / (numRings + 1);
        Vector2[] uvs = new Vector2[numUVs];

        for (int r = 0; r < numRings; r++)
        {
            for (int v = 0; v < numRingVerts; v++)
            {
                uvs[r * numRings + v] = new Vector2(v * uvStepH, r * uvStepV);
            }
        }
        mesh.uv = uvs;
    }

    // Generate a collision box
    private void GenerateColliders()
    {
        MeshCollider collider = gameObject.AddComponent<MeshCollider>();
        collider.convex = true;
        collider.sharedMesh = mesh;
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

            Vector3.OrthoNormalize(ref n, ref t);
            tangents[a].x = t.x;
            tangents[a].y = t.y;
            tangents[a].z = t.z;

            tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
        }

        mesh.tangents = tangents;
    }
}