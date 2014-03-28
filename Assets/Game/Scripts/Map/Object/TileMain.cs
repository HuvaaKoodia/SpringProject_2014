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
		//as long as there are TileGraphics and TileLights
		if(TileGraphics != null)
		{
			if(TileGraphics.TileLights != null)
			{
				//TileGraphics.TileLights.SetState(TileGraphics.TileLights.RandomizeStartState(30, 50, 20));	//randomize start state of the white lights
				//TileGraphics.TileLights.SetState(Lighting_State.Normal);
				//TileGraphics.TileLights.SetPowerOn(Subs.RandomBool());										//randomize whether electricity should flow
				TileGraphics.TileLights.power_on = Subs.RandomBool();										//randomize whether electricity should flow
				//TileGraphics.TileLights.power_on = true;
				//TileGraphics.TileLights.lighting_state = Lighting_State.Flickering;
				TileGraphics.TileLights.EnableLights(4.0f);													//set the intensity of the white lights
			}
		}
	}
	
	void Update()
	{
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
