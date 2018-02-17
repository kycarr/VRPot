using UnityEngine;

public class MeshSpinner : MonoBehaviour {
    public float speed = 1f;
    void Update()
    {
        transform.Rotate(transform.up * Time.deltaTime * 90f * speed);
    }
}