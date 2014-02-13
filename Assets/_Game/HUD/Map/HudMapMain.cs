using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HudMapMain : MonoBehaviour {

	public GameController GC;
	public PlayerMain player;

	public GameObject spriteParent;
	private UISprite[,] mapSprites;

	private int spriteWidth;

	public float zoom = 5.0f;

	private bool initialized = false;

	public bool rotateWithPlayer = true;
	public float returnRotationSpeed = 100.0f;

	public UILabel northIndicator;
	public UISprite playerIndicator;

	// Use this for initialization
	void Start()
	{ }

	public void Init()
	{
		player = GC.Player;

		spriteWidth = GC.SS.PS.FloorSprite.width;

		CreateMap();

		initialized = true;
	}
	
	// Update is called once per frame
	void Update()
	{
		if (!initialized)
			return;

		int mapWidth = mapSprites.GetLength(0);
		int mapHeight = mapSprites.GetLength(1);

		for (int x = 0 ; x < mapWidth; x++)
		{
			for (int y = 0; y < mapHeight; y++)
			{
				UpdateSpritePosition(x, y, mapSprites[x,y]);
			}
		}

		float playerRot = player.transform.rotation.eulerAngles.y;
		if (rotateWithPlayer)
		{	
			float mapRot = spriteParent.transform.rotation.eulerAngles.z;

			if (Mathf.Abs(playerRot - mapRot) < 3.0f)
			{
				Quaternion rot = Quaternion.Euler(0.0f, 0.0f, playerRot);
				spriteParent.transform.rotation = rot;
			}
			else
			{
				Quaternion rot = spriteParent.transform.rotation;
				rot = Quaternion.RotateTowards(rot, Quaternion.Euler(0, 0, playerRot), returnRotationSpeed * Time.deltaTime);
				spriteParent.transform.rotation = rot;
			}

			Quaternion playerIndicatorRot = 
				Quaternion.RotateTowards(playerIndicator.transform.rotation, Quaternion.Euler(0, 0, 0), returnRotationSpeed * Time.deltaTime);
			playerIndicator.transform.rotation = playerIndicatorRot;

			northIndicator.enabled = true;
		}
		else
		{
			if (spriteParent.transform.rotation.eulerAngles != Vector3.zero)
			{
				Quaternion rot = spriteParent.transform.rotation;
				rot = Quaternion.RotateTowards(rot, Quaternion.Euler(0, 0, 0), returnRotationSpeed * Time.deltaTime);
				spriteParent.transform.rotation = rot;
			}
			else
			{
				northIndicator.enabled = false;
			}

			Quaternion playerIndicatorRot = 
				Quaternion.RotateTowards(playerIndicator.transform.rotation, Quaternion.Euler(0, 0, (360 -playerRot)), returnRotationSpeed * Time.deltaTime);
			playerIndicator.transform.rotation = playerIndicatorRot;
		}
	}

	void CreateMap()
	{
		TileMain[,] mapTiles = GC.TileMainMap;
		int mapWidth = mapTiles.GetLength(0);
		int mapHeight = mapTiles.GetLength(1);

		mapSprites = new UISprite[mapWidth, mapHeight];

		for (int x = 0 ; x < mapWidth; x++)
		{
			for (int y = 0; y < mapHeight; y++)
			{
				AddTileSprite(x, y, mapTiles[x,y]);
			}
		}
	}

	void AddTileSprite(int x, int y, TileMain tile)
	{
		if (tile.Data.TileType == TileObjData.Type.Empty
		    || tile.Data.TileType == TileObjData.Type.Wall)
			return;

		UISprite sprite = GameObject.Instantiate(GC.SS.PS.FloorSprite) as UISprite;
		string graphicsName = tile.TileGraphics.name;

		if (graphicsName.EndsWith("(Clone)"))
		{
			graphicsName = graphicsName.Remove(graphicsName.Length-7);
		}

		switch (graphicsName)
		{
		case "Corridor_1wall":
			sprite.spriteName = GC.SS.PS.OneWallSprite.spriteName;
			break;

		case "Corridor_2wall":
			sprite.spriteName = GC.SS.PS.TwoWallsSprite.spriteName;
			break;

		case "Corridor_Corner":
			sprite.spriteName = GC.SS.PS.CorridorCornerSprite.spriteName;
			break;

		case "Corridor_Crossroad":
			sprite.spriteName = GC.SS.PS.CrossroadSprite.spriteName;
			break;

		case "Corridor_DeadEnd":
			sprite.spriteName = GC.SS.PS.DeadendSprite.spriteName;
			break;

		case "Corridor_Floor":
			sprite.spriteName = GC.SS.PS.FloorSprite.spriteName;
			break;

		case "Corridor_Floor1Edge":
			sprite.spriteName = GC.SS.PS.Floor1EdgeSprite.spriteName;
			break;

		case "Corridor_Floor2Edges":
			sprite.spriteName = GC.SS.PS.Floor2EdgesSprite.spriteName;
			break;

		case "Corridor_Floor3Edges":
			sprite.spriteName = GC.SS.PS.Floor3EdgesSprite.spriteName;
			break;

		case "Corridor_FloorOppositeEdges":
			sprite.spriteName = GC.SS.PS.OppositeEdgesSprite.spriteName;
			break;

		case "Corridor_RoomCorner":
			sprite.spriteName = GC.SS.PS.RoomCornerSprite.spriteName;
			break;

		case "Corridor_TCrossing":
			sprite.spriteName = GC.SS.PS.TCrossingSprite.spriteName;
			break;

		case "Corridor_WallCorner":
			sprite.spriteName = GC.SS.PS.WallCornerSprite.spriteName;
			break;

		case "Corridor_WallCornerMirrored":
			sprite.spriteName = GC.SS.PS.WallCornerMirroredSprite.spriteName;
			break;

		case "Door":
			sprite.spriteName = GC.SS.PS.TwoWallsSprite.spriteName;
			break;

		default:
			Debug.Log("Unidentified graphics in map: " + graphicsName);
			break;
		}

		sprite.name = "mapSprite";
		
		sprite.transform.parent = spriteParent.transform;
		sprite.transform.position = spriteParent.transform.position;
		sprite.transform.rotation = spriteParent.transform.rotation;
		
		sprite.transform.localScale = Vector3.one * zoom;

		Vector3 playerPosition = player.transform.position;
		float posX = x - (playerPosition.x / MapGenerator.TileSize.x);
		float posY = y - (playerPosition.z / MapGenerator.TileSize.z);

		sprite.transform.localPosition = new Vector3(posX * (spriteWidth * zoom), posY * (spriteWidth * zoom), 0);

		float graphicsRotation = tile.TileGraphics.transform.rotation.eulerAngles.y;
		graphicsRotation = Mathf.Abs(360 - graphicsRotation);
		Quaternion rot = Quaternion.Euler(0.0f, 0.0f, graphicsRotation);
		sprite.transform.rotation = rot;

		mapSprites[x, y] = sprite;
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
	}

	public void ToggleRotateWithPlayer()
	{
		rotateWithPlayer = !rotateWithPlayer;
	}
}
