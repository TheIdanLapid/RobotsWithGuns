using System.Collections;
using UnityEngine;

[RequireComponent(typeof(GameObject))]
public class Orbit2 : MonoBehaviour {

    [SerializeField]
    private float rotationSpeed = 20f;
    [SerializeField]
    private float rotationTime = 3f;
    [SerializeField]
    private GameObject target;
    [SerializeField]
    private float radius = 2f;

    private float startTime;

    void Start () {
        startTime = Time.time + 1f;
    }

    void Update()
    {
        float phase = (Time.time - startTime) / rotationTime;
        float x = radius * Mathf.Cos(2 * Mathf.PI * phase) * 1.5f;
        float y = radius * Mathf.Sin(2 * Mathf.PI * phase) * 1.5f;
        transform.position = new Vector3(target.transform.position.x + x, target.transform.position.y, target.transform.position.z + y);
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
