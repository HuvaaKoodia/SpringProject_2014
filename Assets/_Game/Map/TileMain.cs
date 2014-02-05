using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileMain : MonoBehaviour 
{
	public TileObjData Data{get;private set;}
    public GameObject TileObject;

    public EntityMain entityOnTile {get; private set;}

	public bool BlockedForMovement{
		get{
			return !((Data.TileType == TileObjData.Type.Floor
			        ||Data.TileType == TileObjData.Type.Corridor
			        ||Data.TileType==TileObjData.Type.Door
			        )&& entityOnTile == null);
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
