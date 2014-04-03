using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetMarkHandler
{
	public GameController GC;
	
	GameObject parentObject;
	UISprite crosshair;
	List<UILabel> numShotsLabels;

	public bool IsVisible
	{
		get { return crosshair.enabled; }
	}
	
	public TargetMarkHandler(GameController gameController, Vector3 crosshairPosition, int rotation)
	{
		GC = gameController;

		parentObject = new GameObject();
		parentObject.name = "TargetMarkHandler";

		Vector3 screenToWorldPoint = GC.menuHandler.player.HudCamera.ScreenToWorldPoint(crosshairPosition);

		parentObject.transform.position = screenToWorldPoint;
		parentObject.transform.parent = GC.Player.HUD.targetMarkPanel.transform;
		
		crosshair = GameObject.Instantiate(GC.SS.PS.InsightSprite) as UISprite;
		crosshair.transform.parent = parentObject.transform;
		crosshair.spriteName = "crosshair_gray";
		crosshair.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
		crosshair.transform.position = screenToWorldPoint;

		crosshair.enabled = true;

		//crosshair.gameObject.AddComponent("UIButton");
		
		numShotsLabels = new List<UILabel>();
		Vector3[] labelPosOffsetDirs =
		{
            new Vector3(Mathf.Cos(150 * Mathf.Deg2Rad), Mathf.Sin(150 * Mathf.Deg2Rad), 0),
			new Vector3(Mathf.Cos(110 * Mathf.Deg2Rad), Mathf.Sin(110 * Mathf.Deg2Rad), 0),
            new Vector3(Mathf.Cos(30 * Mathf.Deg2Rad), Mathf.Sin(30 * Mathf.Deg2Rad), 0),
			new Vector3(Mathf.Cos(70 * Mathf.Deg2Rad), Mathf.Sin(70 * Mathf.Deg2Rad), 0)
		};
		
		for(int i = 0; i < 4; i++)
		{
			UILabel label = GameObject.Instantiate(GC.SS.PS.NumShotsLabel) as UILabel;
			label.text = "";
		
			float distanceMultiplier = 1 + (screenToWorldPoint - GC.Player.HudCamera.transform.position).magnitude / 2.0f;

			label.transform.parent = crosshair.transform;
			label.transform.position += crosshair.transform.position + labelPosOffsetDirs[i]*0.08f*(distanceMultiplier/1.35f);

			label.color = GC.Player.HUD.gunInfoDisplay.GetWeaponColor((WeaponID)i);
			label.enabled = true;

			label.MakePixelPerfect();		

			label.transform.localScale = new Vector3(2.0f*distanceMultiplier, 2.0f*distanceMultiplier, 2.0f*distanceMultiplier);

			numShotsLabels.Add(label);
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
			crosshair.spriteName = "crosshair_red";
		}
		else
		{
			numShotsLabels[(int)gun].text = "";
			crosshair.spriteName = "crosshair_gray";
		}
	}

	public void SetCrosshairVisible(bool visible)
	{
		crosshair.enabled = visible;
	}
}