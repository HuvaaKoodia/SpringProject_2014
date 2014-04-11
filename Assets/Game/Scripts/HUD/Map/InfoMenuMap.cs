using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class InfoMenuMap : MonoBehaviour {

	GameController GC;
	PlayerMain player;
	
	public GameObject spriteParent;
	public UIDragObject UIDrag;

	private List<UISprite[,]> mapSprites;
	private int currentFloor = 0;
	
	private int spriteWidth;

	public UISprite playerIndicator;
	
	public UIPanel spritePanel;

	public UILabel CurrentFloorLabel;
	public GameObject FloorUpButton;
	public GameObject FloorDownButton;

	bool _disabled=false;

	float adjustedPosX;
	float adjustedPosY;
	
	public void Init(GameController gc)
	{
		GC=gc;
		player = GC.Player;
		
		spriteWidth = GC.SS.PS.FloorSprite.width;

		mapSprites = new List<UISprite[,]>();
		
		GC.MiniMapData.CreateMapSprites(spriteParent, mapSprites, 6, spriteWidth);
		
		ChangeFloor(GC.CurrentFloorIndex);


		adjustedPosX = (mapSprites[0].GetLength(0)-1)* ((float)spriteWidth * 3.0f);
		adjustedPosY = (mapSprites[0].GetLength(1)-1)* ((float)spriteWidth * 3.0f);

		for (int i = 0; i < mapSprites.Count; i++)
		{
			for (int x = 0; x < mapSprites[i].GetLength(0); x++)
			{
				for (int y = 0; y < mapSprites[i].GetLength(1); y++)
				{
					if (mapSprites[i][x,y] != null)
						mapSprites[i][x,y].transform.localPosition -= new Vector3(adjustedPosX, adjustedPosY);
				}
			}
		}

		FloorDownButton.SetActive(true);
		FloorUpButton.SetActive(true);
		CurrentFloorLabel.gameObject.SetActive(true);

		if (currentFloor >= mapSprites.Count-1)
		{
			FloorDownButton.SetActive(false);
		}
		if (currentFloor <= 0)
		{
			FloorDownButton.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		if (playerIndicator.gameObject.activeSelf == false)
			return;

		float playerRot = player.transform.rotation.eulerAngles.y;

		Quaternion playerIndicatorRot = 
			Quaternion.Euler(0, 0, (360 -playerRot));
		playerIndicator.transform.localRotation = playerIndicatorRot;

		int posX = GC.Player.movement.currentGridX;
		int posY = GC.Player.movement.currentGridY;

		playerIndicator.transform.localPosition = mapSprites[currentFloor][posX,posY].transform.localPosition;
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
					{
						if (mapTiles[x,y].SeenByPlayer == true)
								mapSprites[i][x,y].enabled = enable;
						else
							mapSprites[i][x,y].enabled = false;
					}
				}
			}
		}

		CurrentFloorLabel.text = currentFloor + "";

		playerIndicator.gameObject.SetActive(currentFloor == GC.CurrentFloorIndex);

		UIDrag.ConstraintToBounds();
	}

	public void FloorUp()
	{
		if (currentFloor < mapSprites.Count-1)
			ChangeFloor(currentFloor+1);

		if (currentFloor <= mapSprites.Count)
		{
			FloorUpButton.SetActive(false);
		}

		FloorDownButton.SetActive(true);
	}

	public void FloorDown()
	{
		if (currentFloor > 0)
			ChangeFloor(currentFloor-1);
		
		if (currentFloor <= 0)
		{
			FloorDownButton.SetActive(false);
		}

		FloorUpButton.SetActive(true);
	}

	public void SetDisabled(bool disabled){
		_disabled=disabled;
		
		spriteParent.SetActive(!disabled);
	}

	public void BecomeVisible()
	{
		ChangeFloor(currentFloor);
	}
}
