using UnityEngine;
using System.Collections;

public class MainMenuButtonGraphics : MonoBehaviour {
	
	public UISprite Spr;
	UIButton button;

	public string NormalSprite,HoverSprite;

	void Start () {
		OnHover(false);

		button=GetComponent<UIButton>();
	}

	void OnHover (bool isOver)
	{
		if (button!=null&&!button.enabled) return;
		Spr.spriteName=isOver?HoverSprite:NormalSprite;
	}
}
