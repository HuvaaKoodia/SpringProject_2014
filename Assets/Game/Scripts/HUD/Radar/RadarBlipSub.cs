using UnityEngine;
using System.Collections;

public class RadarBlipSub : MonoBehaviour {

	public UISprite blipSprite;

	public float FadeSpeed = 200.0f;

	public bool fades = true;

	float alpha = 1.0f;
	bool spriteSetThisCycle = false;

	public bool FinishedFading { get { return fades && alpha <= 0.01f; }}

	// Use this for initialization
	void Start () {
		fades = true;
		alpha = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (fades)
			alpha -= (FadeSpeed /255.0f) * Time.deltaTime;
		else
			alpha = 1.0f;

		blipSprite.alpha = alpha;
	}

	public void SetSpriteName(string spriteName)
	{
		if (spriteSetThisCycle)
			return;

		spriteSetThisCycle = true;
		blipSprite.spriteName = spriteName;
		alpha = 1.0f;
	}

	public void ResetCycle()
	{
		spriteSetThisCycle = false;
	}

	public void MakeInvisible()
	{
		blipSprite.alpha = 0.0f;
	}
}
