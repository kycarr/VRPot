using UnityEngine;

public class ViveInputController : InputController
{
    private SteamVR_TrackedObject trackedObj;       // Reference to controller being tracked
    private SteamVR_Controller.Device Controller    // Device property to provide easy access to the controller. Uses the tracked object’s index to return the controller’s input
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }
    protected override Vector3 Velocity() { return Controller.velocity; }
    protected override Vector3 AngularVelocity() { return Controller.angularVelocity; }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void Update()
    {
        if (Controller.GetHairTriggerDown())
        {
            if (collidingObject)
            {
                InteractWithObject();
            }
        }
        if (Controller.GetHairTriggerUp())
        {
            if (objectInHand)
            {
                ReleaseObject();
            }
        }
    }
}
