using UnityEngine;
using System.Collections;

enum MovementState
{
    NotMoving, Moving, Turning
};

public class PlayerMovementSub : MonoBehaviour
{
    public GameObject parent;
    MovementState currentMovement = MovementState.NotMoving;

    float movementSpeed = 2;
    float turnSpeed = 90;

    public Vector3 targetPosition;
    public int targetRotationAngle;
	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
            Move();
	}

    void MoveForward()
    {
        //TODO CHECK OBSTACLES
        if (currentMovement == MovementState.NotMoving)
        {
            currentMovement = MovementState.Moving;
            targetPosition = parent.transform.position + parent.transform.forward * 3;
        }
    }

    void MoveBackward()
    {
        //TODO CHECK OBSTACLES
        if (currentMovement == MovementState.NotMoving)
        {
            currentMovement = MovementState.Moving;
            targetPosition = parent.transform.position - parent.transform.forward * 3;
        }
    }

    void TurnLeft()
    {
        if (currentMovement == MovementState.NotMoving)
        {
            currentMovement = MovementState.Turning;
            targetRotationAngle = (int)parent.transform.eulerAngles.y - 90;

            if (targetRotationAngle < 0)
                targetRotationAngle += 360;
        }
    }

    void TurnRight()
    {
        if (currentMovement == MovementState.NotMoving)
        {
            currentMovement = MovementState.Turning;
            targetRotationAngle = (int)parent.transform.eulerAngles.y + 90;

            if (targetRotationAngle > 360)
                targetRotationAngle -= 0;
        }
    }

    void Move()
    {
        if (currentMovement == MovementState.Moving)
        {
            Vector3 movementDir = targetPosition - parent.transform.position;
            float distance = movementDir.magnitude;
            movementDir.Normalize();

            
            if (distance > Vector3.Magnitude(movementDir * Time.deltaTime * movementSpeed))
            {
                parent.transform.position += movementDir * Time.deltaTime * movementSpeed;
            }
            else
            {
                parent.transform.position = targetPosition;
                currentMovement = MovementState.NotMoving;
                Debug.Log("Stopped moving");
            }
        }

        else if (currentMovement == MovementState.Turning)
        {
            float direction = targetRotationAngle - parent.transform.eulerAngles.y;
            direction /= Mathf.Abs(direction);

            float distance = targetRotationAngle - parent.transform.eulerAngles.y;

            //to prevent 270 degree turns
            if (distance > 180)
                direction *= -1;

            //Debug.Log("direction: "+ direction + " distance: " + distance);
            if (Mathf.Abs(direction * Time.deltaTime * turnSpeed) < Mathf.Abs(distance))
            {
                parent.transform.Rotate(parent.transform.up, direction * Time.deltaTime * turnSpeed);
            }
            else
            {
                parent.transform.eulerAngles = new Vector3(parent.transform.eulerAngles.x, targetRotationAngle, parent.transform.eulerAngles.z);
                currentMovement = MovementState.NotMoving;
                Debug.Log("Stopped turning");
            }
        }
    }
}
