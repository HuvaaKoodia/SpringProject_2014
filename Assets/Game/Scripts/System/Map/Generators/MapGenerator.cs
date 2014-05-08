using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Generates TileObjectData maps and TileObject maps from ShipObjectData.
/// </summary>
/// 
public class MapGenerator : MonoBehaviour
{
    public static Vector3 TileSize = new Vector3(3, 3, 3);
	//hardcoded icons
	public const string 
		WallIcon = "w", DoorIcon = "d", CorridorIcon = "c", FloorIcon = ".",AirlockIcon = "a",
		SpaceIcon=",",ElevatorDoorIcon="hd",ElevatorIcon="h",RoomIcon="r",RoomEndIcon="t"
	;

    public bool DEBUG_create_temp_tiles = false;
    public PrefabStore MapPrefabs;

	GameObject clone_container;

    /// <summary>
    /// Generates the TileObjectDataMap from an XMLMapData file.
    /// </summary>
    public void GenerateObjectDataMap(FloorObjData floor, MapXmlData md)
    {
        int w = md.W;
        int h = md.H;
            
		floor.ResetTileObjectMap(w, h);

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
				var data = floor.TileObjectMap [x, y] = new TileObjData();
                
                data.X = x;
                data.Y = y;

                //Hard coded tile indices
                var index=md.map_data[x, y];
                switch (index)
                {
                    case CorridorIcon:
                        data.SetType(TileObjData.Type.Corridor);
                        break;
                    case FloorIcon:
                        data.SetType(TileObjData.Type.Floor);
                        break;
                    case SpaceIcon:
                        data.SetType(TileObjData.Type.Empty);
                        break;
                    case WallIcon:
                        data.SetType(TileObjData.Type.Wall);
                        break;
                    case DoorIcon:
						data.SetType(TileObjData.Type.Door);
                        break;
					case AirlockIcon:
						data.SetType(TileObjData.Type.Airlock);
						break;
					case ElevatorIcon:
						data.SetType(TileObjData.Type.ElevatorTrigger);
						break;
					case ElevatorDoorIcon:
						data.SetType(TileObjData.Type.ElevatorDoor);
					break;
                }
                var obj_index=ShipGenerator.GetObjectIndices(index);
                if (obj_index!=null){
                    data.SetType(TileObjData.Type.Floor);
                    data.SetObj(obj_index);
                }
            }
        }

		//remove doors next to nothing
		for (int x = 0; x < w; x++)
		{
			for (int y = 0; y < h; y++)
			{
				var data = floor.TileObjectMap [x, y];
				if (data.TileType==TileObjData.Type.Door)
				{
					bool Delete=true;
					for(int i=0;i<2;i++){
						int x1=0,y1=0,x2=0,y2=0;
						
						if (i==0)       {x1=1;  y1=0;x2=-1;  y2=0;}
						else if (i==1)  {x1=0;  y1=1;x2=0;  y2=-1;}
						
						var t1=floor.GetTileObj(x+x1,y+y1);
						var t2=floor.GetTileObj(x+x2,y+y2);
						bool t1ok=(t1!=null&&(t1.TileType==TileObjData.Type.Corridor||t1.TileType==TileObjData.Type.Floor));
						bool t2ok=(t2!=null&&(t2.TileType==TileObjData.Type.Corridor||t2.TileType==TileObjData.Type.Floor));
						
						if (t1ok&&t2ok){
							Delete=false;
							break;
						}
					}
					if (Delete){
						data.SetType(TileObjData.Type.Wall);
					}
				}
			}
		}
    }

    /// <summary>
    /// Generates the 3d world objects to the scene from the ObjectDataMap.
    /// </summary>
	public void GenerateSceneMap(GameController GC,FloorObjData floor)
    {
        //map loading
		int w = floor.TileMapW;
		int h = floor.TileMapH;

		if (clone_container==null) clone_container = new GameObject("WorldObjects");
        
		var floor_container = new GameObject("Floor "+floor.FloorIndex);
		var tile_container = new GameObject("Tiles");
		var enemy_container = new GameObject("Enemies");
		var object_container = new GameObject("Objects");

		GC.FloorContainers.Add(floor_container);

		floor_container.transform.parent = clone_container.transform;

		object_container.transform.parent = floor_container.transform;
		enemy_container.transform.parent = floor_container.transform;
		tile_container.transform.parent = floor_container.transform;

		floor.ResetTileMainMap(w,h);

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                //tile
                //int y_pos = h - 1 - y;
				Vector2 entity_pos =new Vector2( x,y); 
                var tile_pos = new Vector3(x * TileSize.x, 0, y * TileSize.z);
				var tile = floor.TileMainMap [x, y] = Instantiate(MapPrefabs.TilePrefab, tile_pos, Quaternion.identity) as TileMain;
				tile.SetData(floor.TileObjectMap [x, y]);
                tile.transform.parent = tile_container.transform;

                //tile mesh
				SetTileGraphics(x, y, tile, floor.TileObjectMap,GC,floor);
                if (tile.TileGraphics != null)
                    tile.TileGraphics.transform.parent = tile.transform;
				else if (tile.TileObject!=null){
					tile.TileObject.transform.parent=tile.transform;
				}

                //game objects
                switch (tile.Data.ObjType)
                {
	            case TileObjData.Obj.Player:
					floor.AirlockPositions.Add(entity_pos);
	                break;
	            case TileObjData.Obj.Enemy:
	                var newEnemy = GameObject.Instantiate(MapPrefabs.EnemyPrefab, tile_pos, Quaternion.identity) as EnemyMain;
	                newEnemy.name = "Enemy";
	                newEnemy.movement.SetPositionInGrid(entity_pos);
					floor.Enemies.Add(newEnemy);

					newEnemy.CurrentFloorIndex=floor.FloorIndex;
                
                	newEnemy.transform.parent = enemy_container.transform;

					newEnemy.movement.Init();
	                break;

	            case TileObjData.Obj.Loot:
	                var LootCrate = GameObject.Instantiate(MapPrefabs.LootCratePrefab, tile_pos, Quaternion.identity) as GameObject;

					LootCrateMain crate = LootCrate.GetComponent<LootCrateMain>();
					crate.GC = GC;

					floor.LootCrates.Add(crate);
       				tile.TileObject=LootCrate;
                	tile.TileObject.transform.parent = tile.transform;
	                break;
				case TileObjData.Obj.NavigationTerminal:
				case TileObjData.Obj.ArmoryTerminal:
				case TileObjData.Obj.CargoTerminal:
				case TileObjData.Obj.EngineTerminal:
					var terminal = GameObject.Instantiate(MapPrefabs.DataTerminalPrefab, tile_pos, Quaternion.identity) as DataTerminalMain;
					terminal.GC = GC;
					terminal.SetType(tile.Data.ObjType);

					tile.TileObject=terminal.gameObject;
					tile.TileObject.transform.parent = tile.transform;
					break;
				case TileObjData.Obj.GatlingGun:
					var gatlingTurret = GameObject.Instantiate(MapPrefabs.GatlingTurretPrefab) as GatlingEnemySub;

					gatlingTurret.name = "GatlingTurret";
					gatlingTurret.CurrentFloorIndex=floor.FloorIndex;

					gatlingTurret.transform.position = tile_pos + Vector3.up * (MapGenerator.TileSize.y - 0.1f);
					gatlingTurret.movement.SetPositionInGrid(entity_pos);
					floor.Enemies.Add(gatlingTurret);

					gatlingTurret.transform.parent = enemy_container.transform;

					gatlingTurret.movement.Init();
					//gatlingTurret.movement.GetCurrenTile().LeaveTile();
					break;
                }

				//rotation
				if (tile.Data.ObjType!=TileObjData.Obj.None&&tile.TileObject!=null){
					var rotation=tile.Data.ObjXml.rotation;
					if (rotation<0)
						rotation=Subs.GetRandom(4)*90;
					tile.TileObject.transform.rotation=Quaternion.AngleAxis(rotation,Vector3.up);
				}
            }
        }
    }

	public void InitPlayer(PlayerMain player, FloorObjData floor){

		var StartPos = Subs.GetRandom (floor.AirlockPositions);
		//var tile_pos = new Vector3 (StartPos.x * TileSize.x, 0, StartPos.y * TileSize.z);

		player.CurrentFloorIndex=floor.FloorIndex;
		
		player.movement.SetPositionInGrid (StartPos);
		player.movement.Init();
		player.transform.parent = clone_container.transform;
		
		//rotate player towards non airlock door
		for(int i=0;i<4;i++){
			int x=(int)StartPos.x,y=(int)StartPos.y;
			int xx=GetCardinalX(i),yy=GetCardinalY(i);
			
			var t=floor.GetTileMain(x+xx,y-yy);
			if (t!=null&&t.Data.TileType==TileObjData.Type.Door){
				bool isairlock=t.TileObject.GetComponent<DoorMain>().isAirlockDoor;
				
				if (isairlock) continue;
				
				//Not Airlock -> rotate towards this
				var dir=Mathf.Atan2(yy,xx);
				dir=Mathf.Rad2Deg*dir+90;
				player.transform.rotation=Quaternion.AngleAxis(dir,Vector3.up);
				break;
			}
		}
	}

    /// <summary>
    /// Sets the correct graphics to tiles.
    /// </summary>
    void SetTileGraphics(int x, int y, TileMain tile, TileObjData[,] grid,GameController GC,FloorObjData floor)
    {
        var rotation = Quaternion.identity;
        GameObject tileobj = null;//if floor or corridor
        bool add_as_tileobject_as_well=false;

        //gather adjacent corridor types
        TileObjData.Type[] tile_types = new TileObjData.Type[9];
    
        int xx = 0, yy = 0;
        for (int i=0; i<8; i++)
        {
            if (i == 0){xx = 1;yy = 0;}
            if (i == 1){xx = 1;yy = 1;}
            if (i == 2){xx = 0;yy = 1;}
            if (i == 3){xx = -1;yy = 1;}
            if (i == 4){xx = -1;yy = 0;}
            if (i == 5){xx = -1;yy = -1;}
            if (i == 6){xx = 0;yy = -1;}
            if (i == 7){ xx = 1;yy = -1;}

            if (Subs.insideArea(x + xx, y + yy, 0, 0, grid.GetLength(0), grid.GetLength(1)))
            {
                tile_types [i] = grid [x + xx, y + yy].TileType;
            } else
            {
                tile_types [i] = TileObjData.Type.Empty;
            }
        }
    
        //test functions
        System.Func<TileObjData.Type,bool> NotWall =
            obj => {
			return obj != TileObjData.Type.Empty && obj != TileObjData.Type.Wall;
		};
		System.Func<TileObjData.Type,bool> FloorOrCorridor =
		obj => {
			return obj == TileObjData.Type.Floor || obj == TileObjData.Type.Corridor;
		};
		System.Func<TileObjData.Type,bool> ElevatorTrigger =
		obj => {
			return obj == TileObjData.Type.ElevatorTrigger;
		};
        /*
        System.Func<TileObjData.Type,bool> Floor =
        obj => {
            return obj == TileObjData.Type.Floor;};
            */
        /*System.Func<TileObjData.Type,bool> Wall =
            obj => {
            return obj == TileObjData.Type.Wall;};
            */

        switch (tile.Data.TileType)
        {
            case TileObjData.Type.Wall:
                if (DEBUG_create_temp_tiles)
                    tileobj = MapPrefabs.BasicWall;
                break;
            case TileObjData.Type.Empty:
                if (DEBUG_create_temp_tiles)
                    tileobj = MapPrefabs.BasicEmpty;
                break;
            case TileObjData.Type.Floor:
            case TileObjData.Type.Corridor:
			//check tile type
			if (CheckTypeEqual(ElevatorTrigger, tile_types, 4))
			{
				//Elevator end 1
				tileobj = MapPrefabs.Corridor_Elevator;
			} else if (CheckTypeEqual(ElevatorTrigger, tile_types, 6))
			{
				//Elevator end 2
				tileobj = MapPrefabs.Corridor_Elevator;
				rotation = Quaternion.AngleAxis(-90, Vector3.up);
			} else if (CheckTypeEqual(ElevatorTrigger, tile_types, 0))
			{
				//Elevator end 3
				tileobj = MapPrefabs.Corridor_Elevator;
				rotation = Quaternion.AngleAxis(180, Vector3.up);
			} else if (CheckTypeEqual(ElevatorTrigger, tile_types, 2))
			{
				//Elevator end 4
				tileobj = MapPrefabs.Corridor_Elevator;
				rotation = Quaternion.AngleAxis(90, Vector3.up);
			}
			else if (CheckTypeEqual(NotWall, tile_types, 0, 1, 2, 3, 4, 5, 6, 7))
                {
                    //Floor
                    tileobj = MapPrefabs.Corridor_Floor;
			} else if (CheckTypeEqual(NotWall, tile_types, 0, 1, 2, 3, 4, 5, 6))
                {
                    //Floor 1 edge 1
                    tileobj = MapPrefabs.Corridor_Floor1Edge;
			} else if (CheckTypeEqual(NotWall, tile_types, 0, 2, 3, 4, 5, 6, 7))
                {
                    //Floor 1 edge 2
                    tileobj = MapPrefabs.Corridor_Floor1Edge;
                    rotation = Quaternion.AngleAxis(-90, Vector3.up);
			} else if (CheckTypeEqual(NotWall, tile_types, 0, 1, 2, 4, 5, 6, 7))
                {
                    //Floor 1 edge 3
                    tileobj = MapPrefabs.Corridor_Floor1Edge;
                    rotation = Quaternion.AngleAxis(180, Vector3.up);
			} else if (CheckTypeEqual(NotWall, tile_types, 0, 1, 2, 3, 4, 6, 7))
                {
                    //Floor 1 edge 4
                    tileobj = MapPrefabs.Corridor_Floor1Edge;
                    rotation = Quaternion.AngleAxis(90, Vector3.up);
			} else if (CheckTypeEqual(NotWall, tile_types, 0, 1, 2, 3, 4, 6))
                {
                    //Floor 2 edges 1
                    tileobj = MapPrefabs.Corridor_Floor2Edges;
			} else if (CheckTypeEqual(NotWall, tile_types, 2, 3, 4, 5, 6, 0))
                {
                    //Floor 2 edges 2
                    tileobj = MapPrefabs.Corridor_Floor2Edges;
                    rotation = Quaternion.AngleAxis(-90, Vector3.up);
			} else if (CheckTypeEqual(NotWall, tile_types, 4, 5, 6, 7, 0, 2))
                {
                    //Floor 2 edges 3
                    tileobj = MapPrefabs.Corridor_Floor2Edges;
                    rotation = Quaternion.AngleAxis(180, Vector3.up);
			} else if (CheckTypeEqual(NotWall, tile_types, 6, 7, 0, 1, 2, 4))
                {
                    //Floor 2 edges 4
                    tileobj = MapPrefabs.Corridor_Floor2Edges;
                    rotation = Quaternion.AngleAxis(90, Vector3.up);
			} else if (CheckTypeEqual(NotWall, tile_types, 0, 1, 2, 4, 5, 6))
                {
                    //Floor opposite edges 1
                    tileobj = MapPrefabs.Corridor_FloorOppositeEdges;
			} else if (CheckTypeEqual(NotWall, tile_types, 2, 3, 4, 6, 7, 0))
                {
                    //Floor opposite edges 2
                    tileobj = MapPrefabs.Corridor_FloorOppositeEdges;
                    rotation = Quaternion.AngleAxis(-90, Vector3.up);
			} else if (CheckTypeEqual(NotWall, tile_types, 0, 1, 2, 4, 6))
                {
                    //Floor 3 edges 1
                    tileobj = MapPrefabs.Corridor_Floor3Edges;
			} else if (CheckTypeEqual(NotWall, tile_types, 2, 3, 4, 6, 0))
                {
                    //Floor 3 edges 2
                    tileobj = MapPrefabs.Corridor_Floor3Edges;
                    rotation = Quaternion.AngleAxis(-90, Vector3.up);
			} else if (CheckTypeEqual(NotWall, tile_types, 4, 5, 6, 0, 2))
                {
                    //Floor 3 edges 3
                    tileobj = MapPrefabs.Corridor_Floor3Edges;
                    rotation = Quaternion.AngleAxis(180, Vector3.up);
			} else if (CheckTypeEqual(NotWall, tile_types, 6, 7, 0, 2, 4))
                {
                    //Floor 3 edges 4
                    tileobj = MapPrefabs.Corridor_Floor3Edges;
                    rotation = Quaternion.AngleAxis(90, Vector3.up);
			} else if (CheckTypeEqual(NotWall, tile_types, 6, 7, 0, 1, 2))
                {
                    //Wall 1
                    tileobj = MapPrefabs.Corridor_OneWall;
			} else if (CheckTypeEqual(NotWall, tile_types, 0, 1, 2, 3, 4))
                {
                    //Wall 2
                    tileobj = MapPrefabs.Corridor_OneWall;
                    rotation = Quaternion.AngleAxis(-90, Vector3.up);
			} else if (CheckTypeEqual(NotWall, tile_types, 2, 3, 4, 5, 6))
                {
                    //Wall 3
                    tileobj = MapPrefabs.Corridor_OneWall;
                    rotation = Quaternion.AngleAxis(180, Vector3.up);
			} else if (CheckTypeEqual(NotWall, tile_types, 4, 5, 6, 7, 0))
                {
                    //Wall 4
                    tileobj = MapPrefabs.Corridor_OneWall;
                    rotation = Quaternion.AngleAxis(90, Vector3.up);
			} else if (CheckTypeEqual(NotWall, tile_types, 0, 2, 4, 6))
                {
                    //Crossroad
                    tileobj = MapPrefabs.Corridor_Crossroad;
			} else if (CheckTypeEqual(NotWall, tile_types, 0, 2, 3, 4))
                {
                    //wall corner 1
                    tileobj = MapPrefabs.Corridor_WallCorner;
			} else if (CheckTypeEqual(NotWall, tile_types, 2, 4, 5, 6))
                {
                    //wall corner 2
                    tileobj = MapPrefabs.Corridor_WallCorner;
                    rotation = Quaternion.AngleAxis(-90, Vector3.up);
			} else if (CheckTypeEqual(NotWall, tile_types, 4, 6, 7, 0))
                {
                    //wall corner 3
                    tileobj = MapPrefabs.Corridor_WallCorner;
                    rotation = Quaternion.AngleAxis(180, Vector3.up);
			} else if (CheckTypeEqual(NotWall, tile_types, 6, 0, 1, 2))
                {
                    //wall corner 4
                    tileobj = MapPrefabs.Corridor_WallCorner;
                    rotation = Quaternion.AngleAxis(90, Vector3.up);
			} else if (CheckTypeEqual(NotWall, tile_types, 0, 1, 2, 4))
                {
                    //wall corner Mirrored 1
                    tileobj = MapPrefabs.Corridor_WallCornerMirrored;
			} else if (CheckTypeEqual(NotWall, tile_types, 2, 3, 4, 6))
                {
                    //wall corner Mirrored 2
                    tileobj = MapPrefabs.Corridor_WallCornerMirrored;
                    rotation = Quaternion.AngleAxis(-90, Vector3.up);
                } else if (CheckTypeEqual(NotWall, tile_types, 4, 5, 6, 0))
                {
                    //wall corner Mirrored 3
                    tileobj = MapPrefabs.Corridor_WallCornerMirrored;
                    rotation = Quaternion.AngleAxis(180, Vector3.up);
                } else if (CheckTypeEqual(NotWall, tile_types, 6, 7, 0, 2))
                {
                    //wall corner Mirrored 4
                    tileobj = MapPrefabs.Corridor_WallCornerMirrored;
                    rotation = Quaternion.AngleAxis(90, Vector3.up);
                } else if (CheckTypeEqual(NotWall, tile_types, 0, 1, 2))
                {
                    //room corner 1
                    tileobj = MapPrefabs.Corridor_RoomCorner;
                } else if (CheckTypeEqual(NotWall, tile_types, 2, 3, 4))
                {
                    //room corner 2
                    tileobj = MapPrefabs.Corridor_RoomCorner;
                    rotation = Quaternion.AngleAxis(-90, Vector3.up);
                } else if (CheckTypeEqual(NotWall, tile_types, 4, 5, 6))
                {
                    //room corner 3
                    tileobj = MapPrefabs.Corridor_RoomCorner;
                    rotation = Quaternion.AngleAxis(180, Vector3.up);
                } else if (CheckTypeEqual(NotWall, tile_types, 6, 7, 0))
                {
                    //room corner 4
                    tileobj = MapPrefabs.Corridor_RoomCorner;
                    rotation = Quaternion.AngleAxis(90, Vector3.up);
                } else if (CheckTypeEqual(NotWall, tile_types, 0, 2, 4))
                {
                    //Tcrossing 1
                    tileobj = MapPrefabs.Corridor_TCrossing;
                } else if (CheckTypeEqual(NotWall, tile_types, 2, 4, 6))
                {
                    //Tcrossing 2
                    tileobj = MapPrefabs.Corridor_TCrossing;
                    rotation = Quaternion.AngleAxis(-90, Vector3.up);
                } else if (CheckTypeEqual(NotWall, tile_types, 4, 6, 0))
                {
                    //Tcrossing 3
                    tileobj = MapPrefabs.Corridor_TCrossing;
                    rotation = Quaternion.AngleAxis(180, Vector3.up);
                } else if (CheckTypeEqual(NotWall, tile_types, 6, 0, 2))
                {
                    //Tcrossing 4
                    tileobj = MapPrefabs.Corridor_TCrossing;
                    rotation = Quaternion.AngleAxis(90, Vector3.up);
                } else if (CheckTypeEqual(NotWall, tile_types, 0, 4))
                {
                    //horizontal corridor
                    tileobj = MapPrefabs.Corridor_TwoWall;
                } else if (CheckTypeEqual(NotWall, tile_types, 2, 6))
                {
                    //vertical corridor
                    tileobj = MapPrefabs.Corridor_TwoWall;
                    rotation = Quaternion.AngleAxis(90, Vector3.up);
                } else if (CheckTypeEqual(NotWall, tile_types, 0, 2))
                {
                    //corner 1
                    tileobj = MapPrefabs.Corridor_Corner;
                } else if (CheckTypeEqual(NotWall, tile_types, 2, 4))
                {
                    //corner 2
                    tileobj = MapPrefabs.Corridor_Corner;
                    rotation = Quaternion.AngleAxis(-90, Vector3.up);
                } else if (CheckTypeEqual(NotWall, tile_types, 4, 6))
                {
                    //corner 3
                    tileobj = MapPrefabs.Corridor_Corner;
                    rotation = Quaternion.AngleAxis(180, Vector3.up);
                } else if (CheckTypeEqual(NotWall, tile_types, 6, 0))
                {
                    //corner 4
                    tileobj = MapPrefabs.Corridor_Corner;
                    rotation = Quaternion.AngleAxis(90, Vector3.up);
                }
			else if (CheckTypeEqual(NotWall, tile_types, 0))
			{
				//DeadEnd 1
				tileobj = MapPrefabs.Corridor_Deadend;
			} else if (CheckTypeEqual(NotWall, tile_types, 2))
			{
				//DeadEnd 2
				tileobj = MapPrefabs.Corridor_Deadend;
				rotation = Quaternion.AngleAxis(-90, Vector3.up);
			} else if (CheckTypeEqual(NotWall, tile_types, 4))
			{
				//DeadEnd 3
				tileobj = MapPrefabs.Corridor_Deadend;
				rotation = Quaternion.AngleAxis(180, Vector3.up);
			} else if (CheckTypeEqual(NotWall, tile_types, 6))
			{
				//DeadEnd 4
				tileobj = MapPrefabs.Corridor_Deadend;
				rotation = Quaternion.AngleAxis(90, Vector3.up);
			}
        	break;

		case TileObjData.Type.ElevatorDoor:
				tileobj = MapPrefabs.Corridor_ElevatorDoor;

				if 	(CheckTypeEqual(FloorOrCorridor, tile_types, 2, 6)||
			    	(CheckTypeEqual(FloorOrCorridor, tile_types, 2)||CheckTypeEqual(FloorOrCorridor, tile_types, 6))
			 	){
					rotation = Quaternion.AngleAxis(90, Vector3.up);
				}
				add_as_tileobject_as_well=true;
				break;
		case TileObjData.Type.ElevatorTrigger:
				tileobj = MapPrefabs.ElevatorTrigger;
				add_as_tileobject_as_well=true;
				break;
			case TileObjData.Type.Airlock:
            case TileObjData.Type.Door:
                tileobj = MapPrefabs.Corridor_Door;

				if 	(CheckTypeEqual(FloorOrCorridor, tile_types, 2, 6)||
				     (CheckTypeEqual(FloorOrCorridor, tile_types, 2)||CheckTypeEqual(FloorOrCorridor, tile_types, 6))
				     ){
					rotation = Quaternion.AngleAxis(90, Vector3.up);
				}
                
                add_as_tileobject_as_well=true;
                break;
        }
        if (tileobj != null){
            var go= Instantiate(tileobj, tile.transform.position, rotation) as GameObject;
			tile.TileGraphics= go.GetComponent<TileGraphicsSub>();

            if (add_as_tileobject_as_well){
				tile.TileObject=go;
            }

			if (tile.Data.TileType==TileObjData.Type.Door||tile.Data.TileType==TileObjData.Type.ElevatorDoor){
                var door=tile.TileObject.GetComponent<DoorMain>();

                door.GC=GC;
            }
            
			if (tile.Data.TileType==TileObjData.Type.ElevatorTrigger){
				var elevator=tile.TileObject.GetComponent<ElevatorMain>();
				elevator.GC=GC;
			}

			if (tile.Data.TileType==TileObjData.Type.Airlock){
				var door=tile.TileObject.GetComponent<DoorMain>();
				door.GC=GC;
				door.isAirlockDoor=true;
			}
        }
    }

    bool CheckTypeEqual(System.Func<TileObjData.Type,bool> test, TileObjData.Type[] tile_types, params int[] dirs)
    {
        foreach (var dir in dirs)
        {
            if (!test(tile_types [dir]))
            {
                return false;
            }
        }
        return true;
    }

    int CheckTypeAmount(System.Func<TileObjData.Type,bool> test, TileObjData.Type[] tile_types, params int[] dirs)
    {
        int a=0;
        foreach (var dir in dirs)
        {
            if (test(tile_types [dir]))
            {
                ++a;
            }
        }
        return a;
    }

    public static int GetCardinalX(int dir)
    {
        if (dir==0)return 1;
        if (dir==1)return 0;
        if (dir==2)return -1;
        if (dir==3)return 0;
        return 0;
    }
    public static int GetCardinalY(int dir)
    {
        if (dir==0)return 0;
        if (dir==1)return 1;
        if (dir==2)return 0;
        if (dir==3)return -1;
        return 0;
    }
}

