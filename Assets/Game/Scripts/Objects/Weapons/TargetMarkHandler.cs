﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetMarkHandler
{
	public GameController GC;
	
	GameObject parentObject;
	GameObject textParent;

	UISprite[] crosshairSprites;
	string[] notTargetedSpriteNames;
	string[] targetedSpriteNames;

	GameObject[] targetingHighligts;

	float[] rotationSpeeds;

	List<UILabel> numShotsLabels;

	public bool IsVisible
	{
		get { return crosshairSprites[0].enabled; }
	}

	bool isTargeting
	{
		get { return crosshairSprites[0].spriteName == "crosshair_red"; }
	}

	public TargetMarkHandler(GameController gameController, Vector3 crosshairPosition, int rotation)
	{
		GC = gameController;
		
		parentObject = new GameObject();
		parentObject.name = "TargetMarkHandler";

		textParent = new GameObject();
		textParent.name = "TargetTexts";

		textParent.transform.parent = parentObject.transform;

		Vector3 screenToWorldPoint = GC.Player.PlayerCamera.ScreenToWorldPoint(crosshairPosition);
		
		parentObject.transform.position = screenToWorldPoint;
		parentObject.transform.parent = GC.Player.HUD.targetMarkPanel.transform;

		crosshairSprites = new UISprite[3];

		notTargetedSpriteNames = new string[]{"crosshair_grey", "crosshair_grey_med", "crosshair_grey_small"};
		targetedSpriteNames = new string[]{"crosshair_red", "crosshair_red_med", "crosshair_red_small"};

		rotationSpeeds = new float[]{ 30.0f, -23.0f, 5.0f };

		for (int i = 0; i < 3; i++)
		{
			crosshairSprites[i] = GameObject.Instantiate(GC.SS.PS.InsightSprite) as UISprite;
			crosshairSprites[i].transform.parent = parentObject.transform;
			crosshairSprites[i].spriteName = notTargetedSpriteNames[i];
			crosshairSprites[i].transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
			crosshairSprites[i].transform.position = screenToWorldPoint;
		
			crosshairSprites[i].enabled = true;
		}
		//crosshair.gameObject.AddComponent("UIButton");

		numShotsLabels = new List<UILabel>();

		float[] labelDegrees = { 180, 130, 360, 50 };
		Vector3[] labelPosOffsetDirs =
		{
			new Vector3(Mathf.Cos(labelDegrees[0] * Mathf.Deg2Rad), Mathf.Sin(labelDegrees[0] * Mathf.Deg2Rad), 0),
			new Vector3(Mathf.Cos(labelDegrees[1] * Mathf.Deg2Rad), Mathf.Sin(labelDegrees[1] * Mathf.Deg2Rad), 0),
			new Vector3(Mathf.Cos(labelDegrees[2] * Mathf.Deg2Rad), Mathf.Sin(labelDegrees[2] * Mathf.Deg2Rad), 0),
			new Vector3(Mathf.Cos(labelDegrees[3] * Mathf.Deg2Rad), Mathf.Sin(labelDegrees[3] * Mathf.Deg2Rad), 0)
		};

		Vector3[] highlightOffsets =
		{
			new Vector3(0.01f, -0.03f, 0.0f),
			new Vector3(-0.014f, -0.05f, 0.0f),
			new Vector3(-0.01f, -0.03f, 0.0f),
			new Vector3(0.014f, -0.05f, 0.0f)
		};

		Vector3[] labelPosOffsets =
		{
			Vector3.zero,
			new Vector3(-0.02f, -0.005f, 0.0f),
			Vector3.zero,
			new Vector3(0.02f, -0.005f, 0.0f)
		};

		targetingHighligts = new GameObject[4];

		textParent.transform.position = crosshairSprites[0].transform.position;
		textParent.transform.localScale = new Vector3(0.003f, 0.003f, 0.003f);

		for(int i = 0; i < 4; i++)
		{
			UILabel label = GameObject.Instantiate(GC.SS.PS.NumShotsLabel) as UILabel;
			label.text = "";
			
			float distanceMultiplier = 1 + (screenToWorldPoint - GC.Player.PlayerCamera.transform.position).magnitude / 2.0f;

			if (i == 1 || i == 3)
			{
				distanceMultiplier *= 1.2f;
			}

			label.transform.parent = textParent.transform;
			label.transform.position = textParent.transform.position + labelPosOffsetDirs[i]*0.08f*(distanceMultiplier/1.2f)+labelPosOffsets[i];
			
			label.color = GC.Player.HUD.gunInfoDisplay.GetWeaponColor((WeaponID)i);
			label.enabled = true;
			
			label.MakePixelPerfect();		
			
			//label.transform.localScale = new Vector3(2.0f*distanceMultiplier, 2.0f*distanceMultiplier, 2.0f*distanceMultiplier);
			
			numShotsLabels.Add(label);

			GameObject highlight = GameObject.Instantiate(GC.SS.PS.targetHighlights[i]) as GameObject;
			highlight.transform.parent = textParent.transform;
			highlight.transform.localScale = Vector3.one * 0.2f;
			highlight.transform.position = textParent.transform.position + labelPosOffsetDirs[i]*0.08f*(distanceMultiplier/1.2f) + highlightOffsets[i];

			highlight.GetComponent<UISprite>().color = label.color = GC.Player.HUD.gunInfoDisplay.GetWeaponColor((WeaponID)i);
			highlight.SetActive(false);
			targetingHighligts[i] = highlight;
		}
		
		parentObject.transform.Rotate(0, rotation, 0);
		
	}
	
	public void DeInit()
	{
		GameObject.Destroy(parentObject);
	}
	
	public void ChangeNumShots(WeaponID gun, int numShots,int hit_percent)
	{
		if (numShots != 0)
		{
			numShotsLabels[(int)gun].text = numShots.ToString()+"\n"+hit_percent+"%";
			targetingHighligts[(int)gun].SetActive(true);
			for (int i = 0; i < 3; i++)
			{
				crosshairSprites[i].spriteName = targetedSpriteNames[i];
			}
		}
		else
		{
			numShotsLabels[(int)gun].text = "";
			targetingHighligts[(int)gun].SetActive(false);
			for (int i = 0; i < 3; i++)
			{
				crosshairSprites[i].spriteName = notTargetedSpriteNames[i];
			}
		}
	}
	
	public void SetCrosshairVisible(bool visible)
	{
		for (int i = 0; i < 3; i++)
			crosshairSprites[i].enabled = visible;
	}

	public void Update(float dt)
	{
		if (!isTargeting)
			return;

		for (int i = 0; i < 3; i++)
		{
			crosshairSprites[i].transform.Rotate(0,0,rotationSpeeds[i]*dt);
		}
	}
}