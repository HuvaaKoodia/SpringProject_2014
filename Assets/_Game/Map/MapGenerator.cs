using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour 
{
	public XMLMapLoader XmlMapRead;
	public PrefabStore MapPrefabs;
	
	public void GenerateObjectDataMap(GameController GC){
		
		//random generate map
		var md = XmlMapRead.Maps[0];
			
		int w=md.map_data.GetLength(0);
		int h=md.map_data.GetLength(1);
			
		GC.ResetTileObjectMap(w,h);

		for (int x = 0; x < w; x++)
		{
			for (int y = 0; y < h; y++)
			{
				var data=GC.TileObjectMap[x,y]=new TileObjData();
				
				data.TilePosition=new Vector3(x,0,y);
				
				switch (md.map_data[x,y])
				{
					case ".":
						data.SetType(TileObjData.Type.Floor);
					break;
					case "x":
						data.SetType(TileObjData.Type.Wall);
					break;
				}
			}
		}
	}
	/// <summary>
	/// Generates the 3d world objects to the scene.
	/// </summary>
	public void GenerateSceneMap(GameController GC)
	{
		var md = XmlMapRead.Maps[0];
		
		int w=md.map_data.GetLength(0);
		int h=md.map_data.GetLength(1);
		
		GC.TileMainMap=new TileMain[w,h];
		
		for (int x = 0; x < w; x++)
		{
			for (int y = 0; y < h; y++)
			{	
				var tile=GC.TileMainMap[x,y]= Instantiate(MapPrefabs.TilePrefab, new Vector3(x, 0, y), Quaternion.identity) as TileMain;
				tile.SetData(GC.TileObjectMap[x,y]);
				
				
				GameObject tileobj=MapPrefabs.BasicFloor;
				switch (tile.Data.TileType)
				{
					case TileObjData.Type.Wall:
						tileobj=MapPrefabs.BasicWall;
					break;
				}
				
				tile.TileObject=Instantiate(tileobj,
					GC.TileObjectMap[x,y].TilePosition+tileobj.transform.position,
					Quaternion.identity) as GameObject;
			}
		}
	}

}