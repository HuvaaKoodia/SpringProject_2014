using UnityEngine;
using System.Collections;

public class ElevatorScreenSub : MonoBehaviour {

	public GameObject ScreenUp, ScreenDown;

	public void Init(int floor)
	{
		if (floor == 0)
		{
			ScreenUp.SetActive(true);
			ScreenDown.SetActive(false);
		}
		else
		{
			ScreenUp.SetActive(false);
			ScreenDown.SetActive(true);
		}
	}
}
