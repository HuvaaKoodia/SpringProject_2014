using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrefabStore : MonoBehaviour {

	public EnemyMain EnemyPrefab;
	public EnemyMain GatlingTurretPrefab;

	public TileMain TilePrefab;
	public GameObject LootCratePrefab;
	public DataTerminalMain DataTerminalPrefab;

	public GameObject PowerGenerator,MechaDummy;

	public GameObject 
			BasicWall,
			BasicEmpty
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
		Corridor_Door,
		Corridor_ElevatorDoor,
		Corridor_Airlock_door,
		Corridor_Elevator,
		ElevatorTrigger
		;

	public UISprite
		InsightSprite,
		TargetedSprite,
		PlayerBlipSprite,
		EnemyBlipSprite,
		LootBlipSprite,
		FloorSprite
		;

	public UILabel
		NumShotsLabel
		;

	public RadarBlipSub RadarBlip;

	//Weapon graphics
	public Dictionary<string, GameObject> weaponGraphics;
	public Dictionary<string, GameObject> weaponParticleEmitters,weaponFlashes;
	public Dictionary<string, AudioClip> weaponSoundFX;

	public List<GameObject> targetHighlights;

	public void Awake()
	{
		weaponGraphics = new Dictionary<string, GameObject>();
		weaponGraphics.Add("Shotgun", Resources.Load("Weapons/Shotgun") as GameObject);
		weaponGraphics.Add("Gatling", Resources.Load("Weapons/GatlingArm") as GameObject);
		weaponGraphics.Add("Cannon", Resources.Load("Weapons/Cannon") as GameObject);
		weaponGraphics.Add("Melee", Resources.Load("Weapons/Gladius") as GameObject);
		weaponGraphics.Add("NotFound", Resources.Load("Weapons/WeaponNotFound") as GameObject);

		weaponParticleEmitters = new Dictionary<string, GameObject>();
		weaponParticleEmitters.Add("GatlingBullets", Resources.Load("WeaponParticleEmitters/GatlingBullets") as GameObject);
		weaponParticleEmitters.Add("GatlingMuzzle", Resources.Load("WeaponParticleEmitters/GatlingMuzzle") as GameObject);
		weaponParticleEmitters.Add("CannonBullets", Resources.Load("WeaponParticleEmitters/CannonBullets") as GameObject);
		weaponParticleEmitters.Add("CannonMuzzle", Resources.Load("WeaponParticleEmitters/CannonMuzzle") as GameObject);
		weaponParticleEmitters.Add("ShotgunBullets", Resources.Load("WeaponParticleEmitters/ShotgunBullets") as GameObject);
		weaponParticleEmitters.Add("ShotgunMuzzle", Resources.Load("WeaponParticleEmitters/ShotgunMuzzle") as GameObject);
		weaponParticleEmitters.Add("MeleeBullets", Resources.Load("WeaponParticleEmitters/GladiusBullets") as GameObject);
		weaponParticleEmitters.Add("GaussBullets", Resources.Load("WeaponParticleEmitters/GaussBullets") as GameObject);
		weaponParticleEmitters.Add("MissileBullets", Resources.Load("WeaponParticleEmitters/MissileBullets") as GameObject);

		weaponFlashes = new Dictionary<string, GameObject>();
		LoadFlash("Melee");
		LoadFlash("Gatling");
		LoadFlash("Gauss");
		LoadFlash("Shotgun");
		LoadFlash("Missile");
		LoadFlash("Cannon");

		weaponSoundFX = new Dictionary<string, AudioClip>();
		weaponSoundFX.Add("Gatling", Resources.Load("Weapons/Gatling_gun_v3") as AudioClip);
		weaponSoundFX.Add("Cannon", Resources.Load("Weapons/Cannon_v3") as AudioClip);
		weaponSoundFX.Add("Shotgun", Resources.Load("Weapons/Shotgun_v2") as AudioClip);

		targetHighlights = new List<GameObject>();
		targetHighlights.Add(Resources.Load("TargetingSprites/LeftDown") as GameObject);
		targetHighlights.Add(Resources.Load("TargetingSprites/LeftUp") as GameObject);
		targetHighlights.Add(Resources.Load("TargetingSprites/RightDown") as GameObject);
		targetHighlights.Add(Resources.Load("TargetingSprites/RightUp") as GameObject);
	}

	private void LoadFlash(string weapon){
		string name=weapon+"Flash";
		weaponFlashes.Add(name, Resources.Load("WeaponFlashes/"+name) as GameObject);
	}

	public string MapSpritePrefabFolderName="MapSprites/";

	public GameObject GetMapSpritePrefab(string graphicsName)
	{
		return Resources.Load(MapSpritePrefabFolderName+graphicsName) as GameObject;
	}

	public string[] clutter_names={"AmmoboxesPrefab","BarrelsPrefab","PlasticBarrelsPrefab","RandomBoxesPrefab","ServerThigiesPrefab"};//,"Turret_Parts"

	public Object GetRandomClutter ()
	{
		return Resources.Load("Clutter/"+Subs.GetRandom(clutter_names));
	}
}
