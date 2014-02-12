using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipObjData : ShipMapXmlData {
	
	public ShipMapXmlData XmlData{get;private set;}
	public Dictionary<int,List<CellData>> FloorRooms;

	public ShipObjData(ShipMapXmlData ship):base(ship.Name){
		XmlData=ship;
		FloorRooms=new Dictionary<int, List<CellData>>();
	}
}
