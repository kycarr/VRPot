using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class DeformableMesh : MonoBehaviour {

	Mesh deformingMesh;
    MeshCollider deformingCollider;
    Vector3[] originalVertices;     // original, undeformed mesh
    Vector3[] displacedVertices;    // mesh after deformations
    float uniformScale = 1f;

    void Start()
    {
        deformingMesh = GetComponent<MeshFilter>().sharedMesh;
        originalVertices = deformingMesh.vertices;
        displacedVertices = new Vector3[originalVertices.Length];
        for (int i = 0; i < originalVertices.Length; i++)
        {
            displacedVertices[i] = originalVertices[i];
        }

        deformingCollider = gameObject.AddComponent<MeshCollider>();
        deformingCollider.convex = true;
        deformingCollider.sharedMesh = deformingMesh;
    }

	public void AddDeformingForce(Vector3 contactPoint, Vector3 contactNormal, float force, float radius)
    {
        contactPoint = transform.InverseTransformPoint(contactPoint);
        contactNormal = transform.InverseTransformDirection(-contactNormal);

        for (int i = 0; i < displacedVertices.Length; i++)
        {
            AddForceToVertex(i, contactPoint, contactNormal, force, radius);
        }
        deformingMesh.vertices = displacedVertices;
        deformingMesh.RecalculateBounds();
        UpdateCollider();
    }

	void AddForceToVertex(int i, Vector3 point, Vector3 normal, float force, float radius)
    {
		Vector3 pointToVertex = point - displacedVertices[i];
        pointToVertex *= uniformScale;
		normal.y = 0.0f;
		if (pointToVertex.magnitude <= radius) {
			float attenuatedForce = force * (1 - Mathf.Pow(pointToVertex.magnitude / radius, 2));
			float velocity = attenuatedForce * Time.deltaTime;
			Vector3 vertexVelocity = normal.normalized * velocity;
			displacedVertices[i] += vertexVelocity * Time.deltaTime;
		}
    }

    void UpdateCollider()
    {
        deformingCollider.sharedMesh = deformingMesh;
    }
}