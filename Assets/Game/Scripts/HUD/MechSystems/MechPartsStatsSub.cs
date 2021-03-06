﻿using UnityEngine;
using System.Collections;

public class MechPartsStatsSub : MonoBehaviour {

	public UISprite overheatSprite;
	public UISprite partSprite;

	// Use this for initialization
	void Start () {
		if (overheatSprite != null)
			overheatSprite.enabled = false;
	}

	public void ShowOverheat(bool show)
	{
		if (overheatSprite != null)
			overheatSprite.enabled = show;
	}

	public void ChangePartColor(Color color)
	{
		partSprite.color = color;
	}
}
