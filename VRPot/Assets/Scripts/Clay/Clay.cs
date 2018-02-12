using UnityEngine;

[RequireComponent(typeof(CylinderMesh), typeof(DeformableMesh))]
public class Clay : MonoBehaviour {

    [SerializeField] float forceSensitivity;  // sensitivity to touch
    [SerializeField] float heatSensitivity;   // sensitivity to heat

    DeformableMesh deformableMesh;

    void Start()
    {
        deformableMesh = GetComponent<DeformableMesh>();
    }

    // Collision detection
    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "PotteryWheel")
        {

        }

        if (collision.gameObject.GetComponent<Rigidbody>())
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                Debug.Log(string.Format("{0} hit {1} at point {2} with separation {3}", contact.otherCollider.name, contact.thisCollider.name, contact.point, contact.separation));
                Debug.DrawRay(contact.point, contact.normal, Color.white);
            }
        }
    }

    // Controller detection
    public void OnTriggerEnter(Collider other)
    {

    }
}
