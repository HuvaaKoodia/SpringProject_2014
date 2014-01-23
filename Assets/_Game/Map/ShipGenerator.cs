using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Generates ShipObjectData from ShipMapData.
/// </summary>
public class ShipGenerator : MonoBehaviour 
{
	public XMLMapLoader XmlMapRead;
	public PrefabStore MapPrefabs;

	public ShipObjData GenerateShipObjectData()
	{
		//randomize ship type
		ShipMapXmlData ship_xml_data=Subs.GetRandom(XmlMapRead.Ships.Values);
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

				if (index=="R"){
					//random room
					var cell=new CellData("R");
					cell.X=x;cell.Y=y;
					cell.W=0;cell.H=0;

					Rooms.Add(cell);

					//calculate max room size
					int ww=0;
					while (true){
						if (CurrentFloor.GetIndex(x+ww,y)=="T"){
							cell.W+=1;
							break;
						}
						if (CurrentFloor.GetIndex(x+ww,y).ToLower()=="r"){
							cell.W+=1;
							++ww;
						}
						else break;
					}
					ww=0;
					while (true){
						if (CurrentFloor.GetIndex(x,y+ww)=="T"){
							cell.H+=1;
							break;
						}
						if (CurrentFloor.GetIndex(x,y+ww).ToLower()=="r"){
							cell.H+=1;
							++ww;
						}
						else break;
					}
					CheckLegitWalls(cell,CurrentFloor);

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
				if (NewFloorMap.map_data[x,y].ToLower()=="r")
					NewFloorMap.map_data[x,y]=",";
			}
		}
		
		//create corridor walls
		for(int x=0;x<w;x++)
		{
			for(int y=0;y<h;y++)
			{
				if (NewFloorMap.map_data[x,y].ToLower()=="c"){
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
					
					NewFloorMap.map_data[(int)r_fix.x,(int)r_fix.y]="d";
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

	//OLD
	/*
	
	public MapXmlData GenerateShipObjectData()
	{
		//randomize ship type
		ShipMapXmlData ship_data=Subs.GetRandom(XmlMapRead.Ships.Values);
		//DEV.temp just the first floor
		MapXmlData CurrentFloor=ship_data.Floors[0];
		
		int w=CurrentFloor.W;
		int h=CurrentFloor.H;
		CellData[,] Cellmap=new CellData[w,h];
		
		//generate initial cells
		for(int x=0;x<w;x++)
		{		
			for(int y=0;y<h;y++)
			{
				var cell=Cellmap[x,y]=new CellData(CurrentFloor.map_data[x,y]);
				
				//corridors hard wired down the line
				//				if (cell.TileIndex=="c"){
				//					a specified corridor size: 1 2 3 ?
				//					cell.RoomXmlData=Subs.GetRandom(XmlMapRead.Rooms["corridor"]);
				//				}
				
				if (cell.TileIndex=="r"){
					//random room
					cell.RoomXmlData=Subs.GetRandom(XmlMapRead.Rooms["room"]);
				}
				
				if (cell.TileIndex=="e"){
					//random elevator room
					cell.RoomXmlData=Subs.GetRandom(XmlMapRead.Rooms["elevator"]);
					
				}
			}
		}
		//recalculate corridor sizes to match ship floor parameters
		for (int x = 0; x < w; x++)
		{
			for (int y = 0; y < h; y++)
			{
				var cell=Cellmap[x,y];
				if (cell.TileIndex=="c"){
					//find direction
					bool is_hor=false,is_ver=false;
					CellData 
						data=GetCell(x+1,y,Cellmap);
					if (data!=null&&data.TileIndex=="c")
						is_hor=true;
					data=GetCell(x-1,y,Cellmap);
					if (data!=null&&data.TileIndex=="c")
						is_hor=true;
					data=GetCell(x,y+1,Cellmap);
					if (data!=null&&data.TileIndex=="c")
						is_ver=true;
					data=GetCell(x,y-1,Cellmap);
					if (data!=null&&data.TileIndex=="c")
						is_ver=true;
					
					if (is_hor&&is_ver){
						//crossroad use corridor width for both
						cell.W=cell.H=ship_data.CorridorWidth;
					}
					else if (is_hor){
						cell.isHor=true;
						cell.LOCK_H=cell.H=ship_data.CorridorWidth;
						cell.W=Random.Range(ship_data.CorridorLengthMin,ship_data.CorridorLengthMax+1);
					}
					else if (is_ver){
						cell.isVer=true;
						cell.LOCK_W=cell.W=ship_data.CorridorWidth;
						cell.H=Random.Range(ship_data.CorridorLengthMin,ship_data.CorridorLengthMax+1);
					}
				}
			}
		}
		
		//resize cell sizes to the biggest cell in the row/column
		int temp_size=0;
		//columns
		for (int x = 0; x < w; x++)
		{
			temp_size=0;
			//find max width in column
			for (int y = 0; y < h; y++)
			{
				var cell=Cellmap[x,y];
				if (temp_size<cell.W)
					temp_size=cell.W;	
			}
			
			//set the width of all cells in the column to max
			for (int y = 0; y < h; y++)
			{
				Cellmap[x,y].W=temp_size;
			}
		}
		//rows
		for (int y = 0; y < h; y++)
		{
			temp_size=0;
			//find max height in row
			for (int x = 0; x < w; x++)
			{
				var cell=Cellmap[x,y];
				if (temp_size<cell.H)
					temp_size=cell.H;	
			}
			
			//set the height of all cells in the row to max
			for (int x = 0; x < w; x++)
			{
				Cellmap[x,y].H=temp_size;
			}
		}
		
		//generating mapData object from cellmap and ship blueprint
		
		//calculate maximal size for map. Also set cell x&y positions
		int ship_w=0,ship_h=0;
		temp_size=0;
		for (int x = 0; x < w; x++)
		{
			temp_size=0;
			for (int y = 0; y < h; y++)
			{
				var cell=Cellmap[x,y];
				cell.Y=temp_size;
				temp_size+=cell.H;
			}
			if (temp_size>ship_h)
				ship_h=temp_size;
		}
		
		for (int y = 0; y < h; y++)
		{
			temp_size=0;
			for (int x = 0; x < w; x++)
			{
				var cell=Cellmap[x,y];
				cell.X=temp_size;
				temp_size+=cell.W;
			}
			if (temp_size>ship_w)
				ship_w=temp_size;
		}
		
		var floor_plan=new MapXmlData(ship_w,ship_h);
		//fill with empty
		for (int x = 0; x < floor_plan.W; x++)
		{
			for (int y = 0; y < floor_plan.H; y++)
			{
				floor_plan.map_data[x,y]=",";
			}
		}
		
		//add corridors
		for (int x = 0; x < w; x++)
		{
			for (int y = 0; y < h; y++)
			{
				var cell=Cellmap[x,y]; 
				
				if (cell.TileIndex=="c"){
					//construct corridors (hard wired)
					int cw=cell.W,ch=cell.H;
					
					if (cell.LOCK_H!=0){
						ch=cell.LOCK_H;
						int h_diff=cell.H-cell.LOCK_H;
						
						//check if adjecent cells already have an offset
						if (h_diff>0&&cell.isHor){
							CellData cc;
							cc=GetCell(x-1,y,Cellmap);
							if (cc!=null&&cc.TileIndex=="c"&&cc.isHor&&cc.YOFFchanged){
								cell.YOFF=cc.YOFF;
							}
							else{
								cc=GetCell(x+1,y,Cellmap);
								if (cc!=null&&cc.TileIndex=="c"&&cc.isHor&&cc.YOFFchanged){
									cell.YOFF=cc.YOFF;
								}
								else{
									//randomize y offset
									cell.YOFF=Subs.GetRandom(h_diff+1);
								}
							}
						}
					}else
					if (cell.LOCK_W!=0){
						cw=cell.LOCK_W;
						int w_diff=cell.W-cell.LOCK_W;
						
						//check if adjecent cells already have an offset
						if (w_diff>0&&cell.isVer){
							CellData cc; 
							cc=GetCell(x,y-1,Cellmap);
							if (cc!=null&&cc.TileIndex=="c"&&cc.isVer&&cc.XOFFchanged){
								cell.XOFF=cc.XOFF;
							}
							else{
								cc=GetCell(x,y+1,Cellmap);
								if (cc!=null&&cc.TileIndex=="c"&&cc.isVer&&cc.XOFFchanged){
									cell.XOFF=cc.XOFF;
								}
								else{
									//randomize x offset
									cell.XOFF=Subs.GetRandom(w_diff+1);
								}
							}
							
						}
					}
					
					for (int cx = 0; cx < cw; cx++)
					{
						for (int cy = 0; cy < ch; cy++)
						{
							floor_plan.map_data[cell.X+cell.XOFF+cx,cell.Y+cell.YOFF+cy]=".";
						}
					}
				}
			}
		}
		
		//add rooms
		for (int x = 0; x < w; x++)
		{
			for (int y = 0; y < h; y++)
			{
				var cell=Cellmap[x,y]; 
				
				if (cell.RoomXmlData!=null)
				{
					//check if offset needs to be tampered with
					
					//gather all adjacent corridors
					List<CellData> corridors_dirs=new List<CellData>();
					int xo=0,yo=0;
					CellData other;
					int dir=0;
					
					while (dir<4){
						if (dir==0){ xo=1;yo=0; }
						if (dir==1){ xo=-1;yo=0;}
						if (dir==2){ xo=0;yo=1; }
						if (dir==3){ xo=0;yo=-1;}
						
						other=GetCell(x+xo,y+yo,Cellmap);
						
						if (other!=null&&other.TileIndex=="c"){
							corridors_dirs.Add(other);
						}
						++dir;
					}
					
					//check if there is a need to change offset
					int rw_diff=cell.W-cell.RoomXmlData.W,rh_diff=cell.H-cell.RoomXmlData.H;
					
					List<Vector2> PossibleFixes=new List<Vector2>();
					
					foreach(var ac in corridors_dirs){
						int diff=0;
						if (ac.isVer){
							diff=ac.W-ac.XOFF-ac.LOCK_W;
							if (ac.X<cell.X){//corridor on left side
								if (diff>0){
									PossibleFixes.Add(new Vector2(-diff,0));
								}
							}
							else{//corridor on right side
								if (rw_diff>0||ac.XOFF>0){
									PossibleFixes.Add(new Vector2(ac.XOFF+rw_diff,0));
								}
							}
						}
						else if (ac.isHor){
							diff=ac.H-ac.YOFF-ac.LOCK_H;
							if (ac.Y<cell.Y){//corridor on bottom side
								if (diff>0){
									PossibleFixes.Add(new Vector2(0,-diff));
								}
							}
							else{//corridor on top side
								if (rh_diff>0||ac.YOFF>0){
									PossibleFixes.Add(new Vector2(0,ac.YOFF+rh_diff));
								}
							}
						}
					}
					
					//apply a random fix if necessary
					
					if (PossibleFixes.Count>0){
						var r_fix=Subs.GetRandom(PossibleFixes);
						
						cell.XOFF=(int)r_fix.x;
						cell.YOFF=(int)r_fix.y;
					}
					
					//create room
					for (int mx = 0; mx < cell.RoomXmlData.W; mx++)
					{
						for (int my = 0; my < cell.RoomXmlData.H; my++)
						{
							floor_plan.map_data[cell.X+cell.XOFF+mx,cell.Y+cell.YOFF+my]=cell.RoomXmlData.map_data[mx,my];
						}
					}
				}
			}
		}
		
		Debug.Log("SHIP GENERATOR DEBUG: ");
		
		for (int y = 0; y < h; y++)
		{
			string line="";
			for (int x = 0; x < w; x++)
			{
				var cell=Cellmap[x,y];
				line+=cell.TileIndex+"-["+cell.X+","+cell.Y+"]("+cell.W+","+cell.H+") ";
			}
			Debug.Log(line+"\n");
		}
		
		return floor_plan;
	}
	//enum TileType{None,Corridor}
	
	CellData GetCell(int x,int y,CellData[,] map){
		if (Subs.insideArea(x,y,0,0,map.GetLength(0),map.GetLength(1))){
			return map[x,y];
		}
		return null;
	}
	
	private class CellData{
		
		//public TileType Type;
		public string TileIndex;
		public bool IsEmpty=false,isHor,isVer;
		public int X,Y,W,H,LOCK_W=0,LOCK_H=0;
		MapXmlData room;

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
				SetSize(room.W,room.H);
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
}






/* EVEN OLDER and dysfunctional
		//resize cell sizes to fit adjacent rooms
		
		for(int x=0;x<w;x++)
		{		
			for(int y=0;y<h;y++)
			{
				var cell=Cellmap[x,y];
				
				if (cell.TileIndex=="c"){
					//has to be resized if next to any non empty cell of bigger size
					int xo=0,yo=0;
					CellData other;
					int dir=0;
					
					while (dir<4){
						if (dir==0){ xo=1;yo=0; }
						if (dir==1){ xo=-1;yo=0;}
						if (dir==2){ xo=0;yo=1; }
						if (dir==3){ xo=0;yo=-1;}
						
						dir++;
						
						other=GetCell(x+xo,y+yo,Cellmap);
						
						if (other!=null&&!other.IsEmpty&&other.TileIndex!="c"){
							if (xo!=0){
								//horizontally aligned, copy height
								if (cell.H<other.H)
									cell.H=other.H;
							}
							else{
								//vertically aligned, copy width
								if (cell.W<other.W)
									cell.W=other.W;
							}
						}
					}
				}
			}
			
		}
		*/	

