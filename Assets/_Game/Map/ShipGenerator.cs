using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Generates ShipObjectData from ShipMapData.
/// </summary>
public class ShipGenerator : MonoBehaviour 
{
	private Dictionary<string,string> RoomStartIndexes=
	new Dictionary<string,string>{
		{"R","room"},{"A","airlock"}
	};

	bool IsRoomStartIndex(string index){
		foreach(var i in RoomStartIndexes){
			if (i.Key==index)
				return true;
		}
		return false;
	}

	void FitRoom(string room_index,CellData cell){

		string room_name=RoomStartIndexes[room_index];

		int c=0;
		MapXmlData rroom;
		while (true){
			++c;
			if (c>100){
				Debug.LogError("No fitting room in ["+cell.X+", "+cell.Y+"]");
				break;
			}
			
			rroom=Subs.GetRandom(XmlMapRead.Rooms[room_name]);
			
			if (rroom.W>cell.W||rroom.H>cell.H){
				continue;
			}
			
			cell.RoomXmlData=rroom;
			break;
		}
	}

	public XMLMapLoader XmlMapRead;
	public PrefabStore MapPrefabs;

	public ShipObjData GenerateShipObjectData(string TestLoadShipName)
	{
		//randomize ship type
		ShipMapXmlData ship_xml_data=XmlMapRead.Ships[TestLoadShipName];
		//DEV.temp just the first floor
		MapXmlData CurrentFloor=ship_xml_data.Floors[0];

		int w=CurrentFloor.W;
		int h=CurrentFloor.H;
		List<CellData> Rooms=new List<CellData>();
		MapXmlData NewFloorMap=new MapXmlData(w,h);
		
		//Assign map indexes and generate room cells
		for(int x=0;x<w;x++)
		{		
			for(int y=0;y<h;y++)
			{
				var index=CurrentFloor.GetIndex(x,y);
				NewFloorMap.map_data[x,y]=index;

				if (IsRoomStartIndex(index)){
					//random room
					var cell=new CellData("R");
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
						if (nindex=="T"){
							cell.W+=1;
							break;
						}
						if (nindex=="r"){
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
						if (nindex=="T"){
							cell.H+=1;
							break;
						}
						if (nindex=="r"){
							cell.H+=1;
							++ww;
						}
						else break;
					}
					CheckLegitWalls(cell,CurrentFloor);

					if (index=="R"){
					//randomize a room of right size
						int c=0;
						MapXmlData rroom;
						while (true){
							++c;
							if (c>100){
								Debug.LogError("No fitting room in ["+cell.X+", "+cell.Y+"]");
								break;
							}

							rroom=Subs.GetRandom(XmlMapRead.Rooms["room"]);

							if (rroom.W>cell.W||rroom.H>cell.H){
								continue;
							}

							cell.RoomXmlData=rroom;
							break;
						}
					}
					else if (index=="A"){
						int c=0;
						MapXmlData rroom;
						while (true){
							++c;
							if (c>100){
								Debug.LogError("No fitting room in ["+cell.X+", "+cell.Y+"]");
								break;
							}
							
							rroom=Subs.GetRandom(XmlMapRead.Rooms["airlock"]);
							
							if (rroom.W>cell.W||rroom.H>cell.H){
								continue;
							}
							
							cell.RoomXmlData=rroom;
							break;
						}
					}
				}
			}
		}

	//Rooms

		foreach(var room in Rooms){
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

		//clear room tiles r->.
		for(int x=0;x<w;x++)
		{
			for(int y=0;y<h;y++)
			{
				var index=NewFloorMap.map_data[x,y].ToLower();
				if (index=="r"||index=="t")
					NewFloorMap.map_data[x,y]=",";
			}
		}
		
		//create corridor walls
		for(int x=0;x<w;x++)
		{
			for(int y=0;y<h;y++)
			{
				var t_index=NewFloorMap.map_data[x,y].ToLower();
				if (t_index=="c"||t_index==MapGenerator.DoorIcon){
					int dir=0,xo=0,yo=0;
					while (dir<4){
						if (dir==0){ xo=1;yo=0; }
						if (dir==1){ xo=-1;yo=0;}
						if (dir==2){ xo=0;yo=1; }
						if (dir==3){ xo=0;yo=-1;}
						
						var index=NewFloorMap.GetIndex(x+xo,y+yo);
						
						if (index==","){
							NewFloorMap.map_data[x+xo,y+yo]=MapGenerator.WallIcon;
						}
						
						++dir;
					}
				}
			}
		}


	//add doors

		foreach(var room in Rooms){
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
				}
			}
		}

		ShipObjData ship_obj_data=new ShipObjData(ship_xml_data);
		ship_obj_data.Floors.Add(0,NewFloorMap);
		ship_obj_data.FloorRooms.Add(0,Rooms);

		return ship_obj_data;
	}

	/// <summary>
	/// Gets the random door position.
	/// Returns zero if non found.
	/// </summary>
	private Vector2 GetRandomDoorPos(CellData room,MapXmlData map,int dir){
		var temp_door_list=new List<Vector2>();//DEV. unnec list
		if (dir==0){
			for (int i=0;i<room.RoomXmlData.H;i++){
				int 
					x=room.X+room.XOFF+room.RoomXmlData.W-1,
				y=room.Y+room.YOFF+i;

				var oi=map.GetIndex(x+1,y);
				if (oi=="c"){
					oi=map.GetIndex(x,y);
					if (oi==MapGenerator.WallIcon){
						oi=map.GetIndex(x-1,y);
						if (oi=="."){
							temp_door_list.Add(new Vector2(x,y));
						}
					}
				}
			}
		}else
		if (dir==1){
			for (int i=0;i<room.RoomXmlData.W;i++){
				int 
					x=room.X+room.XOFF+i,
					y=room.Y+room.YOFF;
				
				var oi=map.GetIndex(x,y-1);
				if (oi=="c"){
					oi=map.GetIndex(x,y);
					if (oi==MapGenerator.WallIcon){
						oi=map.GetIndex(x,y+1);
						if (oi=="."){
							temp_door_list.Add(new Vector2(x,y));
						}
					}
				}
			}
		}else
		if (dir==2){
			for (int i=0;i<room.RoomXmlData.H;i++){
				int 
					x=room.X+room.XOFF,
					y=room.Y+room.YOFF+i;
				
				var oi=map.GetIndex(x-1,y);
				if (oi=="c"){
					oi=map.GetIndex(x,y);
					if (oi==MapGenerator.WallIcon){
						oi=map.GetIndex(x+1,y);
						if (oi=="."){
							temp_door_list.Add(new Vector2(x,y));
						}
					}
				}
			}
		}else 
		if (dir==3){
			for (int i=0;i<room.RoomXmlData.W;i++){
				int 
					x=room.X+room.XOFF+i,
					y=room.Y+room.YOFF+room.RoomXmlData.H-1;
				
				var oi=map.GetIndex(x,y+1);
				if (oi=="c"){
					oi=map.GetIndex(x,y);
					if (oi==MapGenerator.WallIcon){
						oi=map.GetIndex(x,y-1);
						if (oi=="."){
							temp_door_list.Add(new Vector2(x,y));
						}
					}
				}
			}
		}

		if (temp_door_list.Count>0){
			return Subs.GetRandom(temp_door_list);
		}
		return Vector2.zero;
	}
	/// <summary>
	/// Checks the legit walls, which can be used to create doors, for random buildings.
	/// DEV. imp a more adv algorithm
	/// </summary>
	private void CheckLegitWalls(CellData cell,MapXmlData floor){
		cell.corridor_dirs.Clear();
		bool upok=true,downok=true,leftok=true,rightok=true;
		for (int i=0;i<cell.W;i++){
			if (upok&&floor.GetIndex(cell.X+i,cell.Y-1)!="c"){
				upok=false;
			}
			if (downok&&floor.GetIndex(cell.X+i,cell.Y+cell.H)!="c"){
				downok=false;
			}
		}
		
		for (int i=0;i<cell.H;i++){
			if (leftok&&floor.GetIndex(cell.X-1,cell.Y+i)!="c"){
				leftok=false;
			}
			if (rightok&&floor.GetIndex(cell.X+cell.W,cell.Y+i)!="c"){
				rightok=false;
			}
		}

		if (upok) 		cell.corridor_dirs.Add(1);
		if (downok) 	cell.corridor_dirs.Add(3);
		if (rightok) 	cell.corridor_dirs.Add(0);
		if (leftok) 	cell.corridor_dirs.Add(2);
	}

}


public class CellData{
	//public TileType Type;
	public string TileIndex;
	public bool IsEmpty=true;
	public int X,Y,W=0,H=0,LOCK_W=0,LOCK_H=0;
	MapXmlData room;
	public List<int> corridor_dirs=new List<int>();

	int xoff,yoff;
	
	public bool XOFFchanged{get;private set;}
	public bool YOFFchanged{get;private set;}
	
	public int XOFF{
		get{return xoff;}
		set{
			xoff=value;
			XOFFchanged=true;
		}
	}
	public int YOFF{
		get{return yoff;}
		set{
			yoff=value;
			YOFFchanged=true;
		}
	}
	
	public MapXmlData RoomXmlData{
		get 
		{
			return room;	
		}
		set
		{
			IsEmpty=false;
			room=value;
			//SetSize(room.W,room.H);
		}
	}
	
	public void SetSize(int w,int h){
		IsEmpty=false;	
		W=w;
		H=h;
	}
	
	public CellData(string index){
		SetSize(1,1);
		TileIndex=index;
	}
}
