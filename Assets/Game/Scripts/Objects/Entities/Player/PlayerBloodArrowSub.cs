using UnityEngine;
using System.Collections;

public class PlayerBloodArrowSub : MonoBehaviour {

	UISprite bloodArrow;
	bool visible;

	public float timeBeforeFade = 0.4f;
	public float fadeSpeed = 1.5f;

	float timer;

	void Awake()
	{
		bloodArrow = transform.GetComponent<UISprite>();
		visible = false;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (!visible) return;

		timer += Time.deltaTime;

		if (timer > timeBeforeFade)
		{
			bloodArrow.alpha -= fadeSpeed * Time.deltaTime;

			if (bloodArrow.alpha <= 0.01f)
			{
				bloodArrow.enabled = false;
				timer = 0.0f;
				visible = false;
			}
		}
	}

	public void ShowArrow()
	{
		visible = true;

		timer = 0.0f;

		bloodArrow.enabled = true;
		bloodArrow.alpha = 1.0f;
	}
}
