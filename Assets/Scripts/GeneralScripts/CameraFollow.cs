using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Assign player
    public Vector3 offset;   // tweak for better framing of cam to player
    public float smoothSpeed = 0.125f; //makes it so the camera doesnt make the player wanna die playing ur game

    void LateUpdate()
    {
        //allows the camera to follow the player so the player doesnt essentially run off screen.
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
    }
}

