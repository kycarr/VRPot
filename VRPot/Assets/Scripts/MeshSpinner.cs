using UnityEngine;

public class MeshSpinner : MonoBehaviour {

    public float speed = 0.5f;

    void Update()
    {
        gameObject.transform.Rotate(new Vector3(0, speed, 0));
    }
}
