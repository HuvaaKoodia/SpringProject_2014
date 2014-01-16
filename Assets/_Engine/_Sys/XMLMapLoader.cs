using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System.IO;
using System.Xml;


//DEV. rename to RoomXMLData and ShipXMLData
public class MapData{
	public string[,] map_data;

	public MapData(int w,int h){
		map_data=new string[w,h];
	}
}

public class ShipMapData{
	public string Name;
	public Dictionary<int,MapData> Floors;
	
	public ShipMapData(string name){
		Name=name;
		Floors=new Dictionary<int, MapData>();
	}
}

public class XMLMapLoader : MonoBehaviour{

	public List<MapData> Maps {get;private set;}
	public Dictionary<string,ShipMapData> Ships {get;private set;}
	
	public void Awake()
	{
		Maps=new List<MapData>();
		Ships=new Dictionary<string, ShipMapData>();
		
		XML_sys.OnRead+=read;
	}
	
	void read()
	{
		XML_Loader.checkFolder("Data/Maps");
		var files=Directory.GetFiles("Data/Maps");

		foreach (var f in files)
		{
			var Xdoc=new XmlDocument();
			Xdoc.Load(f);

			var root=Xdoc["Root"];

			foreach (XmlNode node in root){
				if (node.Name=="Room")
				{
					Maps.Add(readMapData(node));
				}
				if (node.Name=="Ship")
				{
					string name=node.Attributes["name"].Value;
					int floor=XML_Loader.getAttInt(node,"floor");
					
					ShipMapData ship=null;
					if (Ships.ContainsKey(name))
					{
						Ships.TryGetValue(name,out ship);
					}
					else
					{
						ship=new ShipMapData(name);
						Ships.Add(name,ship);	
					}
					ship.Floors.Add(floor,readMapData(node));
				}
			}
		}
		/*DEBUG
		Debug.Log("Ships: ");
		foreach(var ship in Ships){
			Debug.Log("Name "+ship.Key+" floors: "+ship.Value.Floors.Count);
			
			foreach(var floor in ship.Value.Floors){
				int w=floor.Value.map_data.GetLength(0);
				int h=floor.Value.map_data.GetLength(1);
				
				for (int y = 0; y < h; y++)
					{
					string line="";
					for (int x = 0; x < w; x++)
					{
						line+=floor.Value.map_data[x,y]+" ";	
					}
					Debug.Log(line+"\n");
				}
			}
		}
		*/
	}
	
	MapData readMapData(XmlNode node){
				
		int 
			width=XML_Loader.getAttInt(node,"w"),
			height=XML_Loader.getAttInt(node,"h");
		
		var map=new MapData(width,height);

		var spl=node.InnerText.Replace(" ","").Replace("\r","").Replace("\t","").Split('\n');
		int y=0,x=0;
		foreach (var line in spl)
		{
			if (line=="") continue;
			while (x<map.map_data.GetLength(0))
			{
				var ss=line.Substring(x,1).ToLower();
				map.map_data[x,height-1-y]=ss;
				x++;
			}
			y++;
			x=0;
		}
		
		return map;
	}
}
