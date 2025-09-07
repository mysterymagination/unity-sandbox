using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : MonoBehaviour
{
    /**
     * Acceleration in m/s^2; acceleration is the rate of change of velocity, expressed
     * as meters per second per second as in velocity of N m/s increases by M m/s
     * every second. We'll use a constant rate of acceleration for simplicity,
     * but it might be neat to add a mechanic where holding the 'pedal' down
     * longer increases rate of acceleration.
     */
    public float acceleration = 10.0f;
    /**
     * Rate of change of acceleration in m/s^2, added to acceleration value per acceleration period with pedal held down.
     */
    public float acceleration_rate = 10.0f;
    /**
     * Rate of change of deceleration in m/s^2, subtracted from acceleration value per deceleration period with pedal released.
     */
    public float deceleration_rate = 10.0f;
    /**
     * Factor by which current acceleration will be multiplied per acceleration period with pedal held down.
     */
    public float acceleration_growth_factor = 1.5f;
    /**
     * Power to which current acceleration will be raised to generate new acceleration per acceleration period with pedal held down.
     */
    public float acceleration_growth_power = 1.0f;
    /**
     * Starting speed that increases/decreases with acceleration;
     * forward or backward (depth input axis) increases speed by 
     * acceleration over time, and releasing decreases speed by same.
     */
    public float speed = 10.0f;
    public float turnSpeed = 55.0f;
    InputAction moveAction;
    UnityEvent accelerationEvent = new UnityEvent();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        moveAction.started += OnPedalPressedEvent;
        moveAction.performed += context => Debug.Log($"{context.action} performed");
        moveAction.canceled += context => Debug.Log($"{context.action} canceled");
        accelerationEvent.AddListener(OnAccelerationPeriodEvent);
        ClockworkTasks clocks = gameObject.GetComponent<ClockworkTasks>();
        clocks.LaunchClock("TestEvent", accelerationEvent, 0, true, 5);
    }

    // Update is called once per frame
    void Update()
    {
        
        // TODO: while depth axis movement pressed, increase speed by acceleration on a curve. while depth axis movement is unpressed, decrease speed by same.

        Vector2 moveValue = moveAction.ReadValue<Vector2>();

        // move vehicle forward
        transform.Translate(Vector3.forward * Time.deltaTime * speed * moveValue[1]);
        // yaw rotate vehicle via user horizontal axis input
        transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * moveValue[0]);
    }

    void OnPedalPressedEvent(CallbackContext context)
    {
        Debug.Log($"{context.action} started as pedal is pressed");
    }

    void OnAccelerationPeriodEvent()
    {
        speed += acceleration;
        Debug.Log("Speed accelerates to " + speed); 
    }
}
