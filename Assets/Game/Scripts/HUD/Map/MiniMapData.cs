using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class MiniMapData {

	public GameController GC;

	List<MiniMapTileData[,]> mapFloors;

	public MiniMapTileData[,] GetFloor(int index)
	{
		return mapFloors[index];
	}

	// Use this for initialization
	public MiniMapData()
	{
		mapFloors = new List<MiniMapTileData[,]>();
	}
	
	// Update is called once per frame
	public void Init(GameController GC) {
		this.GC = GC;
		CreateMap();
	}

	void CreateMap()
	{
		for (int i = 0; i < GC.Floors.Count; i++)
		{
			TileMain[,] mapTiles = GC.GetFloor(i).TileMainMap;
			int mapWidth = mapTiles.GetLength(0);
			int mapHeight = mapTiles.GetLength(1);
		
			MiniMapTileData[,] currentFloor = new MiniMapTileData[mapWidth,mapHeight];
			
			for (int x = 0 ; x < mapWidth; x++)
			{
				for (int y = 0; y < mapHeight; y++)
				{
					AddTile(mapTiles[x,y], currentFloor, x, y);
				}
			}

			mapFloors.Add(currentFloor);
		}

		int z = 0;
		z++;
	}
	
	void AddTile(TileMain tile, MiniMapTileData[,] mapTileArr, int x, int y)
	{
		//var str=tile.Data.TileType.ToString();
		if (tile.Data.TileType == TileObjData.Type.Empty
		    || tile.Data.TileType == TileObjData.Type.Wall
		    || tile.TileGraphics == null)
			return;

		string graphicsName = tile.TileGraphics.name;
		
		if (graphicsName.EndsWith("(Clone)"))
		{
			graphicsName = graphicsName.Remove(graphicsName.Length-7);
		}

		mapTileArr[x,y] = new MiniMapTileData(graphicsName, (int)tile.TileGraphics.transform.localRotation.eulerAngles.y);
	}
}
