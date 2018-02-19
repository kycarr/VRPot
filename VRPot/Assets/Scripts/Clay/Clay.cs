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
        // Clay was put on pottery wheel
        if (collision.gameObject.GetComponent<PotteryWheel>())
        {
            SetKinematic(true);
        }
        else if (collision.gameObject.GetComponent<Rigidbody>())
        {
			if (collision.contacts [0].separation < minDistance)
			{
				deformableMesh.AddDeformingForce(collision.contacts[0].point, -collision.contacts[0].normal, force, pointSensitivity);
			}
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<PotteryWheel>())
        {
            SetKinematic(false);
        }
    }

    private void SetKinematic(bool isKinematic)
    {
        rigidBody.isKinematic = isKinematic;
        rigidBody.useGravity = !isKinematic;
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
    }
}