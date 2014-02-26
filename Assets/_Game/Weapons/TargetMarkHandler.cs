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
	
	public TargetMarkHandler(GameController gameController, Vector3 crosshairPosition)
	{
		GC = gameController;

		parentObject = new GameObject();
		parentObject.name = "TargetMarkHandler";
		parentObject.transform.parent = GC.menuHandler.targetMarkPanel.transform;
		
		crosshair = GameObject.Instantiate(GC.SS.PS.InsightSprite) as UISprite;
		crosshair.transform.parent = parentObject.transform;
		crosshair.spriteName = "crosshair_gray";
		crosshair.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
		crosshair.transform.position = GC.menuHandler.NGUICamera.ScreenToWorldPoint(crosshairPosition);
		crosshair.enabled = true;
		
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
			
			label.transform.parent = parentObject.transform;
			label.transform.localScale = new Vector3(0.0025f, 0.0025f, 0.0025f);
			label.transform.position += crosshair.transform.position + labelPosOffsetDirs[i]*0.08f;
			label.color = GC.menuHandler.gunInfoDisplay.GetWeaponColor((WeaponID)i);
			label.enabled = true;

			numShotsLabels.Add(label);
		}
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