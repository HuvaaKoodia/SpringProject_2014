using UnityEngine;
using System.Collections;

public class MainMenuButtonGraphics : MonoBehaviour {
	
	public UISprite Spr;

	public string NormalSprite,HoverSprite;

	void Start () {
		OnHover(false);
	}

	void OnHover (bool isOver)
	{
		Spr.spriteName=isOver?HoverSprite:NormalSprite;
	}
}
