using UnityEngine;
using System.Collections;

public class ScreenInputSub : MonoBehaviour {

	public TimanttiPeli screen;

	void OnPress (UICamera.InputEvent isPressed)
	{
		if (isPressed.isPressed) screen.Click(isPressed.Hit);
	}
}
