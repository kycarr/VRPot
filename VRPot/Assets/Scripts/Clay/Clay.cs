using UnityEngine;

public class Clay : MonoBehaviour {

    [SerializeField] float forceSensitivity = 0.1f;  // sensitivity to touch
    [SerializeField] float heatSensitivity;          // sensitivity to heat

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
            foreach (ContactPoint contact in collision.contacts)
            {
                deformableMesh.AddDeformingForce(contact.point, contact.normal, 10f);
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