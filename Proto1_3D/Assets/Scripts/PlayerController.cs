using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 10.0f;
    public float turnSpeed;
    InputAction moveAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        Debug.Log("Move action says " + moveValue[0] + "," + moveValue[1]);

        // move vehicle forward
        transform.Translate(Vector3.forward * Time.deltaTime * speed * moveValue[1]);
        // move vehicle laterally via the editor slider for turnSpeed
        transform.Translate(Vector3.right * Time.deltaTime * turnSpeed);
        // move vehicle laterally via user horizontal axis input
        transform.Translate(Vector3.right * Time.deltaTime * speed * moveValue[0]);
    }
}
