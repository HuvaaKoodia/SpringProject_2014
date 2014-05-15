using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomXmlIndex{
    public string index,type,name;
    public bool randomize_pos=false,randomize_doors=false;
}

public class ObjectXmlIndex{
    public string index;
    public int rotation;
    public TileObjData.Obj type;
}
/// <summary>
/// Generates ShipObjectData from ShipMapData.
/// </summary>
public class ShipGenerator : MonoBehaviour 
{
    public static Dictionary<string,RoomXmlIndex> RoomIndices;
    public static Dictionary<string,ObjectXmlIndex> ObjectIndices;

    public static ObjectXmlIndex GetObjectIndices(string index)
    {
        ObjectXmlIndex out_index=null;
        ObjectIndices.TryGetValue(index,out out_index);
        return out_index;
    }

	bool IsRoomStartIndex(string index){
        foreach(var i in RoomIndices){
			if (i.Key==index)
				return true;
		}
		return false;
	}

	void FitRoom(string room_index,ShipRoomObjData cell){

		var room_stats=RoomIndices[room_index];

		MapXmlData rroom;

        List<MapXmlData> PossibleRooms=new List<MapXmlData>();
        
        foreach(var r in XmlMapRead.Rooms[room_stats.name]){
            PossibleRooms.Add(r);
        }

		while (true){
			if (PossibleRooms.Count==0){
                Debug.LogError("No fitting room in ["+cell.X+", "+cell.Y+"] "+room_index+" max size: "+cell.W+" "+cell.H);
				break;
			}
			
            rroom=Subs.GetRandom(PossibleRooms);
            PossibleRooms.Remove(rroom);
			//toobig
			if (rroom.W>cell.W||rroom.H>cell.H){
				continue;
			}
            //toosmall
            if (rroom.W<cell.W-2||rroom.H<cell.H-2){
                continue;
            }
			
			cell.RoomXmlData=rroom;
            cell.roomStats=room_stats;
			break;
		}
	}

	public XMLMapLoader XmlMapRead;
	public PrefabStore MapPrefabs;

	public ShipObjData GenerateShipObjectData(string ship_name)
	{
        ShipMapXmlData ship_xml_data=XmlMapRead.Ships[ship_name];
		ShipObjData ship_obj_data=new ShipObjData(ship_xml_data);

		for (int i=0;i<ship_xml_data.Floors.Count;++i){
			MapXmlData CurrentFloor=ship_xml_data.Floors[i];

			int w=CurrentFloor.W;
			int h=CurrentFloor.H;
			var Rooms=new List<ShipRoomObjData>();
			MapXmlData NewFloorMap=new MapXmlData(w,h);
			
			//Assign map indices and generate room cells
			for(int x=0;x<w;x++)
			{		
				for(int y=0;y<h;y++)
				{
					var index=CurrentFloor.GetIndex(x,y);
					NewFloorMap.map_data[x,y]=index;

					if (IsRoomStartIndex(index)){
						//random room
						var cell=new ShipRoomObjData();
						cell.X=x;cell.Y=y;
						cell.W=1;cell.H=1;

						Rooms.Add(cell);

						//calculate max room size
						int ww=1;
						while (true){
							var nindex=CurrentFloor.GetIndex(x+ww,y);
							if (IsRoomStartIndex(nindex)){
								break;
							}
							if (nindex==MapGenerator.RoomEndIcon){
								cell.W+=1;
								break;
							}
							if (nindex==MapGenerator.RoomIcon){
								cell.W+=1;
								++ww;
							}
							else break;
						}
						ww=1;
						while (true){
							var nindex=CurrentFloor.GetIndex(x,y+ww);
							if (IsRoomStartIndex(nindex)){
								break;
							}
							if (nindex==MapGenerator.RoomEndIcon){
	                            cell.H+=1;
	                            break;
	                        }
							if (nindex==MapGenerator.RoomIcon){
								cell.H+=1;
								++ww;
							}
							else break;
						}
						CheckLegitWalls(cell,CurrentFloor);

						//randomize a room of right size
	                    FitRoom(index,cell);
					}
				}
			}

			//Rooms

			foreach(var room in Rooms){
	            if (!room.roomStats.randomize_pos) continue;
				//check if offset needs to be tampered with
				int rw_diff=room.W-room.RoomXmlData.W,rh_diff=room.H-room.RoomXmlData.H;

				List<Vector2> PossibleFixes=new List<Vector2>();

				foreach (var dir in room.corridor_dirs){

					if (dir==2){//corridor on left side
						PossibleFixes.Add(new Vector2(0,Subs.GetRandom(rh_diff)));
					}
					else
					if (dir==0){//corridor on right side
						PossibleFixes.Add(new Vector2(rw_diff,Subs.GetRandom(rh_diff)));
					}
					else
					if (dir==3){//corridor on bottom side
						PossibleFixes.Add(new Vector2(Subs.GetRandom(rw_diff),rh_diff));
					}
					else
					if (dir==1){//corridor on top side
						PossibleFixes.Add(new Vector2(Subs.GetRandom(rw_diff),0));
					}
				}

				//apply a random offset fix if necessary
				if (PossibleFixes.Count>0){
					var r_fix=Subs.GetRandom(PossibleFixes);
					
					room.XOFF=(int)r_fix.x;
					room.YOFF=(int)r_fix.y;
				}
			}
		    
			//add rooms to floor plan
			foreach(var room in Rooms){
				for (int mx = 0; mx < room.RoomXmlData.W; mx++)
				{
					for (int my = 0; my < room.RoomXmlData.H; my++)
					{
						NewFloorMap.map_data[room.X+room.XOFF+mx,room.Y+room.YOFF+my]=room.RoomXmlData.map_data[mx,my];
					}
				}
			}

			//clear room tiles
			for(int x=0;x<w;x++)
			{
				for(int y=0;y<h;y++)
				{
					var index=NewFloorMap.map_data[x,y];
					if (index==MapGenerator.RoomIcon||index==MapGenerator.RoomEndIcon||IsRoomStartIndex(index))
						NewFloorMap.map_data[x,y]=MapGenerator.SpaceIcon;
				}
			}
			
			//create corridor walls
			for(int x=0;x<w;x++)
			{
				for(int y=0;y<h;y++)
				{
					var t_index=NewFloorMap.map_data[x,y].ToLower();
					if (t_index==MapGenerator.CorridorIcon||t_index==MapGenerator.DoorIcon||t_index==MapGenerator.FloorIcon){
						int dir=0,xo=0,yo=0;
						while (dir<4){
							xo=MapGenerator.GetCardinalX(dir);
							yo=MapGenerator.GetCardinalY(dir);
							
							var index=NewFloorMap.GetIndex(x+xo,y+yo);
							
							if (index==MapGenerator.SpaceIcon){
								NewFloorMap.map_data[x+xo,y+yo]=MapGenerator.WallIcon;
							}
							++dir;
						}
					}
				}
			}

		    //add doors
			foreach(var room in Rooms){
	            if (!room.roomStats.randomize_doors) continue;

				List<Vector2> PossibleDoors=new List<Vector2>();
				
				for (var dir=0;dir<4;dir++){
					var door_p=GetRandomDoorPos(room,NewFloorMap,dir);
					if (door_p!=Vector2.zero){
						PossibleDoors.Add(door_p);
					}
				}

				//apply random door(s)
				if (PossibleDoors.Count>0){
					int amount_of_doors=Mathf.Max(1,Subs.GetRandom(PossibleDoors.Count));
					while (amount_of_doors>0){
						--amount_of_doors;
						var r_fix=Subs.GetRandom(PossibleDoors);

						NewFloorMap.map_data[(int)r_fix.x,(int)r_fix.y]=MapGenerator.DoorIcon;
						
						//change non floors next to added door to floors
						for (int s=0;s<4;++s){
							int x=MapGenerator.GetCardinalX(s);
							int y=MapGenerator.GetCardinalY(s);
							var index=NewFloorMap.GetIndex((int)r_fix.x+x,(int)r_fix.y+y);
							if (index=="") continue;
							if (index!=MapGenerator.SpaceIcon
							    &&index!=MapGenerator.FloorIcon
							    &&index!=MapGenerator.WallIcon
							    &&index!=MapGenerator.CorridorIcon)
							{
								Debug.Log(index+" changed!!");
								NewFloorMap.map_data[(int)r_fix.x+x,(int)r_fix.y+y]=MapGenerator.FloorIcon;
							}
						}
					}
				}
			}

			ship_obj_data.Floors.Add(i,NewFloorMap);
			ship_obj_data.FloorRooms.Add(i,Rooms);
		}

		return ship_obj_data;
	}

	/// <summary>
	/// Gets the random door position.
	/// Returns zero if non found.
	/// </summary>
	private Vector2 GetRandomDoorPos(ShipRoomObjData room,MapXmlData map,int dir){
		var temp_door_list=new List<Vector2>();//DEV. unnec list
		
        int x=0,y=0;//,x1=0,y1=0,x2=0,y2=0;
        if (dir==0){
			for (int i=0;i<room.RoomXmlData.H;i++){
				
                x=room.X+room.XOFF+room.RoomXmlData.W-1;
			    y=room.Y+room.YOFF+i;

                if (IsPosGoodForADoor(map,x,y,1,0,-1,0)) temp_door_list.Add(new Vector2(x,y));
			}
		}else
		if (dir==1){
			for (int i=0;i<room.RoomXmlData.W;i++){
                x=room.X+room.XOFF+i;
				y=room.Y+room.YOFF;
				
                if (IsPosGoodForADoor(map,x,y,0,-1,0,1)) temp_door_list.Add(new Vector2(x,y));
			}
		}else
		if (dir==2){
			for (int i=0;i<room.RoomXmlData.H;i++){
                x=room.X+room.XOFF;
				y=room.Y+room.YOFF+i;
				
                if (IsPosGoodForADoor(map,x,y,-1,0,1,0)) temp_door_list.Add(new Vector2(x,y));
			}
		}else 
		if (dir==3){
			for (int i=0;i<room.RoomXmlData.W;i++){
                x=room.X+room.XOFF+i;
				y=room.Y+room.YOFF+room.RoomXmlData.H-1;
				
                if (IsPosGoodForADoor(map,x,y,0,1,0,-1)) temp_door_list.Add(new Vector2(x,y));
			}
		}

		if (temp_door_list.Count>0){
			return Subs.GetRandom(temp_door_list);
		}
		return Vector2.zero;
	}

    private bool IsPosGoodForADoor(MapXmlData map,int x,int y,int x1,int y1,int x2,int y2){
        var oi=map.GetIndex(x+x1,y+y1);
		if (oi==MapGenerator.CorridorIcon){
            oi=map.GetIndex(x,y);
            if (oi==MapGenerator.WallIcon){
                oi=map.GetIndex(x+x2,y+y2);
				if (oi!=MapGenerator.SpaceIcon&&oi!=MapGenerator.WallIcon){
                    return true;
                }
            }
        }
        return false;
    }

	/// <summary>
	/// Checks the legit walls, which can be used to create doors randomly.
	/// DEV. imp a more adv. algorithm
	/// </summary>
	private void CheckLegitWalls(ShipRoomObjData cell,MapXmlData floor){
		cell.corridor_dirs.Clear();
		bool upok=true,downok=true,leftok=true,rightok=true;
		for (int i=1;i<cell.W-1;i++){
			if (upok&&floor.GetIndex(cell.X+i,cell.Y-1)!=MapGenerator.CorridorIcon){
				upok=false;
			}
			if (downok&&floor.GetIndex(cell.X+i,cell.Y+cell.H)!=MapGenerator.CorridorIcon){
				downok=false;
			}
		}
		
		for (int i=0;i<cell.H;i++){
			if (leftok&&floor.GetIndex(cell.X-1,cell.Y+i)!=MapGenerator.CorridorIcon){
				leftok=false;
			}
			if (rightok&&floor.GetIndex(cell.X+cell.W,cell.Y+i)!=MapGenerator.CorridorIcon){
				rightok=false;
			}
		}

		if (upok) 		cell.corridor_dirs.Add(1);
		if (downok) 	cell.corridor_dirs.Add(3);
		if (rightok) 	cell.corridor_dirs.Add(0);
		if (leftok) 	cell.corridor_dirs.Add(2);
	}
}


