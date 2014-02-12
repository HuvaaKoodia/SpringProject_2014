﻿using UnityEngine;
using System.Collections;

public class PrefabStore : MonoBehaviour {
	
	public PlayerMain PlayerPrefab;
	public EnemyMain EnemyPrefab;
	public TileMain TilePrefab;
	public GameObject LootCratePrefab;
	
	public GameObject 
			BasicFloor,
			BasicWall,
			BasicEmpty,
			BasicDoor
		;

	public GameObject 
		Corridor_OneWall,
		Corridor_TwoWall,
		Corridor_Corner,
		Corridor_RoomCorner,
		Corridor_WallCorner,
		Corridor_WallCornerMirrored,
		Corridor_TCrossing,
		Corridor_Floor,
		Corridor_Floor1Edge,
		Corridor_Floor2Edges,
		Corridor_Floor3Edges,
		Corridor_FloorOppositeEdges,
		Corridor_Crossroad,
		Corridor_Deadend,
		Corridor_Door
		;

	public UISprite
		InsightSprite,
		TargetedSprite,
		PlayerBlipSprite,
		EnemyBlipSprite,
		LootBlipSprite,
		FloorSprite,
		OneWallSprite,
		TwoWallsSprite,
		RoomCornerSprite,
		CorridorCornerSprite,
		CrossroadSprite,
		DeadendSprite,
		Floor1EdgeSprite,
		Floor2EdgesSprite,
		Floor3EdgesSprite,
		OppositeEdgesSprite,
		TCrossingSprite,
		WallCornerSprite,
		WallCornerMirroredSprite
		;

	public UILabel
		NumShotsLabel
		;
}