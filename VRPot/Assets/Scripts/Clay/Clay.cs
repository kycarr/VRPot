using UnityEngine;

public class Clay : MonoBehaviour {

	[SerializeField] float minDistance = 0.1f;
	[SerializeField] float force = 10f;
    [SerializeField] float pointSensitivity = 0.2f;  // distance at which points are affected by contact

    private DeformableMesh deformableMesh;
    private Rigidbody rigidBody;

    void Start()
    {
        deformableMesh = GetComponent<DeformableMesh>();
        rigidBody = GetComponent<Rigidbody>();
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody>())
        {
			if (collision.contacts [0].separation < minDistance)
			{
				deformableMesh.AddDeformingForce(collision.contacts[0].point, -collision.contacts[0].normal, force, pointSensitivity);
			}
        }
    }

    public void SetDeformable(bool isDeformable)
    {
        GetComponent<MeshCollider>().convex = !isDeformable;
        rigidBody.isKinematic = isDeformable;
        rigidBody.useGravity = !isDeformable;
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
    }
}