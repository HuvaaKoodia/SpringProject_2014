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
	
	public void GenerateShipObjectData(ShipMapData ship_data)
	{
		MapData CurrentFloor=ship_data.Floors[0];
		int w=CurrentFloor.map_data.GetLength(0);
		int h=CurrentFloor.map_data.GetLength(1);
		CellData[,] Cellmap=new CellData[w,h];
		
		for(int x=0;x<w;x++)
		{		
			for(int y=0;y<h;y++)
			{
				var cell=Cellmap[x,y]=new CellData();
				var tile=CurrentFloor.map_data[x,y];
				if (tile=="x"){
					cell.w=1;
					cell.h=1;
				}
				if (tile=="c"){
					cell.w=1;
					cell.h=1;
				}
				if (tile=="r"){
					//random room
					cell.RoomData=Subs.GetRandom(XmlMapRead.Maps);
				}
				
				if (tile=="e"){
					//random elevator room
					cell.w=3;
					cell.h=3;
				}
			}
		}
	}

	private class CellData{
		public int w,h;
		MapData room;
		public MapData RoomData{
			get 
			{
				return room;	
			}
			set
			{
				room=value;
				w=room.map_data.GetLength(0);
				h=room.map_data.GetLength(1);
			}
		}
		
		
	}
}

