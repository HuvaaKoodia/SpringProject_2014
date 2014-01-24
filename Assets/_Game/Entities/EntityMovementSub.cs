﻿using UnityEngine;
using System.Collections;

public enum MovementState
{
    NotMoving, Moving, Turning
};

[RequireComponent(typeof(EntityMain))]
public class EntityMovementSub : MonoBehaviour
{
    public Transform parentTransform;
	public EntityMain parentEntity;

    TileMain[,] tilemap;
    int mapWidth;
    int mapHeight;

	public int currentGridX;// { get; private set; }
	public int currentGridY;// { get; private set; }
    public MovementState currentMovement = MovementState.NotMoving;

    float movementSpeed = 2;
    float turnSpeed = 90;

    public Vector3 targetPosition;
    public int targetRotationAngle;

	bool waitBeforeMoving;

	// Use this for initialization
	void Start()
    {
		parentTransform = transform;
        parentEntity = transform.gameObject.GetComponent<EntityMain>();

        tilemap = GameObject.Find("SharedSystems").GetComponentInChildren<GameController>().TileMainMap;

		tilemap[currentGridX, currentGridY].SetEntity(parentTransform.GetComponent<EntityMain>());
        
        mapWidth = tilemap.GetLength(0);
        mapHeight = tilemap.GetLength(1);

		waitBeforeMoving = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (!waitBeforeMoving)
        	Move();

		if (currentMovement == MovementState.NotMoving)
		{
			parentTransform.eulerAngles = new Vector3(parentTransform.eulerAngles.x, targetRotationAngle, parentTransform.eulerAngles.z);
		}
	}

    public void SetPositionInGrid(int[] position)
    {
        currentGridX = position[0];
        currentGridY = position[1];
		transform.position = new Vector3(position[0]*MapGenerator.TileSize.x, transform.position.y, position[1]*MapGenerator.TileSize.z);
    }

    public bool MoveForward()
    {
        return TryToMove(targetRotationAngle);
    }

    public bool MoveBackward()
    {
        int dir = (180 + targetRotationAngle) % 360;
        return TryToMove(dir);
    }

    bool TryToMove(int direction)
    {
        //TODO CHECK OBSTACLES
        int nextX = GetNextTileX(direction, currentGridX);
        int nextY = GetNextTileY(direction, currentGridY);

        if (currentMovement == MovementState.NotMoving)
        {
            if (CanMoveToTile(nextX, nextY))
            {

                tilemap[currentGridX, currentGridY].LeaveTile();

                currentGridX = nextX;
                currentGridY = nextY;

                currentMovement = MovementState.Moving;

                targetPosition = tilemap[currentGridX, currentGridY].transform.position;

                tilemap[currentGridX, currentGridY].SetEntity(parentEntity);
                return true;
            }
            else
                return false;
        }
        else
            return true;
    }

    public void TurnLeft()
    {
		int target = (int)parentTransform.eulerAngles.y - 90;
        Turn(target);
    }

    public void TurnRight()
    {  
		int target = (int)parentTransform.eulerAngles.y + 90;
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
        //else
        //{
        //    Debug.Log("jännä finish kusi hommat");
        //    FinishMoving();
        //}
    }

    void Move()
    {
        if (currentMovement == MovementState.Moving)
        {
			Vector3 movementDir = targetPosition - parentTransform.position;
            float distance = movementDir.magnitude;
            movementDir.Normalize();

            
            if (distance > Vector3.Magnitude(movementDir * Time.deltaTime * movementSpeed))
            {
				parentTransform.position += movementDir * Time.deltaTime * movementSpeed;
            }
            else
            {
				parentTransform.position = targetPosition;
	            FinishMoving();
            }
        }

        else if (currentMovement == MovementState.Turning)
        {
			float direction = targetRotationAngle - parentTransform.eulerAngles.y;
            direction /= Mathf.Abs(direction);

			float distance = targetRotationAngle - parentTransform.eulerAngles.y;

            //to prevent 270 degree turns
            if (distance > 180)
                direction *= -1;

            //Debug.Log("direction: "+ direction + " distance: " + distance);
            if (Mathf.Abs(direction * Time.deltaTime * turnSpeed) < Mathf.Abs(distance))
            {
				parentTransform.Rotate(parentTransform.transform.up, direction * Time.deltaTime * turnSpeed);
            }
            else
            {
				parentTransform.eulerAngles = new Vector3(parentTransform.eulerAngles.x, targetRotationAngle, parentTransform.eulerAngles.z);
                FinishMoving();
            }
        }
    }

    void FinishMoving()
    {
        currentMovement = MovementState.NotMoving;
        parentEntity.FinishedMoving(false);
    }

    bool CanMoveToTile(int x, int y)
    {
		if (!Subs.insideArea(x,y,0,0,mapWidth,mapHeight))
        {
            //Debug.Log("Out of range, X: " + x + " Y: " + y);
            return false;
        }

        TileMain nextTile = tilemap[x, y];
        if ((nextTile.Data.TileType == TileObjData.Type.Floor || nextTile.Data.TileType == TileObjData.Type.Corridor) 
		    && nextTile.entityOnTile == null)
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

    //returns true if moved, false if turned
    public MovementState MoveToTile(Point3D tile, bool stayWaiting)
    {
		waitBeforeMoving = stayWaiting;

        switch (targetRotationAngle)
        {
            #region 360/0
            case 360:
            case 0:
                if (tile.Y - currentGridY == 1)
                {
                    return MoveForward() ? MovementState.Moving : MovementState.NotMoving;
                }
                else if (tile.X - currentGridX == 1)
                {
					TurnRight();
                    return MovementState.Turning;
                }
                else
                {
                    TurnLeft();
                    return MovementState.Turning;
                }
            #endregion
            #region 180
            case 180:
                if (currentGridY - tile.Y == 1)
                {
                    return MoveForward() ? MovementState.Moving : MovementState.NotMoving;
                }
                else if (currentGridX - tile.X == 1)
                {
					TurnRight();
                    return MovementState.Turning;
                }
				else
				{
					TurnLeft();
                    return MovementState.Turning;
				}
            #endregion
            #region 90
            case 90:
                if (tile.X - currentGridX == 1)
                {
                    return MoveForward() ? MovementState.Moving : MovementState.NotMoving;
                }
                else if (currentGridY - tile.Y == 1)
                {
                    TurnRight();
                    return MovementState.Turning;
                }
				else
				{
					TurnLeft();
                    return MovementState.Turning;
				}
            #endregion
            #region 270
            case 270:
                if (currentGridX - tile.X == 1)
                {
                    return MoveForward() ? MovementState.Moving : MovementState.NotMoving;
                }
                else if (tile.Y - currentGridY == 1)
                {
                    TurnRight();
                    return MovementState.Turning;
                }
                else
                {
                    TurnLeft();
                    return MovementState.Turning;
                }
            #endregion
        }

        return MovementState.NotMoving;
    }

	public void StartMoving()
	{
		waitBeforeMoving = false;
	}

	public TileMain GetCurrenTile()
	{
		return tilemap[currentGridX, currentGridY];
	}

	public TileMain GetTileInFront()
	{
		int nextX = currentGridX, nextY = currentGridY;
		if (targetRotationAngle == 0 || targetRotationAngle == 360)
		{
			nextY += 1;
		}
		else if (targetRotationAngle == 180)
		{
			nextY -= 1;
		}
		else if (targetRotationAngle == 90)
		{
			nextX += 1;
		}
		else
		{
			nextX -= 1;
		}

		if (nextX < 0 || nextX > tilemap.GetLength(0) || nextY < 0 || nextY > tilemap.GetLength(1))
			return null;

		return tilemap[nextX, nextY];
	}
}