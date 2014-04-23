using UnityEngine;
using System.Collections;

public class ScreenInputSub : MonoBehaviour {

	public ComputerSystems.TimanttiPeli screen;

	void OnPress (UICamera.InputEvent isPressed)
	{
		if (isPressed.isPressed){
			if (isPressed.PressIndex==0){
				screen.Click(isPressed.Hit);
			}
			else{
				screen.ToggleContext();
			}
		}
	}
}
