using UnityEngine;

public class FollowCameraController : MonoBehaviour
{
    public GameObject playerVehicle;
    public Vector3 cameraOffset = new Vector3(0, 5, -7);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // LateUpdate is called once per frame after Update
    void LateUpdate()
    {
        // follow the player vehicle at configurable offset.
        transform.position = playerVehicle.transform.position + cameraOffset;
    }
}
