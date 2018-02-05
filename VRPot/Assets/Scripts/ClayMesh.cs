using UnityEngine;

public class ClayMesh : MonoBehaviour {

    public float radius = 1f;
    public float height = 1f;
    public float thickness = 0.2f;
    public int numRings = 2;
    public int numRingPoints = 10;

    private GameObject outerCylinder;
    private GameObject innerCylinder;
    private Vector3[] vertices;
    private Mesh mesh;

    // Generate the clay mesh
    public void Generate()
    {
        // Generate outer clay "pipe"
        outerCylinder = new GameObject("ProceduralCylinder");
        ProceduralCylinder cOut = outerCylinder.AddComponent<ProceduralCylinder>();
        cOut.SetDimensions(radius, height, numRings, numRingPoints, transform.position);
        cOut.GenerateMesh();

        // Generate inner clay "pipe"
        outerCylinder = new GameObject("ProceduralCylinder");
        ProceduralCylinder cIn = outerCylinder.AddComponent<ProceduralCylinder>();
        cIn.SetDimensions(radius - thickness, height, numRings, numRingPoints, transform.position);
        cIn.GenerateMesh();

        // Connect inner and outer cylinders
    }
}
