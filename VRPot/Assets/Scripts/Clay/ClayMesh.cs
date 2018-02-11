using UnityEngine;

public class ClayMesh : MonoBehaviour {

    public float radius = 1f;
    public float height = 1f;
    public float thickness = 0.2f;
    public int numRings = 2;
    public int numRingPoints = 10;

    private Vector3[] vertices;
    private Mesh mesh;

    // Generate the clay mesh
    public void Generate()
    {
    }
}
