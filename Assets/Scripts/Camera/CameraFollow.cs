using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;       // Player
    public float smoothSpeed = 5f; // Semakin besar → semakin cepat mengejar
    public Vector3 offset = new Vector3(0f, 0f, -10f); // Tetap jaga posisi Z kamera

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
