﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileMain : MonoBehaviour 
{
	public TileObjData Data{get;private set;}
	public TileGraphicsSub TileGraphics;
	public GameObject TileObject;

    public EntityMain entityOnTile {get; private set;}

	public bool BlockedForMovementEnemy{
		get{
			if (entityOnTile != null){
              	//terrible hax alien can move under turret
				if (entityOnTile.GetComponent<GatlingEnemySub>()!=null){
					return false;
				}
				return true;
			}

			var door=GetDoor();
			if (door!=null){
                if (door.IsOpen)
                    return false;
                return true;
            }

			return !((Data.TileType == TileObjData.Type.Floor
			        ||Data.TileType == TileObjData.Type.Corridor
                      )&& TileObject==null);
		}
	}

	public bool BlockedForMovement{
		get{
			if (entityOnTile != null){
				return true;
			}
			
			var door=GetDoor();
			if (door!=null){
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

	/// <summary>
	/// Does a null check HUZZAA!
	/// </summary>
	public void ShowGraphicsSafe(bool show){
		if (TileGraphics!=null) ShowGraphicsUnsafe(show);
	}

	/// <summary>
	/// Doesn't null check BEWARE!
	/// </summary>
	public void ShowGraphicsUnsafe(bool show){
		TileGraphics.GraphicsObject.SetActive(show);
		if (TileObject!=null){
			TileObject.SetActive(show);
		}
	}
}
