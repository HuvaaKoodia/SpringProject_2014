using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileMain : MonoBehaviour 
{
	public TileObjData Data{get;private set;}
	public TileGraphicsSub TileGraphics;
	public GameObject TileObject;

    public EntityMain entityOnTile {get; private set;}

	void Start()
	{
		//as long as there are TileGraphics and TileLights, turn on 50% of the white lights in the TilePrefabs at random
		if(TileGraphics != null)
		{
			if(TileGraphics.TileLights != null)
			{
				TileGraphics.TileLights.EnableLights(Subs.RandomBool(), Environment_Light.WhiteLight);
			}
		}
	}

	void Update()
	{
		//as long as there are TileGraphs, TileLights and TileLights' light_flicker has been enabled, make lights flicker
		if(TileGraphics != null)
		{
			if(TileGraphics.TileLights != null)
			{
				if(TileGraphics.TileLights.light_flicker)
				{
					TileGraphics.TileLights.Flicker(TileGraphics.TileLights.delay, Subs.RandomBool(), Environment_Light.WhiteLight);
				}
			}
		}
	}

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

	/// <summary>
	/// Does a null check HUZZAA!
	/// </summary>
	public void ShowGraphicsSafe(bool show){
		if (TileGraphics!=null) TileGraphics.GraphicsObject.SetActive(show);
	}

	/// <summary>
	/// Doesn't null check BEWARE!
	/// </summary>
	public void ShowGraphicsUnsafe(bool show){
		TileGraphics.GraphicsObject.SetActive(show);
	}
}
