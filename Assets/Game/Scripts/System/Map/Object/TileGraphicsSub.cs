using UnityEngine;
using System.Collections;

public class TileGraphicsSub : MonoBehaviour {

	public GameObject GraphicsObject;

	public TileLightsSub TileLights;

	public void DisableLightGraphics(){
		if (TileLights!=null&&TileLights.WhiteLightGraphics!=null){
			TileLights.WhiteLightGraphics.SetActive(false);
		}
	}
}
