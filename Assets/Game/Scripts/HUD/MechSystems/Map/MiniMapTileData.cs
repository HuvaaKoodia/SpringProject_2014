using UnityEngine;
using System.Collections;

public class MiniMapTileData {

	public readonly string TileType;
	public readonly int Rotation;
	public bool SeenByPlayer;

	public MiniMapTileData(string type, int rotation)
	{
		TileType = type;
		Rotation = rotation;

		SeenByPlayer = false;
	}
}
