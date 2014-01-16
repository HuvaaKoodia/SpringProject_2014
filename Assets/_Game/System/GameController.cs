using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
	
	//public List<TileObjData> TileObjects{get;private set;}
	//public List<TileMain> Tiles{get;private set;}
	
	public MapGenerator MapGen;
	
	public TileObjData[,] TileObjectMap;
	public TileMain[,] TileMainMap;
	
	// Use this for initialization
	void Start () {
		//TileObjects=new List<TileObjData>();
		//Tiles=new List<TileMain>();
		
		MapGen.GenerateObjectDataMap(this);
		MapGen.GenerateSceneMap(this);
	}
	
	// Update is called once per frame
	void Update (){
				
	}

	/// <summary>
	/// Resets the tile object map.
	/// Doesn't create objects.
	/// </summary>
	public void ResetTileObjectMap (int w, int h)
	{
		TileObjectMap=new TileObjData[w,h];
	}
	
	/// <summary>
	/// Resets the tile main map.
	/// Doesn't create objects.
	/// </summary>
	public void ResetTileMainMap (int w, int h)
	{
		TileMainMap=new TileMain[w,h];
	}
}
