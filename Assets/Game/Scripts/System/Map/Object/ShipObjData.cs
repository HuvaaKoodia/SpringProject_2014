using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipObjData : ShipMapXmlData {
	
	public ShipMapXmlData XmlData{get;private set;}
	public Dictionary<int,List<ShipRoomObjData>> FloorRooms;

	public ShipObjData(ShipMapXmlData ship):base(ship.Name){
		XmlData=ship;
		FloorRooms=new Dictionary<int, List<ShipRoomObjData>>();
	}
}

public class ShipRoomObjData{
	public int X,Y,W=1,H=1,XOFF,YOFF;
	public List<int> corridor_dirs=new List<int>();
	public List<TileObjData> LootCrateTiles=new List<TileObjData>();

	public RoomXmlIndex roomStats {get;set;}
	public MapXmlData RoomXmlData {get;set;}
}
