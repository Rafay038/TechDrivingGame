using UnityEngine;

public class FloatSpin : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(0, 50 * Time.deltaTime, 0);
        transform.position += Vector3.up * Mathf.Sin(Time.time * 2f) * 0.001f;
    }
}
