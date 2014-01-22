using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipObjData : ShipMapXmlData {

	public Dictionary<int,List<CellData>> FloorRooms;

	public ShipObjData(string name):base(name){
		FloorRooms=new Dictionary<int, List<CellData>>();
	}
}
