using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System.IO;
using System.Xml;

public class MapData{
	public string[,] map_data;

	public MapData(int w,int h){
		map_data=new string[w,h];
	}
}

public class XMLMapLoader : MonoBehaviour{

	public List<MapData> Maps {get;private set;}
	public Dictionary<string,MapData> Ships {get;private set;}
	
	public void Awake()
	{
		Maps=new List<MapData>();
		Ships=new Dictionary<string, MapData>();
		
		XML_sys.OnRead+=read;
	}
	
	void read()
	{
		XML_Loader.checkFolder("Data/Maps");

		var files=Directory.GetFiles("Data/Maps");

		foreach (var f in files){
			var Xdoc=new XmlDocument();
			Xdoc.Load(f);

			//read xml
			var root=Xdoc["Root"];

			foreach (XmlNode node in root){
				if (node.Name=="Room"){
					Maps.Add(readMapData(node));
				}
				if (node.Name=="Ship"){
					Maps.Add(readMapData(node));
				}
			}
		}
	}
	
	MapData readMapData(XmlNode node){
				
		int 
			width=XML_Loader.getAttInt(node,"w"),
			height=XML_Loader.getAttInt(node,"h");
		
		var map=new MapData(width,height);

		var spl=node.InnerText.Replace(" ","").Replace("\r","").Replace("\t","").Split('\n');
		int y=0,x=0;
		foreach (var line in spl){
			if (line=="") continue;
			while (x<map.map_data.GetLength(0)){
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
