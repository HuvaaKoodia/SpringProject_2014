using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HudMapMain : MonoBehaviour {

	public GameController GC;
	public PlayerMain player;

	public GameObject spriteParent;

	private List<UISprite[,]> mapSprites;
	private int currentFloor = 0;

	private int spriteWidth;

	public float zoom = 5.0f;

	private bool initialized = false;

	public bool rotateWithPlayer = true;
	public float returnRotationSpeed = 100.0f;

	public UILabel northIndicator;
	public UISprite playerIndicator;

	public UIPanel spritePanel;

	bool _disabled=false;

	// Use this for initialization
	void Awake()
	{ 
		mapSprites = new List<UISprite[,]>();
	}

	public void Init()
	{
		player = GC.Player;

		spriteWidth = GC.SS.PS.FloorSprite.width;

		mapSprites = new List<UISprite[,]>();

		GC.MiniMapData.CreateMapSprites(spriteParent, mapSprites, zoom, spriteWidth);

		ChangeFloor(GC.CurrentFloorIndex);

		float playerRot = player.transform.rotation.eulerAngles.y;
		Quaternion rot = Quaternion.Euler(0.0f, 0.0f, playerRot);

		playerIndicator.transform.localRotation =  Quaternion.Euler(0, 0, 360 - playerRot);

		if (rotateWithPlayer)
		{	
			spriteParent.transform.localRotation = rot;
		}

		initialized = true;
	}
	
	// Update is called once per frame
	void Update()
	{
		if (!initialized||_disabled)
			return;

		int mapWidth = mapSprites[currentFloor].GetLength(0);
		int mapHeight = mapSprites[currentFloor].GetLength(1);

		returnRotationSpeed = player.movement.turnSpeed;

		for (int x = 0 ; x < mapWidth; x++)
		{
			for (int y = 0; y < mapHeight; y++)
			{
				UpdateSpritePosition(x, y, mapSprites[currentFloor][x,y]);
			}
		}

		float playerRot = player.transform.rotation.eulerAngles.y;
		if (rotateWithPlayer)
		{	
			float mapRot = spriteParent.transform.localRotation.eulerAngles.z;

			if (Mathf.Abs(playerRot - mapRot) < 3.0f)
			{
				Quaternion rot = Quaternion.Euler(0.0f, 0.0f, playerRot);
				spriteParent.transform.localRotation = rot;
			}
			else
			{
				Quaternion rot = spriteParent.transform.localRotation;
				rot = Quaternion.RotateTowards(rot, Quaternion.Euler(0, 0, playerRot), returnRotationSpeed * Time.deltaTime);
				spriteParent.transform.localRotation = rot;
			}

			Quaternion playerIndicatorRot = 
				Quaternion.RotateTowards(playerIndicator.transform.localRotation, Quaternion.Euler(0, 0, 360 - playerRot), returnRotationSpeed * Time.deltaTime);
			playerIndicator.transform.localRotation = playerIndicatorRot;

			northIndicator.enabled = true;
		}
		else
		{
			if (spriteParent.transform.localRotation.eulerAngles != Vector3.zero)
			{
				Quaternion rot = spriteParent.transform.localRotation;
				rot = Quaternion.RotateTowards(rot, Quaternion.Euler(0, 0, 0), returnRotationSpeed * Time.deltaTime);
				spriteParent.transform.localRotation = rot;
			}
			else
			{
				northIndicator.enabled = false;
			}

			Quaternion playerIndicatorRot = 
				Quaternion.RotateTowards(playerIndicator.transform.localRotation, Quaternion.Euler(0, 0, (360 -playerRot)), returnRotationSpeed * Time.deltaTime);
			playerIndicator.transform.localRotation = playerIndicatorRot;
		}
	}

	void UpdateSpritePosition(float x, float y, UISprite sprite)
	{
		if (sprite == null)
			return;

		Vector3 playerPosition = player.transform.position;
		float posX = x - (playerPosition.x / MapGenerator.TileSize.x);
		float posY = y - (playerPosition.z / MapGenerator.TileSize.z);

		sprite.transform.localScale = Vector3.one * zoom;
		sprite.transform.localPosition = new Vector3(posX * (spriteWidth * zoom), posY * (spriteWidth * zoom), 0);

		if (sprite.transform.localPosition.magnitude < spritePanel.finalClipRegion.magnitude/2)
		{
			GC.MiniMapData.GetFloor(currentFloor)[(int)x,(int)y].SeenByPlayer = true;
		}
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
	public void ToggleRotateWithPlayer()
	{
		rotateWithPlayer = !rotateWithPlayer;
	}

	public void SetDisabled(bool disabled){
		_disabled=disabled;

		spriteParent.SetActive(!disabled);
	}
}
