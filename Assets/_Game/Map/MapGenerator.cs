using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Generates TileObjectData maps and TileObject maps from ShipObjectData.
/// </summary>
/// 
public class MapGenerator : MonoBehaviour 
{
	public const string WallIcon="w";

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
						data.SetType(TileObjData.Type.Corridor);
					break;
					case ".":
						data.SetType(TileObjData.Type.Floor);
					    break;
					case ",":
						data.SetType(TileObjData.Type.Empty);
					    break;
					case WallIcon:
						data.SetType(TileObjData.Type.Wall);
					    break;
					case "d":
						data.SetType(TileObjData.Type.Door);
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
				tile.SetData(GC.TileObjectMap[x,y]);

				GameObject tileobj=MapPrefabs.BasicFloor;//if floor or corridor
				switch (tile.Data.TileType)
				{
					case TileObjData.Type.Wall:
						tileobj=MapPrefabs.BasicWall;
					break;
					case TileObjData.Type.Empty:
						tileobj=MapPrefabs.BasicEmpty;
					break;
					case TileObjData.Type.Door:
						//DEV:HAXtileobj=MapPrefabs.BasicDoor;
						tileobj=MapPrefabs.BasicFloor;
					break;
				}

				tile.TileObject=Instantiate(tileobj,tile.transform.position+tileobj.transform.position,Quaternion.identity) as GameObject;

				//other objects
				int[] pos = { x, y }; //pelaajan & vihujen gridisijainnin asetukseen
				switch (tile.Data.ObjType)
				{
					case TileObjData.Obj.Player:
					//Instantiate player instead 
                        var player = GameObject.Instantiate(MapPrefabs.PlayerPrefab, new Vector3(x, 0, y), Quaternion.identity) as PlayerMain;
                        player.name = "Player";

						GC.player=player;

                    	player.movement.SetPositionInGrid(pos);

					break;
										
					case TileObjData.Obj.Enemy:
						var newEnemy = GameObject.Instantiate(MapPrefabs.EnemyPrefab, new Vector3(x, 0, y), Quaternion.identity) as EnemyMain;
                        newEnemy.name = "Enemy";
                        newEnemy.SendMessage("SetPositionInGrid", pos);
                        GC.aiController.enemies.Add(newEnemy);
					break;

					case TileObjData.Obj.Loot:
						var LootCrate = GameObject.Instantiate(MapPrefabs.LootCratePrefab, new Vector3(x,0, y), Quaternion.identity) as GameObject;
					break;
				}
			}
		}
	}
	
	/// <summary>
	/// Generates items (loot & enemies) to the room and corridors of the ship to the TileObjectMap
	/// Call after GenerateObjectDataMap and before GenerateSceneMap
	/// </summary>

	public void GenerateShipItems(GameController GC,ShipObjData ship){

		int current_floor=0;
		var md=ship.Floors[current_floor];
		var xml_md=ship.XmlData.Floors[current_floor];

		int w=md.W;
		int h=md.H;

		int floor_amount_enemies=Subs.GetRandom(xml_md.EnemyAmountMin,xml_md.EnemyAmountMax);
		int floor_amount_loot=Subs.GetRandom(xml_md.LootAmountMin,xml_md.LootAmountMax);

		//rooms
		List<TileObjData> free_tiles=new List<TileObjData>();
		var rooms_list=ship.FloorRooms[current_floor];
		foreach (var room in rooms_list){

			//loot crates

			GetFreeTilesOfType(GC,TileObjData.Type.Floor,free_tiles);
			int l_amount=Subs.GetRandom(room.RoomXmlData.LootAmountMin,room.RoomXmlData.LootAmountMax);
			
			while (free_tiles.Count>0){
				if (l_amount==0) break;
				
				var tile=Subs.GetRandom(free_tiles);
				free_tiles.Remove(tile);
				
				l_amount--;
				
				tile.SetObj(TileObjData.Obj.Loot);
			}

			//enemies
			GetFreeTilesOfType(GC,TileObjData.Type.Floor,free_tiles);
			int e_amount=Subs.GetRandom(room.RoomXmlData.EnemyAmountMin,room.RoomXmlData.EnemyAmountMax);

			while (free_tiles.Count>0){

				if (e_amount==0||floor_amount_enemies==0) break;

				var tile=Subs.GetRandom(free_tiles);
				free_tiles.Remove(tile);

				e_amount--;
				floor_amount_enemies--;

				tile.SetObj(TileObjData.Obj.Enemy);
			}
		}

		//add remaining enemies to corridors
		if (floor_amount_enemies>0){
			GetFreeTilesOfType(GC,TileObjData.Type.Corridor,free_tiles);
			while (free_tiles.Count>0){
				
				if (floor_amount_enemies==0) break;
				
				var tile=Subs.GetRandom(free_tiles);
				free_tiles.Remove(tile);

				floor_amount_enemies--;
				
				tile.SetObj(TileObjData.Obj.Enemy);
			}
		}
		//add items to the item crates DEV.TODO
	}

	void GetFreeTilesOfType(GameController GC,TileObjData.Type type,List<TileObjData> free_tiles){
		free_tiles.Clear();

		for (int x = 0; x < GC.TileObjectMap.GetLength(0); x++)
		{
			for (int y = 0; y < GC.TileObjectMap.GetLength(1); y++)
			{
				var tile=GC.TileObjectMap[x,y];
				if (tile.TileType==type&&tile.ObjType==TileObjData.Obj.None){
					free_tiles.Add(tile);
				}
			}
		}
	}
}

