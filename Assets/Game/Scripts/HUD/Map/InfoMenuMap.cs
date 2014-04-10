using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class InfoMenuMap : MonoBehaviour {

	GameController GC;
	PlayerMain player;
	
	public GameObject spriteParent;
	
	private List<UISprite[,]> mapSprites;
	private int currentFloor = 0;
	
	private int spriteWidth;

	public UISprite playerIndicator;
	
	public UIPanel spritePanel;
	
	bool _disabled=false;

	// Use this for initialization
	void Awake()
	{ 
		mapSprites = new List<UISprite[,]>();
	}
	
	public void Init(GameController gc)
	{
		GC=gc;
		player = GC.Player;
		
		spriteWidth = GC.SS.PS.FloorSprite.width;

		mapSprites = new List<UISprite[,]>();
		
		GC.MiniMapData.CreateMapSprites(spriteParent, mapSprites, 6, spriteWidth);
		
		ChangeFloor(GC.CurrentFloorIndex);


		float halfWidth = (mapSprites[0].GetLength(0)-1)* ((float)spriteWidth * 3.0f);
		float halfHeight = (mapSprites[0].GetLength(1)-1)* ((float)spriteWidth * 3.0f);

		for (int i = 0; i < mapSprites.Count; i++)
		{
			for (int x = 0; x < mapSprites[i].GetLength(0); x++)
			{
				for (int y = 0; y < mapSprites[i].GetLength(1); y++)
				{
					if (mapSprites[i][x,y] != null)
						mapSprites[i][x,y].transform.localPosition -= new Vector3(halfWidth, halfHeight);
				}
			}
		}


		float playerRot = player.transform.rotation.eulerAngles.y;
		Quaternion rot = Quaternion.Euler(0.0f, 0.0f, playerRot);
		
		//playerIndicator.transform.localRotation =  Quaternion.Euler(0, 0, 360 - playerRot);
	}
	
	// Update is called once per frame
	void Update()
	{
	}

	public void ChangeFloor(int floorIndex)
	{
		currentFloor = floorIndex;
		
		for (int i = 0; i < mapSprites.Count; i++)
		{
			MiniMapTileData[,] mapTiles = GC.MiniMapData.GetFloor(i);
			int mapWidth = mapTiles.GetLength(0);
			int mapHeight = mapTiles.GetLength(1);
			
			bool enable = (floorIndex == i);
			
			for (int x = 0 ; x < mapWidth; x++)
			{
				for (int y = 0; y < mapHeight; y++)
				{
					if (mapSprites[i][x,y] != null)
						mapSprites[i][x,y].enabled = enable;
				}
			}
		}
	}

	public void SetDisabled(bool disabled){
		_disabled=disabled;
		
		spriteParent.SetActive(!disabled);
	}

	public void Zoom(float delta)
	{

	}
}
