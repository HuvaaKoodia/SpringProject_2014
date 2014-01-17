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

				if (cell.TileIndex=="c"){
					//a specified corridor size: 1 2 3 ?
					cell.RoomData=Subs.GetRandom(XmlMapRead.Rooms["corridor"]);
				}
				if (cell.TileIndex=="r"){
					//random room
					cell.RoomData=Subs.GetRandom(XmlMapRead.Rooms["room"]);
				}
				
				if (cell.TileIndex=="e"){
					//random elevator room
					cell.RoomData=Subs.GetRandom(XmlMapRead.Rooms["elevator"]);
					
				}
			}
		}
		//recalculate corridor sizes to ship floor presets
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
						cell.H=ship_data.CorridorWidth;
						cell.W=Random.Range(ship_data.CorridorLengthMin,ship_data.CorridorLengthMax+1);
					}
					else if (is_ver){
						cell.W=ship_data.CorridorWidth;
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
		
		/* OLD dysfunctional
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
		//generating mapData object from cellmap and ship blueprint
		
		
		
		
		int ship_w=0,ship_h=0;
		//calculate maximal size for map. Also set cell x&y positions
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
		
		//add rooms
		for (int x = 0; x < w; x++)
			{
			for (int y = 0; y < h; y++)
			{
				var cell=Cellmap[x,y]; 
				
				if (cell.TileIndex=="c"){
					//construct corridor (hard wired)
					bool horizontal=true;
					
					for (int cx = 0; cx < cell.W; cx++)
					{
						for (int cy = 0; cy < cell.H; cy++)
						{
							floor_plan.map_data[cell.X+cx,cell.Y+cy]=".";
						}
					}
				}
				else
				if (cell.RoomData!=null){
					for (int mx = 0; mx < cell.RoomData.W; mx++)
						{
						for (int my = 0; my < cell.RoomData.H; my++)
						{
							floor_plan.map_data[cell.X+mx,cell.Y+my]=cell.RoomData.map_data[mx,my];
							
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
		public bool IsEmpty=false;
		public int X,Y,W,H;
		MapXmlData room;
		public MapXmlData RoomData{
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

