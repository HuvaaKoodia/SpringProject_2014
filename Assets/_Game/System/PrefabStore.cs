using UnityEngine;
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
		Corridor_TCrossing,
		Corridor_Floor
		;
}
