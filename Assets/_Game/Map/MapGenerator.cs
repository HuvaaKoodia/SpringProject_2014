using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Generates TileObjectData maps and TileObject maps from ShipObjectData.
/// </summary>
/// 
public class MapGenerator : MonoBehaviour 
{
	public static Vector3 TileSize=new Vector3(3,3,3);
	public const string WallIcon="w",DoorIcon="d";

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
					case DoorIcon:
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
		var clone_parent=new GameObject("WorldObjects");

		int w=GC.TileObjectMap.GetLength(0);
		int h=GC.TileObjectMap.GetLength(1);
		
		GC.TileMainMap=new TileMain[w,h];
		
		for (int x = 0; x < w; x++)
		{
			for (int y = 0; y < h; y++)
			{	
				//tile
				int y_pos=h-1-y;
				var tile_pos=new Vector3(x*TileSize.x, 0, y*TileSize.z);
				var tile=GC.TileMainMap[x,y]= Instantiate(MapPrefabs.TilePrefab, tile_pos, Quaternion.identity) as TileMain;
				tile.SetData(GC.TileObjectMap[x,y]);
				tile.transform.parent=clone_parent.transform;

				//tile mesh
				SetTileObject(x,y,tile,GC.TileObjectMap);
				tile.TileObject.transform.parent=tile.transform;

				//game objects
				int[] entity_pos = { x, y };

				switch (tile.Data.ObjType)
				{
					case TileObjData.Obj.Player:
					var player = GameObject.Instantiate(MapPrefabs.PlayerPrefab, tile_pos, Quaternion.identity) as PlayerMain;
                	player.name = "Player";

					GC.player=player;

					player.movement.SetPositionInGrid(entity_pos);

					player.transform.parent=clone_parent.transform;
					break;
										
					case TileObjData.Obj.Enemy:
					var newEnemy = GameObject.Instantiate(MapPrefabs.EnemyPrefab, tile_pos, Quaternion.identity) as EnemyMain;
                	newEnemy.name = "Enemy";
					newEnemy.movement.SetPositionInGrid(entity_pos);
               		GC.aiController.enemies.Add(newEnemy);

					newEnemy.transform.parent=clone_parent.transform;
					break;

					case TileObjData.Obj.Loot:
					var LootCrate = GameObject.Instantiate(MapPrefabs.LootCratePrefab, tile_pos, Quaternion.identity) as GameObject;
					LootCrate.transform.parent=clone_parent.transform;
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

	void SetTileObject (int x,int y,TileMain tile,TileObjData[,] grid)
	{
		var rotation=Quaternion.identity;
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
			tileobj=MapPrefabs.BasicDoor;
			break;
		case TileObjData.Type.Floor:
		case TileObjData.Type.Corridor:
			TileObjData.Type[] tile_types=new TileObjData.Type[9];

			int xx=0,yy=0;
			for(int i=0;i<8;i++){
				if (i==0){xx=1;yy=0;}
				if (i==1){xx=1;yy=1;}
				if (i==2){xx=0;yy=1;}
				if (i==3){xx=-1;yy=1;}
				if (i==4){xx=-1;yy=0;}
				if (i==5){xx=-1;yy=-1;}
				if (i==6){xx=0;yy=-1;}
				if (i==7){xx=1;yy=-1;}

				tile_types[i]=grid[x+xx,y+yy].TileType;
			}

			if (CheckTypeEqual(
				obj=>{return obj==TileObjData.Type.Corridor||obj==TileObjData.Type.Floor;},
				tile_types,0,4))
			{
				//horizontal corridor
				tileobj=MapPrefabs.Corridor_TwoWall;
			}else
			if (CheckTypeEqual(
				obj=>{return obj==TileObjData.Type.Corridor||obj==TileObjData.Type.Floor;},
				tile_types,2,6))
			{
				//vertical corridor
				tileobj=MapPrefabs.Corridor_TwoWall;
				rotation=Quaternion.AngleAxis(90,Vector3.up);
			}


			break;
		}
		
		tile.TileObject=Instantiate(tileobj,tile.transform.position+tileobj.transform.position,rotation) as GameObject;
	}

	bool CheckTypeEqual(System.Func<TileObjData.Type,bool> test, TileObjData.Type[] tile_types,params int[] dirs){
		foreach(var dir in dirs){
			if (!test(tile_types[dir])){
				return false;
			}
		}
		return true;
	}
}

