using UnityEngine;

public class ViveInputController : MonoBehaviour
{
    private SteamVR_TrackedObject trackedObj;       // Reference to controller being tracked
    private GameObject collidingObject;             // Object that controller is currently colliding with
    private GameObject objectInHand;                // Object that controller is currently holding
    private SteamVR_Controller.Device Controller    // Device property to provide easy access to the controller. Uses the tracked object’s index to return the controller’s input
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void Update()
    {
        // Player squeezed the trigger
        if (Controller.GetHairTriggerDown())
        {
            // If touching an object, grab it
            if (collidingObject)
            {
                GrabObject();
            }
        }

        // Player released the trigger
        if (Controller.GetHairTriggerUp())
        {
            // If holding an object, drop it
            if (objectInHand)
            {
                ReleaseObject();
            }
        }
    }

    // When the controller touches an object, set it up as an interaction target.
    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }

    // Ensures the target is set when the player holds a controller over an object for a while. Without this, the collision may fail or become buggy.
    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }

    // When the collider exits an object, remove the interaction target
    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }

        collidingObject = null;
    }

    private void SetCollidingObject(Collider col)
    {
        // Doesn’t make the GameObject a potential grab target if the player is already holding something or the object has no rigidbody.
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        // Assigns the object as a potential grab target.
        collidingObject = col.gameObject;
    }

    private void GrabObject()
    {
        // Move the GameObject inside the player’s hand and remove it from the collidingObject variable.
        objectInHand = collidingObject;
        collidingObject = null;
        // Add a new joint that connects the controller to the object using the AddFixedJoint() method below.
        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }

    private void ReleaseObject()
    {
        // Make sure there’s a fixed joint attached to the controller.
        if (GetComponent<FixedJoint>())
        {
            // Remove the connection to the object held by the joint and destroy the joint.
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
            // Add the speed and rotation of the controller when the player releases the object, so the result is a realistic arc.
            objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
            objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }
        // Remove the reference to the formerly attached object.
        objectInHand = null;
    }

    // Make a new fixed joint, add it to the controller, and then set it up so it doesn’t break easily. Finally, you return it.
    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }
}
