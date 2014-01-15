using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour 
{
	public int gridWidth{get;private set;}
	public int gridHeight{get;private set;}
	/*
	private List<TileMain> tiles = new List<TileMain>();
	
	public TileData[,] tiles_map;
	
	
	public Tilemain GetTile(TileData tile)
	{
		return tiles_map[(int)tile.TilePosition.x,(int)tile.TilePosition.y];
	}
	
	public void GenerateGrid(GameDatabase GDB,MapLoader ml)
	{
		MapData md = ml.Maps[0];
			
		gridWidth=md.map_data.GetLength(0);
		gridHeight=md.map_data.GetLength(1);
			
		tiles_map=new TileMain[gridX,gridY];

		for (int i = 0; i < gridX; i++)
		{
			for (int j = 0; j < gridY; j++)
			{	
				var go = Instantiate(tileCrossroad, new Vector3(i*tileCrossroad.size.width, 0, j*tileCrossroad.size.height), Quaternion.identity) as Tile;
				tiles.Add(go);	
				tiles_map[i,j]=go;
				go.SetData(GDB.tiledata_map[i,j]);
			}
		}	
		
		for (int i = 0; i < gridX; i++)
		{
			for (int e = 0; e < gridY; e++)
			{
				var pos=tiles_map[i,e].transform.position;

				switch (md.map_data[i,e])
				{
					case "p":
						tiles_map[i,e].tileObject = (GameObject)Instantiate(policeStationPrefab, pos, policeStationPrefab.transform.rotation);
					break;
					case "a":
						tiles_map[i,e].tileObject = (GameObject)Instantiate(alleyPrefab,pos, alleyPrefab.transform.rotation);
					break;
					case "n":
						tiles_map[i,e].tileObject = (GameObject)Instantiate(newsStationPrefab, pos, newsStationPrefab.transform.rotation);
					break;
					case "u":
						tiles_map[i,e].tileObject = (GameObject)Instantiate(urbanDrugstashPrefab, pos, urbanDrugstashPrefab.transform.rotation);
					break;
					case "c":
						tiles_map[i,e].tileObject = (GameObject)Instantiate(cityHallPrefab, pos, cityHallPrefab.transform.rotation);
					break;
					case "f":
						tiles_map[i,e].tileObject = (GameObject)Instantiate(foxxyPrefab, pos, foxxyPrefab.transform.rotation);
					break;
				}
			}
		}
	}*/

}