using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Generates TileObjectData maps and TileObject maps from ShipObjectData.
/// </summary>
/// 
public class MapGenerator : MonoBehaviour 
{
	public XMLMapLoader XmlMapRead;
	public PrefabStore MapPrefabs;
	
	/// <summary>
	/// Generates the TileObjectDataMap from a XMLMapData file.
	/// DEV. Debug.
	/// </summary>
	public void GenerateObjectDataMap(GameController GC){
		
		//random generate map
		var md = XmlMapRead.Rooms["room"][0];
			
		GenerateObjectDataMap(GC,md);
	}
	/// <summary>
	/// Generates the TileObjectDataMap from a XMLMapData file.
	/// </summary>
	public void GenerateObjectDataMap(GameController GC,MapXmlData md){
		
		int w=md.W;
		int h=md.H;
			
		GC.ResetTileObjectMap(w,h);

		for (int x = 0; x < w; x++)
		{
			for (int y = 0; y < h; y++)
			{
				var data=GC.TileObjectMap[x,y]=new TileObjData();
				
				data.TilePosition=new Vector3(x,0,y);
				
				switch (md.map_data[x,y])
				{
					case "c":
					case ".":
						data.SetType(TileObjData.Type.Floor);
					    break;
					case ",":
						data.SetType(TileObjData.Type.Empty);
					    break;
					case "x":
						data.SetType(TileObjData.Type.Wall);
					    break;

                    case "p":
                        data.SetType(TileObjData.Type.Floor);
						data.SetObj(TileObjData.Obj.Player);
                        break;
                    case "e":
                        data.SetType(TileObjData.Type.Floor);
						data.SetObj(TileObjData.Obj.Enemy);	
                        break;
				}
			}
		}
	}
	
	/// <summary>
	/// Generates the 3d world objects to the scene from the ObjectDataMap.
	/// </summary>
	public void GenerateSceneMap(GameController GC)
	{
		int w=GC.TileObjectMap.GetLength(0);
		int h=GC.TileObjectMap.GetLength(1);
		
		GC.TileMainMap=new TileMain[w,h];
		
		for (int x = 0; x < w; x++)
		{
			for (int y = 0; y < h; y++)
			{	
				//tile
				int y_pos=h-1-y;
				var tile=GC.TileMainMap[x,y]= Instantiate(MapPrefabs.TilePrefab, new Vector3(x, 0, y), Quaternion.identity) as TileMain;
				tile.SetData(GC.TileObjectMap[x,y_pos]);

				GameObject tileobj=MapPrefabs.BasicFloor;
				switch (tile.Data.TileType)
				{
					case TileObjData.Type.Wall:
						tileobj=MapPrefabs.BasicWall;
					break;
					case TileObjData.Type.Empty:
						tileobj=MapPrefabs.BasicEmpty;
					break;
				}

				tile.TileObject=Instantiate(tileobj,tile.transform.position+tileobj.transform.position,Quaternion.identity) as GameObject;

				//other objects
				int[] pos = { x, y }; //pelaajan & vihujen gridisijainnin asetukseen
				switch (tile.Data.ObjType)
				{
					case TileObjData.Obj.Player:
					//Instantiate player instead 
						var player=GameObject.Find("Player");
						if (player!=null)
							player.SendMessage("SetPositionInGrid", pos);
					break;
										
					case TileObjData.Obj.Enemy:
						var newEnemy = GameObject.Instantiate(MapPrefabs.EnemyPrefab, new Vector3(x, 0, y), Quaternion.identity) as EnemyMain;
                        newEnemy.name = "Enemy";
                        newEnemy.SendMessage("SetPositionInGrid", pos);
                        GC.enemies.Add(newEnemy);
					break;
				}
				

			}
		}
	}

}