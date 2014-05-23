using UnityEngine;
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

	public bool hax_is_enemy=false;

	public int currentGridX;// { get; private set; }
	public int currentGridY;// { get; private set; }
    public MovementState currentMovement = MovementState.NotMoving;

    public float movementSpeed = 2;
    public float turnSpeed = 90;

    public Vector3 targetPosition;
    public int targetRotationAngle;

	bool waitBeforeMoving;

    public AudioClip WalkForwardFX;
	public AudioClip WalkBackwardFX;
	public AudioClip SideStepFX;

    public AudioClip TurnSoundFX;

	public bool InstantMovement;

	// Use this for initialization
	void Awake()
    {
		if (parentTransform == null)
			parentTransform = transform;

        parentEntity = transform.gameObject.GetComponent<EntityMain>();
        
		waitBeforeMoving = false;

		InstantMovement=!SharedSystemsMain.I.GOps.Data.MovementAnimations;
	}

    public void Init(){
		UpdateFloor();
		
		UpdateTileEntityToThis();
    }

	public void UpdateFloor()
	{
		tilemap = parentEntity.CurrentFloor.TileMainMap;
		mapWidth = tilemap.GetLength(0);
		mapHeight = tilemap.GetLength(1);

		targetRotationAngle=(int)parentEntity.transform.rotation.eulerAngles.y;
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

    public void SetPositionInGrid(Vector2 pos)
    {
		currentGridX = (int)pos.x;
		currentGridY = (int)pos.y;
		transform.position = new Vector3(currentGridX*MapGenerator.TileSize.x, transform.position.y, currentGridY*MapGenerator.TileSize.z);
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

	public bool MoveLeft()
	{
		int dir = (270 + targetRotationAngle) % 360;
		return TryToMove(dir);
	}

	public bool MoveRight()
	{
		int dir = (90 + targetRotationAngle) % 360;
		return TryToMove(dir);
	}

    bool TryToMove(int direction)
    {
        int nextX = GetNextTileX(direction, currentGridX);
        int nextY = GetNextTileY(direction, currentGridY);

        if (currentMovement == MovementState.NotMoving)
        {
            if (CanMoveToTile(nextX, nextY))
            {
				parentEntity.MovedLastPhase = true;
                tilemap[currentGridX, currentGridY].LeaveTile();

                currentGridX = nextX;
                currentGridY = nextY;

                currentMovement = MovementState.Moving;

                targetPosition = tilemap[currentGridX, currentGridY].transform.position;

                UpdateTileEntityToThis();

				int relationalDir = direction - targetRotationAngle;

				if (relationalDir == 0 && WalkForwardFX != null)
                {
					audio.PlayOneShot(WalkForwardFX);
                }
				else if (Mathf.Abs(relationalDir) == 180 && WalkBackwardFX != null)
				{
					audio.PlayOneShot(WalkBackwardFX);
				}
				else if (SideStepFX != null)
				{
					audio.PlayOneShot(SideStepFX);
				}

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

    public void Turn(int nextTarget)
    {
        if (currentMovement == MovementState.NotMoving)
        {
			parentEntity.MovedLastPhase = true;

            currentMovement = MovementState.Turning;

            targetRotationAngle = nextTarget;

            if (targetRotationAngle < 0)
                targetRotationAngle += 360;
            else if (targetRotationAngle > 360)
                targetRotationAngle -= 360;

            if (TurnSoundFX != null)
            {
                audio.PlayOneShot(TurnSoundFX);
            }
        }
    }

    void Move()
    {
        if (currentMovement == MovementState.Moving)
        {
			Vector3 movementDir = targetPosition - parentTransform.position;
            float distance = movementDir.magnitude;
            movementDir.Normalize();

			var speed=movementSpeed;
			if (InstantMovement){
				speed*=XmlDatabase.NoMovementAnimationsMultiplier;
			}
			Vector3 movement = movementDir * Time.deltaTime * speed;
            
            if (distance > movement.magnitude && movement != Vector3.zero)
            {
				parentTransform.position += movement;
            }
            else
            {
				parentTransform.position = targetPosition;
	            FinishMoving();
            }
        }

        else if (currentMovement == MovementState.Turning)
        {
			float distance = targetRotationAngle - parentTransform.eulerAngles.y;
            float direction = distance / Mathf.Abs(distance);

			var speed=turnSpeed;
			if (InstantMovement){
				speed*=XmlDatabase.NoMovementAnimationsMultiplier;
			}

            //to prevent 270 degree turns
            if (Mathf.Abs(distance) > 180)
                direction *= -1;

            //Debug.Log("direction: "+ direction + " distance: " + distance);
			if (Mathf.Abs(direction * Time.deltaTime * speed) < Mathf.Abs(distance))
            {
				parentTransform.rotation = Quaternion.Euler(
					parentTransform.rotation.eulerAngles.x, 
					parentTransform.rotation.eulerAngles.y + (direction * Time.deltaTime * speed),
					parentTransform.rotation.eulerAngles.z);
				//parentTransform.Rotate(parentTransform.transform.up, direction * Time.deltaTime * turnSpeed);
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

        //audio.Stop();
    }

    bool CanMoveToTile(int x, int y)
    {
		if (!Subs.insideArea(x,y,0,0,mapWidth,mapHeight))
        {
            //Debug.Log("Out of range, X: " + x + " Y: " + y);
            return false;
        }
        TileMain nextTile = tilemap[x, y];
		if (hax_is_enemy)
			return !nextTile.BlockedForMovementEnemy;
        return !nextTile.BlockedForMovement;
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
	
    public MovementState MoveToTile(Point3D tile, bool stayWaiting)
    {
		waitBeforeMoving = stayWaiting;

        switch (targetRotationAngle)
        {
            #region 360/0
            case 360:
            case 0:
                if (tile.Y - currentGridY > 0)
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
                if (currentGridY - tile.Y > 0)
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
                if (tile.X - currentGridX > 0)
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
                if (currentGridX - tile.X > 0)
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

		if (nextX < 0 || nextX >= tilemap.GetLength(0) || nextY < 0 || nextY >= tilemap.GetLength(1))
			return null;

		return tilemap[nextX, nextY];
	}

	public DoorMain CheckForDoor(){
		Component target;
		if (Subs.GetObjectMousePos(out target,MapGenerator.TileSize.x+2, "Door"))
		{
			var door=target.GetComponent<DoorMain>();
			return door;
		}
		return null;
	}

    void UpdateTileEntityToThis()
    {
        tilemap[currentGridX, currentGridY].SetEntity(parentEntity);
    }
}