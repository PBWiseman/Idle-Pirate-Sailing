using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    //Boat movement variables

    //Tack speed is a multiplier for the rotation speed of the boat when turning
    //1f is normal speed, 2f is double speed, 0.5f is half speed, etc.
    private float TackSpeed = 1f;
    //Sail speed is the speed at which the boat is currently moving forward
    //It is increased by the joystick's vertical input and decreased by the joystick's horizontal input
    private float SailSpeed = 0;
    //Acceleration is the rate at which the boat accelerates when the joystick is pushed forward
    private float Acceleration = 3f;
    //Max speed is the maximum speed the boat can reach
    private float MaxSpeed = 20f;
    //Passive deceleration is the rate at which the boat slows down when the joystick is not being used
    private float PassiveDeceleration = .5f;
    //Active deceleration is the rate at which the boat slows down when the joystick is pulled back
    private float ActiveDeceleration = 2.5f;

    //References to the boat GameObject and the FloatingJoystick component
    [Header("Boat Assignments")]
    //The boat GameObject that this script will control
    [SerializeField]
    private GameObject boat;
    //The FloatingJoystick component that will be used to control the boat's movement
    [SerializeField]
    private FloatingJoystick floatingJoystick;

    //Start is called before the first frame update
    void Start()
    {
        if (floatingJoystick == null)
        {
            Debug.LogError("VariableJoystick is not assigned in the BoatMovement script.");
            return;
        }
        if (boat == null)
        {
            Debug.LogError("Boat GameObject is not assigned in the BoatMovement script.");
            return;
        }
    }

    //Update is called once per frame
    void Update()
    {
        //Take the horizontal input from the joystick and rotate the boat based on that input
        float horizontalInput = floatingJoystick.Horizontal;
        //if the horizontal input is not 0, rotate the boat based on that input
        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            RotateBoat(horizontalInput * -30f);
        }

        //Take the vertical input from the joystick and move the boat forward based on that input
        float verticalInput = floatingJoystick.Vertical;
        VerticalMovement(verticalInput);

        //Move the boat forward based on the sail speed and current ship rotation
        if (SailSpeed > 0)
        {
            // Move the boat forward in the direction it is facing
            Vector3 forwardMovement = boat.transform.up * SailSpeed * Time.deltaTime;
            transform.Translate(forwardMovement, Space.World);
            // transform.Translate(Vector3.up * SailSpeed * Time.deltaTime, Space.World);
        }
        //clamp the z axis of the boat as this is a 2D game
        Vector3 boatPosition = transform.position;
        boatPosition.z = 0f; // Ensure the z position is always 0
        transform.position = boatPosition;
    }

    private void RotateBoat(float rotationSpeed)
    {
        //Modify the rotation speed based on the tack speed
        rotationSpeed *= TackSpeed;
        float currentZAngle = boat.transform.rotation.eulerAngles.z;
        if (currentZAngle > 180) currentZAngle -= 360; // Convert to range -180 to 180
        boat.transform.rotation = Quaternion.Euler(0, 0, currentZAngle + rotationSpeed * Time.deltaTime);
    }

    private void VerticalMovement(float verticalInput)
    {
        //Increase or decrease the sail speed based on the joystick's vertical input.
        //Pushing it forward increases the speed. The further the joystick is pushed, the faster the acceleration. Releasing the joystick will gently decrease the speed to 0. Pulling it back will slow the boat down
        if (verticalInput > 0)
        {
            SailSpeed = Mathf.Min(SailSpeed + Acceleration * Time.deltaTime, MaxSpeed);
        }
        else if (verticalInput < 0)
        {
            SailSpeed = Mathf.Max(SailSpeed - ActiveDeceleration * Time.deltaTime, 0f);
        }
        else if (SailSpeed > 0)
        {
            //If the joystick is not being used, gently decrease the sail speed to 0
            SailSpeed = Mathf.Lerp(SailSpeed, 0f, PassiveDeceleration * Time.deltaTime);
            //If the speed is very low, set it to 0 to avoid floating point errors
            if (SailSpeed < 0.1f)
            {
                SailSpeed = 0f;
            }
        }
    }
}
