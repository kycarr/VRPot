using System.Collections.Generic;
using UnityEngine;

public class PotteryWheel : MonoBehaviour {

    [SerializeField] Material defaultMaterial;

    public float speed;                         // speed the wheel is spinning at
    private List<GameObject> objectsOnWheel;    // objects that are on the wheel

    void Start()
    {
        speed = 1f;
        objectsOnWheel = new List<GameObject>();
    }

    void Update()
    {
        transform.Rotate(transform.up * Time.deltaTime * 90f * speed);
        foreach (GameObject obj in objectsOnWheel)
        {
            // BUG: if object is flipped on a different axis, rotation moves it forward instead of rotating it
            obj.transform.Rotate(obj.transform.up * Time.deltaTime * 90f * speed);
        }
    }

    // Toggle speed of wheel between normal, fast, and off
    public void AdjustSpeed()
    {
        if (speed == 1f)
        {
            speed = 2f;
        }
        else if (speed == 2f)
        {
            speed = 0f;
        }
        else if (speed == 0f)
        {
            speed = 1f;
        }
    }

    // Collision detection
    public void OnCollisionEnter(Collision collision)
    {
        AddSpinningObject(collision.gameObject);
    }

    public void OnCollisionExit(Collision collision)
    {
        RemoveSpinningObject(collision.gameObject);
    }

    // Controller detection
    public void OnTriggerEnter(Collider other)
    {
        // Highlight the pottery wheel to show the user can interact with it
        // TODO: draw menu displaying current speed of wheel and telling user to squeeze trigger to change speed
        // TODO: make highlighted material look nicer and maybe add particle effects?
        GetComponent<MeshRenderer>().material = null;
    }

    public void OnTriggerExit(Collider other)
    {
        GetComponent<MeshRenderer>().material = defaultMaterial;
    }

    // When an object is placed on the pottery wheel, add it to the list of objects to spin
    private void AddSpinningObject(GameObject obj)
    {
        if (!objectsOnWheel.Contains(obj) && obj.GetComponent<Rigidbody>())
        {
            objectsOnWheel.Add(obj);
        }
    }

    // When an object is taken off the pottery wheel, remove it from the list of objects to spin
    private void RemoveSpinningObject(GameObject obj)
    {
        if (objectsOnWheel.Contains(obj))
        {
            objectsOnWheel.Remove(obj);
        }
    }
}