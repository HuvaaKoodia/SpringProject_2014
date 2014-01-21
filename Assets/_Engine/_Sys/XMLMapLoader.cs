using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System.IO;
using System.Xml;


//DEV. rename to RoomXMLData and ShipXMLData
public class MapXmlData{
	public string[,] map_data;
	public int W{get{return map_data.GetLength(0);}}
	public int H{get{return map_data.GetLength(1);}}

	public MapXmlData(int w,int h){
		map_data=new string[w,h];
	}

	public string GetIndex(int x,int y){
		if (Subs.insideArea(x,y,0,0,W,H)){
			return map_data[x,y];
		}
		return "";
	}
}

public class ShipMapXmlData{
	public string Name{get;private set;}
	public Dictionary<int,MapXmlData> Floors;
	public int CorridorLengthMin{get;private set;}
	public int CorridorLengthMax{get;private set;}
	public int CorridorWidth{get;private set;}
	
	public ShipMapXmlData(string name){
		Name=name;
		Floors=new Dictionary<int, MapXmlData>();
	}
	
	public void SetCorridor(int lmin,int lmax,int width){
		CorridorLengthMax=lmax;
		CorridorLengthMin=lmin;
		CorridorWidth=width;
	}
}

public class XMLMapLoader : MonoBehaviour{
	
	public Dictionary<string,List<MapXmlData>> Rooms {get;private set;}
	public Dictionary<string,ShipMapXmlData> Ships {get;private set;}
	
	public void Awake()
	{
		Ships=new Dictionary<string, ShipMapXmlData>();
		Rooms=new Dictionary<string, List<MapXmlData>>();
		
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
					string name=node.Attributes["type"].Value;
					
					if (!Rooms.ContainsKey(name))
					{
						Rooms.Add(name,new List<MapXmlData>());
					}

					Rooms[name].Add(readMapData(node));
				}
				if (node.Name=="Ship")
				{
					string name=node.Attributes["type"].Value;
					int floor=XML_Loader.getAttInt(node,"floor");
					int lmin=1,lmax=3,w=2;
					var att=node.Attributes["c_len"];
					if (att!=null){
						var spl=Subs.Split(att.Value,",");
						if (spl.Length>0)
						{
							lmin=lmax=int.Parse(spl[0]);
						}
						if (spl.Length>1)
						{
							lmax=int.Parse(spl[1]);
						}
					}
					
					att=node.Attributes["c_wid"];
					if (att!=null){
						w=int.Parse(att.Value);
					}
					
					ShipMapXmlData ship=null;
					if (Ships.ContainsKey(name))
					{
						Ships.TryGetValue(name,out ship);
						
					}
					else
					{
						ship=new ShipMapXmlData(name);
						Ships.Add(name,ship);
						ship.SetCorridor(lmin,lmax,w);
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
	
	MapXmlData readMapData(XmlNode node){
				
		//int 
		//	width=XML_Loader.getAttInt(node,"w"),
		//	height=XML_Loader.getAttInt(node,"h");

		var lines=Subs.Split(node.InnerText.Replace("\r","").Replace("\t",""),"\n");
		var indexes=Subs.Split(lines[0]," ");

		int width=indexes.Length,height=lines.Length;


		var map=new MapXmlData(width,height);

		lines=Subs.Split(node.InnerText.Replace("\r","").Replace("\t",""),"\n");
		int y=0;
		foreach (var line in lines)
		{
			indexes=Subs.Split(line," ");
			for (int x=0;x<map.W;x++){
				var ss=indexes[x];
				map.map_data[x,y]=ss;
			}
			++y;
		}
		
		return map;
	}
}
