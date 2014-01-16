using UnityEngine;
using System.Collections;

public enum MovementState
{
    NotMoving, Moving, Turning
};

public class EntityMovementSub : MonoBehaviour
{
    public Transform parent;

    TileMain[,] tilemap;
    int mapWidth;
    int mapHeight;

    int currentGridX = 0;
    int currentGridY = 0;

    public MovementState currentMovement = MovementState.NotMoving;

    float movementSpeed = 2;
    float turnSpeed = 90;

    public Vector3 targetPosition;
    public int targetRotationAngle;

	// Use this for initialization
	void Start()
    {
        parent = transform.root;

        tilemap = GameObject.Find("SharedSystems").GetComponentInChildren<GameController>().TileMainMap;

        tilemap[currentGridX, currentGridY].SetEntity(parent.GetComponent<EntityMain>());
        
        mapWidth = tilemap.GetLength(0);
        mapHeight = tilemap.GetLength(1);
	}
	
	// Update is called once per frame
	void Update ()
    {
        Move();
	}

    public void SetPositionInGrid(int[] position)
    {
        currentGridX = position[0];
        currentGridY = position[1];
        transform.position = new Vector3(position[0], transform.position.y, position[1]);
    }

    void MoveForward()
    {
        TryToMove(targetRotationAngle);
    }

    void MoveBackward()
    {
        int dir = (180 + targetRotationAngle) % 360;
        TryToMove(dir);
    }

    void TryToMove(int direction)
    {
        //TODO CHECK OBSTACLES
        if (currentMovement == MovementState.NotMoving)
        {
            int nextX = GetNextTileX(direction, currentGridX);
            int nextY = GetNextTileY(direction, currentGridY);

            //Debug.Log("Next X: " + nextX + " nextY: " + nextY);
            if (CanMoveToTile(nextX, nextY))
            {
                tilemap[currentGridX, currentGridY].LeaveTile();

                currentGridX = nextX;
                currentGridY = nextY;

                currentMovement = MovementState.Moving;

                targetPosition = tilemap[currentGridX, currentGridY].Data.TilePosition;

                tilemap[currentGridX, currentGridY].SetEntity(parent.gameObject.GetComponent<EntityMain>());
            }
        }
    }

    void TurnLeft()
    {
        int target = (int)parent.eulerAngles.y - 90;
        Turn(target);
    }

    void TurnRight()
    {  
        int target = (int)parent.eulerAngles.y + 90;
        Turn(target);
    }

    void Turn(int nextTarget)
    {
        if (currentMovement == MovementState.NotMoving)
        {
            currentMovement = MovementState.Turning;

            targetRotationAngle = nextTarget;

            if (targetRotationAngle < 0)
                targetRotationAngle += 360;
            else if (targetRotationAngle > 360)
                targetRotationAngle -= 360;
        }
    }

    void Move()
    {
        if (currentMovement == MovementState.Moving)
        {
            Vector3 movementDir = targetPosition - parent.position;
            float distance = movementDir.magnitude;
            movementDir.Normalize();

            
            if (distance > Vector3.Magnitude(movementDir * Time.deltaTime * movementSpeed))
            {
                parent.position += movementDir * Time.deltaTime * movementSpeed;
            }
            else
            {
                parent.position = targetPosition;
                currentMovement = MovementState.NotMoving;
            }
        }

        else if (currentMovement == MovementState.Turning)
        {
            float direction = targetRotationAngle - parent.eulerAngles.y;
            direction /= Mathf.Abs(direction);

            float distance = targetRotationAngle - parent.eulerAngles.y;

            //to prevent 270 degree turns
            if (distance > 180)
                direction *= -1;

            //Debug.Log("direction: "+ direction + " distance: " + distance);
            if (Mathf.Abs(direction * Time.deltaTime * turnSpeed) < Mathf.Abs(distance))
            {
                parent.Rotate(parent.transform.up, direction * Time.deltaTime * turnSpeed);
            }
            else
            {
                parent.eulerAngles = new Vector3(parent.eulerAngles.x, targetRotationAngle, parent.eulerAngles.z);
                currentMovement = MovementState.NotMoving;
            }
        }
    }

    bool CanMoveToTile(int x, int y)
    {
        if (x < 0 || x > mapWidth - 1 ||
            y < 0 || y > mapHeight - 1)
        {
            //Debug.Log("Out of range, X: " + x + " Y: " + y);
            return false;
        }

        if (tilemap[x, y].Data.TileType == TileObjData.Type.Floor)
            return true;
        else
            return false;
    }

    int GetNextTileX(int direction, int currentX)
    {
        if (direction == 90)
            return currentX + 1;
        else if (direction == 270)
            return currentX - 1;
        else
            return currentX;
    }

    int GetNextTileY(int direction, int currentY)
    {
        if (direction == 0 || direction == 360)
            return currentY + 1;
        else if (direction == 180)
            return currentY - 1;
        else
            return currentY;
    }
}
