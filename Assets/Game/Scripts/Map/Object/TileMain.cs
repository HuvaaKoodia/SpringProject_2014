using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileMain : MonoBehaviour 
{
	public TileObjData Data{get;private set;}
    public GameObject TileGraphics,TileObject;

    public EntityMain entityOnTile {get; private set;}

	public bool BlockedForMovement{
		get{
            if (entityOnTile != null)
                return true;

            if (Data.TileType==TileObjData.Type.Door){
                var door=GetDoor();
                if (door.IsOpen)
                    return false;
                return true;
            }

			return !((Data.TileType == TileObjData.Type.Floor
			        ||Data.TileType == TileObjData.Type.Corridor
                      )&& TileObject==null);
		}
	}

	public void SetData(TileObjData data){
		Data=data;
	}

    public void SetEntity(EntityMain entity)
    {
        entityOnTile = entity;
    }

    public void LeaveTile()
    {
        entityOnTile = null;
    }

    public DoorMain GetDoor(){
        if (TileObject==null) return null;
        return TileObject.GetComponent<DoorMain>();
    }
}
