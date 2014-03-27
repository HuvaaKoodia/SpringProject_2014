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
				//TileGraphics.TileLights.power_on = Subs.RandomBool();
				TileGraphics.TileLights.EnableLights(4.0f);													//call function to turn on 50% of the white lights in the TilePrefabs at random
				//TileGraphics.TileLights.light_flicker = true;																							//call function if light flickering wants to be randomized
				//TileGraphics.TileLights.SetState(Lighting_State.Flickering);
				
			//<NOT NEEDED AT THE MOMENT>//
				//TileGraphics.TileLights.EnableLights(false, Environment_Light.AllLight);																//call function to turn on or off all the lights in the TilePrefabs
				//TileGraphics.TileLights.EnableLights(Subs.RandomBool(), Subs.GetRandomEnum<Environment_Light>());										//call function to randomize which lights to enable and turn on 50% of the white lights in the TilePrefabs at random
				//TileGraphics.TileLights.SetDelay(0.1f);																								//call function to set delay value through code instead of inspector
			//<NOT NEEDED AT THE MOMENT>//
			}
		}
	}
	
	void Update()
	{
//<NOT NEEDED AT THE MOMENT>//
//		//as long as there are TileGraphs, TileLights and TileLights' light_flicker has been enabled
//		if(TileGraphics != null)
//		{
//			if(TileGraphics.TileLights != null)
//			{
//				if(TileGraphics.TileLights.GetFlicker())
//				{
//					TileGraphics.TileLights.Flicker(TileGraphics.TileLights.GetDelay(), Subs.RandomBool(), Environment_Light.WhiteLight);				//call function to  make lights flicker
//					TileGraphics.TileLights.Flicker(TileGraphics.TileLights.GetDelay(), Subs.RandomBool(), Environment_Light.OrangeLight, 1);				//call function to  make lights flicker
//					TileGraphics.TileLights.Flicker(TileGraphics.TileLights.GetDelay(), Subs.RandomBool(), Subs.GetRandomEnum<Environment_Light>());	//call function to randomize which set of lights to flicker
//				}
//			}
//		}
//<NOT NEEDED AT THE MOMENT>//
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
