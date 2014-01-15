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

public class MapLoader : MonoBehaviour{

	public List<MapData> Maps=new List<MapData>();
	
	public void Awake(){
		XML_sys.OnRead+=read;
	}
	
	void read()
	{
		XML_Loader.checkFolder("Data/Maps");

		var files=Directory.GetFiles("Data/Maps");

		foreach (var f in files){
			var Xdoc=new XmlDocument();
			//Debug.Log("f "+f);
			Xdoc.Load(f);

			//read xml
			var root=Xdoc["Root"];

			foreach (XmlNode node in root){
				if (node.Name=="Map"){
					int height=5;
					var map=new MapData(7,height);

					var spl=node.InnerText.Replace(" ","").Replace("\r","").Split('\n');
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
					
					Maps.Add(map);
				}
			}
		}
	}
}
