//
// Radar - Based off of the other radar scripts I've seen on the wiki
// By: Justin Zaun
//
// Attach this wherever you like, I recommend you attach it where the rest of your GUI is. Once
// attached take a look at the inspector for the object. You are going to see a bunch of options
// to setup. I've tried to give a set of defaults that will work with little messing around.
//
// The first item that should be set is the "Radar Center Tag" for anything of interesting to
// happen. This tag should be the object at the center of the radar, typically this is the local
// player's object. Place a check in the "Radar Center Active" box to diplay the play on the radar.
//
// The second item that should be set is the "Radar Blip1 Tag." This is the tag given to the
// objects you want to show on the radar. Typically this would be the remote player's tag or
// the bad guy's tag.
//
// To turn on the display of the blip place a check in the "Radar Blip1 Active" box.
//
// If you run your game at this point you will see a radar in the bottom center of your screen
// that is showing you all the remote players/bad guys that are around you.
//
// Now that you have seen a quick example of the radar I'll explain all the options. There looks
// like a lot but they are split into two sections. At the top are general radar settings that
// determin how you would like the radar to look. On the bottom there are settings for the blips
//
// I'll explain the blips first. This radar supports up to 4 types of blips. Each blip has an
// "Radar Blip# Active" option for turning on or off the blip. This allows you to have everything
// setup and then in game based on events to turn on the display of different types on blips.
//
// The second options is the "Radar Blip# Color" setting. This is the color of the blip. Not hard
// to explain, it changed the color of the object's blips for a given Tag. The last option for a
// blip is "Radar Blip# Tag." This is the tag for the object you would like to have on the radar.
//
// Some examples would be using Blip1 for bad guys, Blip2 for items the play is trying to find,
// Blip3 for forts and Blip4 could be the level's exits. Having the items on the radar in differnt
// colors will let the player identify the type of object the blip represents.
//
// The top options are the overall settings. First is the location of the radar. There are several
// options to choose from. If you choose "Custom" you have to fill in the "Radar Location Custom"
// to define the location. When you are defining the location please note this is the center of the
// radar.
//
// The third option "Radar Type" is how you would like your radar to look. You have a choice of
// Textured, Round and Transparent. The default is Round and is the colored bullseye you
// saw at the start if you played with the first example. If you choose Textured you MUST set
// "Radar Texture" to some image for the background. If you choose Round you can change the colors
// used in the bullseye with "Radar BackroundA" and "Radar BackgroundB"
//
// The last two options are about the size and zoom of the radar. The "Radar Size" is a percent
// of the screen the radar will take up, for example .2 is 20% of the screen. The "Radar Zoom"
// determines how much of the map should be displayed on the radar. Making this number smaller
// will zoom out and show you more stuff.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RadarMain : MonoBehaviour
{
	public GameController GC;
	public Transform radarPanelTransform;
	public GameObject RadarThingsPanel;

	public List<RadarBlipSub> blips;
	public int currentBlip = 0;

	public UISprite radarViewSprite;
	public GameObject blipParent;

	//background
	public UISprite radarBG;

	// Display size
	public float radarZoom = 0.60f;
	
	// Center Object information
	public bool radarCenterActive;

	public bool rotateWithCenter = true;
	public float returnRotationSpeed = 100.0f;

	// Blip information
	public bool   enemyBlipActive;
	List<EnemyMain> enemies;
	
	public bool   lootBlipActive;
	List<LootCrateMain> lootCrates;

	//circle scan
	public UISprite circleScanSprite;
	public bool circleScanActive = true;
	public float scanSpeed = 1.0f;
	public float scanRadiusMax = 220;
	float scanRadius;

	// Internal vars
	private GameObject _centerObject;
	private UISprite  _radarCenterTexture;
	private UISprite  enemyBlipTexture;
	private UISprite  lootBlipTexture;
	private float BGWidth;

	bool initialized = false;

	bool _disabled=false;

	public bool DrawOnlyLastPhaseMovers = true;
	public bool DrawOnlyUnopenedLoot = true;
	// Initialize the radar
	public void Init()
	{	
		// Get our center object
		_centerObject = GC.Player.gameObject;

		BGWidth = radarBG.width;

		enemies = GC.CurrentFloorData.Enemies;
		lootCrates = GC.CurrentFloorData.LootCrates;

		blips = new List<RadarBlipSub>();

		_radarCenterTexture = GC.SS.PS.PlayerBlipSprite;
		enemyBlipTexture = GC.SS.PS.EnemyBlipSprite;
		lootBlipTexture = GC.SS.PS.LootBlipSprite;

		circleScanSprite.transform.localScale = Vector3.zero;

		float playerRot = GC.Player.transform.rotation.eulerAngles.y;

		if (rotateWithCenter)
		{
			Quaternion rot = Quaternion.Euler(0.0f, 0.0f, playerRot);
			blipParent.transform.localRotation = rot;
		}
		else
		{
			Quaternion viewSpriteRot = Quaternion.RotateTowards(radarViewSprite.transform.rotation, Quaternion.Euler(0, 0, 360 - playerRot), 360);
			radarViewSprite.transform.rotation = viewSpriteRot;
		}
		initialized = true;

		for (int i = enemies.Count + lootCrates.Count; i >= 0; i--)
		{
			createBlip();
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		if (!initialized||_disabled)
			return;

		returnRotationSpeed = GC.Player.movement.turnSpeed;

		if (circleScanActive)
		{
			circleScanSprite.enabled = true;
			circleScanSprite.transform.localScale += Vector3.one * scanSpeed * Time.deltaTime;
			scanRadius = circleScanSprite.transform.localScale.x * (BGWidth / 2.0f);
			if (scanRadius > scanRadiusMax)
			{
				circleScanSprite.transform.localScale = Vector3.zero;
				scanRadius = 0;

				ResetBlipCycles();
			}
		}
		else
		{
			circleScanSprite.enabled = false;
		}

		currentBlip = 0;

		// Draw blips
		if (enemyBlipActive)
		{
			// Iterate through them and call drawBlip function
			foreach (EnemyMain enemy in enemies)
			{
				if (!DrawOnlyLastPhaseMovers || enemy.MovedLastPhase)
				{
					drawBlip(enemy.gameObject, enemyBlipTexture);
				}
				currentBlip++;
			}
		}
		if (lootBlipActive)
		{
			foreach (LootCrateMain loot in lootCrates)
			{
				if (!DrawOnlyUnopenedLoot || !loot.isOpen)
				{
					drawBlip(loot.gameObject, lootBlipTexture);
					currentBlip++;
				}
			}
		}
		
		// Draw center oject
		if (radarCenterActive)
		{
			drawBlip(_centerObject, _radarCenterTexture);
			currentBlip++;
		}

		for (int i = currentBlip; i < blips.Count; i++)
			blips[i].enabled = false;


		float playerRot = GC.Player.transform.rotation.eulerAngles.y;
		float radarRot = blipParent.transform.localRotation.eulerAngles.z;

		if (rotateWithCenter)
		{	
			if (Mathf.Abs(playerRot - radarRot) < 3.0f)
			{
				Quaternion rot = Quaternion.Euler(0.0f, 0.0f, playerRot);
				blipParent.transform.localRotation = rot;
			}
			else
			{
				Quaternion rot = blipParent.transform.localRotation;
				rot = Quaternion.RotateTowards(rot, Quaternion.Euler(0, 0, playerRot), returnRotationSpeed * Time.deltaTime);
				blipParent.transform.localRotation = rot;
			}

			if (Mathf.Abs(playerRot - radarViewSprite.transform.localRotation.z) < 3.0f)
			{
				radarViewSprite.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
			}
			else
			{
				Quaternion rot = Quaternion.RotateTowards(radarViewSprite.transform.localRotation, Quaternion.Euler(0, 0, 0), returnRotationSpeed * Time.deltaTime);
				radarViewSprite.transform.localRotation = rot;
			}
		}
		else 
		{
			if (blipParent.transform.localRotation.eulerAngles != Vector3.zero)
			{
				Quaternion rot = blipParent.transform.localRotation;
				rot = Quaternion.RotateTowards(rot, Quaternion.Euler(0, 0, 0), returnRotationSpeed * Time.deltaTime);
				blipParent.transform.localRotation = rot;
			}

			Quaternion viewSpriteRot = Quaternion.RotateTowards(radarViewSprite.transform.localRotation, Quaternion.Euler(0, 0, 360 - playerRot), returnRotationSpeed * Time.deltaTime);
			radarViewSprite.transform.localRotation = viewSpriteRot;
		}

	}
	
	// Draw a blip for an object
	void drawBlip(GameObject go, UISprite blipTexture)
	{
		if (_centerObject)
		{
			Vector3 centerPos = _centerObject.transform.position;
			Vector3 extPos = go.transform.position;		
			
			// Get the object's offset from the centerObject
			float dx = centerPos.x - extPos.x;
			float dy = centerPos.z - extPos.z;
			
			// just basic trigonometry to find the point x,y (enemy's location) given the angle deltay
			float bX = -dx;
			float bY = -dy;

			// Scale the objects position to fit within the radar
			bX = bX * radarZoom;
			bY = bY * radarZoom;

			if (blips.Count <= currentBlip)
			{
				createBlip();
			}

			Vector3 posOffset = new Vector3(bX, bY, 0);

			posOffset.x *= radarZoom;
			posOffset.y *= radarZoom;

			bool onTopOfOther = false;
			
			Vector3 position = blipParent.transform.localPosition + posOffset;

			for (int i = 0; i < currentBlip; i++)
			{
				if (blips[i].transform.localPosition == position)
				{
					onTopOfOther = true;
					break;
				}
			}

			if (posOffset.magnitude <= scanRadius || !circleScanActive)
			{

				if (!onTopOfOther || blipTexture.spriteName == "playerBlip")
				{
					blips[currentBlip].transform.localPosition = position;
					blips[currentBlip].transform.localScale = Vector3.one * radarZoom;
					blips[currentBlip].enabled = true;
					blips[currentBlip].SetSpriteName(blipTexture.spriteName);
				}

				if (blipTexture.spriteName == "playerBlip" || !circleScanActive)
				{
					blips[currentBlip].fades = false;
					blips[currentBlip].ResetCycle();
				}
				else
					blips[currentBlip].fades = true;
			}
			else
			{
				if (!onTopOfOther || blipTexture.spriteName == "playerBlip")
				{
					blips[currentBlip].transform.localPosition = position;
					blips[currentBlip].transform.localScale = Vector3.one * radarZoom;
				}
			}
		}
	}

	void createBlip()
	{
		RadarBlipSub blip = GameObject.Instantiate(GC.SS.PS.RadarBlip) as RadarBlipSub;

		blip.name = "radarBlip";

		blip.transform.parent = blipParent.transform;
		blip.transform.position = blipParent.transform.position;
		blip.transform.localRotation = Quaternion.Euler(0,0,0);

		blip.transform.localScale = Vector3.one * radarZoom;

		blips.Add(blip);
	}

	void ResetBlipCycles()
	{
		foreach(RadarBlipSub blip in blips)
		{
			blip.ResetCycle();
		}
	}
	public void ToggleRotateWithCenter()
	{
		rotateWithCenter = !rotateWithCenter;
	}

	public void SetDisabled(bool disabled){
		_disabled=disabled;
		
		RadarThingsPanel.SetActive(!disabled);
	}

	public void ChangeFloor(int index)
	{
		enemies = GC.GetFloor(index).Enemies;
		lootCrates = GC.GetFloor(index).LootCrates;

		for (int i = 0; i < blips.Count; i++)
		{
			blips[i].MakeInvisible();
		}
	}
}