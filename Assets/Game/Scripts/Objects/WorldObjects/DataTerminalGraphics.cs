using UnityEngine;
using System.Collections;

public class DataTerminalGraphics:MonoBehaviour
{
	public GameObject[] Screens;

	public void SetType (TileObjData.Obj type){
		for(int i=0;i<Screens.Length;++i){
			var screen=Screens[i];
			screen.SetActive(i==DataTerminalMain.TypeToIndex(type));
		}
	}
}


