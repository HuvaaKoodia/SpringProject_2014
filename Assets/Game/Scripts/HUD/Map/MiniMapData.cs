using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class MiniMapData {

	public GameController GC;

	public List<MiniMapTileData[,]> mapFloors {get; private set;}

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

	public void CreateMapSprites(GameObject spriteParent, List<UISprite[,]> spriteList, float zoom, int spriteWidth)
	{
		for (int i = 0; i < GC.Floors.Count; i++)
		{
			MiniMapTileData[,] mapTiles = GC.MiniMapData.GetFloor(i);
			int mapWidth = mapTiles.GetLength(0);
			int mapHeight = mapTiles.GetLength(1);
			
			spriteList.Add(new UISprite[mapWidth,mapHeight]);
			
			for (int x = 0 ; x < mapWidth; x++)
			{
				for (int y = 0; y < mapHeight; y++)
				{
					if (mapTiles[x,y] != null)
						spriteList[i][x,y] = CreateTileSprite(x, y, mapTiles[x,y], spriteParent, zoom, spriteWidth);
				}
			}
		}
	}

	public List<UISprite[,]> CreateMapSprites(GameObject spriteParent, float zoom, int spriteWidth)
	{
		List<UISprite[,]> spriteList = new List<UISprite[,]>();

		for (int i = 0; i < GC.Floors.Count; i++)
		{
			MiniMapTileData[,] mapTiles = GC.MiniMapData.GetFloor(i);
			int mapWidth = mapTiles.GetLength(0);
			int mapHeight = mapTiles.GetLength(1);
			
			spriteList.Add(new UISprite[mapWidth,mapHeight]);
			
			for (int x = 0 ; x < mapWidth; x++)
			{
				for (int y = 0; y < mapHeight; y++)
				{
					if (mapTiles[x,y] != null)
						spriteList[i][x,y] = CreateTileSprite(x, y, mapTiles[x,y], spriteParent, zoom, spriteWidth);
				}
			}
		}

		return spriteList;
	}
	
	UISprite CreateTileSprite(int x, int y, MiniMapTileData tile, GameObject spriteParent, float zoom, int spriteWidth)
	{
		string graphicsName = tile.TileType;
		Debug.Log(graphicsName);
		var go=GameObject.Instantiate(GC.SS.PS.GetMapSpritePrefab(graphicsName)) as GameObject;
		UISprite sprite = go.GetComponent<UISprite>();

		sprite.name = "mapSprite";
		
		sprite.transform.parent = spriteParent.transform;
		sprite.transform.position = spriteParent.transform.position;
		sprite.transform.localRotation = spriteParent.transform.localRotation;
		
		sprite.transform.localScale = Vector3.one * zoom;
		
		//Vector3 playerPosition = GC.Player.transform.position;
		float posX = x; //- (playerPosition.x / MapGenerator.TileSize.x);
		float posY = y; //- (playerPosition.z / MapGenerator.TileSize.z);
		
		sprite.transform.localPosition = new Vector3(posX * ((spriteWidth) * zoom), posY * ((spriteWidth) * zoom), 0);
		
		float graphicsRotation = tile.Rotation;
		graphicsRotation = Mathf.Abs(360 - graphicsRotation);
		Quaternion rot = Quaternion.Euler(0.0f, 0.0f, graphicsRotation);
		sprite.transform.localRotation = rot;
		
		sprite.enabled = false;
		
		return sprite;
	}
}
