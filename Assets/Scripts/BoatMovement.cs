using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class BoatMovement : MonoBehaviour
{
    private float speed = 5.0f;
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
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
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

        //Check for touch input
        // //Convert the boat's world position to screen position
        // Vector3 boatScreenPosition = Camera.main.WorldToScreenPoint(transform.position);

        // //Check if the touch is on the right or left side of the boat
        // if (touch.position.x > boatScreenPosition.x + 50 && canMoveRight)
        // {
        //     //Move the boat to the right
        //     transform.Translate(Vector3.right * speed * Time.deltaTime);
        //     //If the rotation is not set to 0 set the rotation to 0
        //     if (boat.transform.rotation.eulerAngles.z > 1 && boat.transform.rotation.eulerAngles.z < 180)
        //     {
        //         boat.transform.rotation = Quaternion.identity;
        //     }
        //     //While the boat is moving to the right, rotate it to the right
        //     RotateBoat(-30f);
        // }
        // else if (touch.position.x < boatScreenPosition.x - 50 && canMoveLeft)
        // {
        //     //Move the boat to the left
        //     transform.Translate(Vector3.left * speed * Time.deltaTime);
        //     //If the rotation is not set to 0, rotate the boat back to its original position very swiftly
        //     if (boat.transform.rotation.eulerAngles.z < 359 && boat.transform.rotation.eulerAngles.z > 180)
        //     {
        //         boat.transform.rotation = Quaternion.identity;
        //     }
        //     //While the boat is moving to the left, rotate it to the left
        //     RotateBoat(30f);
        // }
        // else
        // {
        //     boat.transform.rotation = Quaternion.Slerp(boat.transform.rotation, Quaternion.identity, 5 * Time.deltaTime);
        // }
        // //If there is no touch input, rotate the boat back to its original position
        // boat.transform.rotation = Quaternion.Slerp(boat.transform.rotation, Quaternion.identity, 5 * Time.deltaTime);

        // //If the boats X position is in the tile columns covered by the TileSpawning.Instance.coastLines array debug it
        // if (transform.position.x < TileSpawning.Instance.coastLines[0].x && transform.position.x > TileSpawning.Instance.coastLines[TileSpawning.Instance.coastLines.Count - 1].x)
        // {
        //     ShopUI.Instance.SellItems();
        // }
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
        //If the boats Y position isn't -8 then set it to -8
        if (transform.position.y != -8)
        {
            transform.position = new Vector3(transform.position.x, -8, transform.position.z);
        }
    }
}
