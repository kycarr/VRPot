using UnityEngine;

// Defines shared functionality for VR controllers (Oculus Rift and HTC Vive)
public abstract class InputController : MonoBehaviour {

    protected GameObject collidingObject;           // Object that controller is currently colliding with
    protected GameObject objectInHand;              // Object that controller is currently holding
    abstract protected Vector3 Velocity();          // velocity of controller
    abstract protected Vector3 AngularVelocity();   // angular velocity of controller

    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other.gameObject);
    }

    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other.gameObject);
    }

    public void OnTriggerExit(Collider other)
    {
        RemoveCollidingObject();
    }

    // When the controller touches an object, set it up as an interaction target.
    protected void SetCollidingObject(GameObject obj)
    {
        // Don't interact if we are already interacting with something
        if (collidingObject)
        {
            return;
        }
        collidingObject = obj;
    }

    protected void RemoveCollidingObject()
    {
        if (!collidingObject)
        {
            return;
        }
        collidingObject = null;
    }

    // Interact with the object that is being touched
    protected void InteractWithObject()
    {
        // Adjust the speed of the pottery wheel
        if (collidingObject.GetComponent<PotteryWheel>())
        {
            collidingObject.GetComponent<PotteryWheel>().AdjustSpeed();
        }
        // Object can be picked up
        else if (collidingObject.GetComponent<Rigidbody>())
        {
            GrabObject();
        }
    }

    // Pick up an object and hold it with the controller
    protected void GrabObject()
    {
        objectInHand = collidingObject;
        collidingObject = null;
        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }

    // Let go of the object the controller is currently holding
    protected void ReleaseObject()
    {
        if (GetComponent<FixedJoint>())
        {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
            // Add the speed and rotation of the controller when the player releases the object, so the result is a realistic arc.
            objectInHand.GetComponent<Rigidbody>().velocity = Velocity();
            objectInHand.GetComponent<Rigidbody>().angularVelocity = AngularVelocity();
        }
        objectInHand = null;
    }

    // Add a joint to the controller to connect an object to
    protected FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }
}
