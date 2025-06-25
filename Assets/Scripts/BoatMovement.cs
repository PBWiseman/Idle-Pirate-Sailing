using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class BoatMovement : MonoBehaviour
{
    private float TackSpeed = 5.0f;
    private float SailSpeed = 0;
    private float Acceleration = 3f;
    private float MaxSpeed = 20f;
    private float Deceleration = 1f;
    public GameObject boat;
    private bool canMoveRight = true;   
    private bool canMoveLeft = true;
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
        //Set the boat's position to a bit above the bottom of the camera
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found. Please ensure there is a camera tagged as 'MainCamera' in the scene.");
            return;
        }
        Vector3 worldPosition = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.1f, mainCamera.nearClipPlane));
        //Set the boat's position
        transform.position = new Vector3(transform.position.x, worldPosition.y, transform.position.z);
    }

    //Update is called once per frame
    void Update()
    {
        Vector3 direction = Vector3.forward * floatingJoystick.Vertical + Vector3.right * floatingJoystick.Horizontal;
        //If the joystick is not being used, set the direction to 0
        if (direction.magnitude < 0.1f)
        {
            direction = Vector3.zero;
        }
        //If the joystick is being used, move the boat in the direction of the joystick
        if (direction != Vector3.zero)
        {
            //Move the boat in the direction of the joystick
            transform.Translate(direction * TackSpeed * Time.deltaTime, Space.World);
            //If the rotation is not set to 0 set the rotation to 0
            if (boat.transform.rotation.eulerAngles.z > 1 && boat.transform.rotation.eulerAngles.z < 180)
            {
                boat.transform.rotation = Quaternion.identity;
            }
            //While the boat is moving, rotate it based on joystick input
            RotateBoat(floatingJoystick.Horizontal * -30f);
        }
        else
        {
            //If there is no joystick input, rotate the boat back to its original position
            boat.transform.rotation = Quaternion.Slerp(boat.transform.rotation, Quaternion.identity, 5 * Time.deltaTime);
        }
        //Increase or decrease the sail speed based on the joystick's vertical input. Minimum speed is 0, maximum speed is 10. Pushing it forward should increase the speed. The further the joystick is pushed, the faster the acceleration. Releasing the joystick should gently decrease the speed to 0. Pulling it back should slow the boat down
        if (floatingJoystick.Vertical > 0)
        {
            SailSpeed = Mathf.Min(SailSpeed + Acceleration * Time.deltaTime, MaxSpeed);
        }
        else if (floatingJoystick.Vertical < 0)
        {
            SailSpeed = Mathf.Max(SailSpeed - Deceleration * Time.deltaTime, 0f);
        }
        else
        {
            SailSpeed = Mathf.Lerp(SailSpeed, 0f, Acceleration * Time.deltaTime);
        }

        //Move the boat forward based on the sail speed
        if (SailSpeed > 0)
        {
            transform.Translate(Vector3.up * SailSpeed * Time.deltaTime, Space.World);
        }
    }

    private void RotateBoat(float rotationSpeed)
    {
        float currentZAngle = boat.transform.rotation.eulerAngles.z;
        if (currentZAngle > 180) currentZAngle -= 360; // Convert to range -180 to 180

        float newZAngle = Mathf.Clamp(currentZAngle + rotationSpeed * Time.deltaTime, -30f, 30f);
        boat.transform.rotation = Quaternion.Euler(0, 0, newZAngle);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Tilemap>() != null)
        {
            Vector2 collisionNormal = collision.contacts[0].normal;

            if (collisionNormal.x > 0)
            {
                //Collision from the left, disable right movement
                canMoveLeft = false;
            }
            else if (collisionNormal.x < 0)
            {
                //Collision from the right, disable left movement
                canMoveRight = false;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Tilemap>() != null)
        {
            canMoveRight = true;
            canMoveLeft = true;
        }
        // //If the boats Y position isn't -8 then set it to -8
        // if (transform.position.y != -8)
        // {
        //     transform.position = new Vector3(transform.position.x, -8, transform.position.z);
        // }
    }
}
