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
    public float acceleration = 0.0f;
    /**
     * Acceleration in m/s^2; acceleration is the rate of change of velocity, expressed
     * as meters per second per second as in velocity of N m/s increases by M m/s
     * every second. We'll use a constant rate of acceleration for simplicity,
     * but it might be neat to add a mechanic where holding the 'pedal' down
     * longer increases rate of acceleration.
     */
    public float deceleration = 0.0f;
    /**
     * Rate of change of acceleration in m/s^2, added to acceleration value per acceleration period with pedal held down.
     */
    public float accelerationRate = 2.0f;
    /**
     * Rate of change of deceleration in m/s^2, subtracted from acceleration value per deceleration period with pedal released.
     */
    public float decelerationRate = 2.0f;
    /**
     * Starting speed that increases/decreases with acceleration;
     * forward or backward (depth input axis) increases speed by 
     * acceleration over time, and releasing decreases speed by same.
     */
    public float speed = 0.0f;
    /**
     * Rotation speed in degrees/second.
     */
    public float turnSpeed = 55.0f;
    /**
     * Time period at which acceleration events occur in seconds.
     */
    public float accelerationEventPeriod = 1.0F;
    InputAction moveAction;
    UnityEvent accelerationEvent = new UnityEvent();
    /**
     * Tracks pedal pressed state in current acceleration period event.
     */
    private bool pedalPressed = false;
    /**
     * Tracks pedal pressed state from previous acceleration period event.
     */
    private bool previousPedalPressed = false;
    /**
     * Tracks whether we are moving forward or backward by multiplying the forward vector by 1 or -1 based on forward or reverse input.
     */
    private float forwardDirectionFactor = 1.0F;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        moveAction.started += OnPedalPressedEvent;
        moveAction.canceled += OnPedalReleasedEvent;
        accelerationEvent.AddListener(OnAccelerationPeriodEvent);
        ClockworkTasks clocks = gameObject.GetComponent<ClockworkTasks>();
        clocks.LaunchClock("AccelEvent", accelerationEvent, 0, true, accelerationEventPeriod);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();

        // move vehicle forward
        transform.Translate(Vector3.forward * Time.deltaTime * speed * forwardDirectionFactor);
        // yaw rotate vehicle via user horizontal axis input
        transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * moveValue[0]);
    }

    void OnPedalPressedEvent(CallbackContext context)
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        // No forward input shouldn't immediately stop us; instead, we only track positive vs. negative and let deceleration slow us.
        if (moveValue[1] != 0)
        {
            forwardDirectionFactor = moveValue[1];
        }
        Debug.Log($"{context.action} started as pedal is pressed");
        pedalPressed = true;
    }

    void OnPedalReleasedEvent(CallbackContext context)
    {
        Debug.Log($"{context.action} started as pedal is released");
        pedalPressed = false;
    }

    /**
     * Modifies acceleration and speed over time. Goal is linear acceleration and exponential speed.
     */
    void OnAccelerationPeriodEvent()
    {
        // check to see if pedal action was maintained through an accel period
        if (previousPedalPressed == pedalPressed)
        {
            if (pedalPressed)
            {
                acceleration += accelerationRate;
                speed += acceleration;
            }
            else
            {
                if (speed > 0.0F)
                {
                    deceleration += decelerationRate;
                    speed -= deceleration;
                }
                else
                {
                    acceleration = 0.0F;
                    deceleration = 0.0F;
                }
            }
        }
        speed = Mathf.Clamp(speed, 0.0F, 100.0F);
        Debug.Log("Accel event! Speed: " + speed);
        previousPedalPressed = pedalPressed;
    }
}
