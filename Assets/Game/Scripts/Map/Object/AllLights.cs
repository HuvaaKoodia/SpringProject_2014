using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//enumerator for the environment lights in TilePrefabs under TestObjects in GameScene
public enum Environment_Light
{
	WhiteLight = 0,
	OrangeLight
}

public class AllLights : MonoBehaviour
{	
	public List<Light> white_lights;																			//instantiate a list for the white lights in the various TilePrefabs under TestObjects of GameScene
	public List<Light> orange_lights;																			//instantiate a list for the orange lights in the various TilePrefabs under TestObjects of GameScene
		
	public bool light_flicker;																					//instantiate boolean to allow light to flicker

	public float delay;																							//instantiate time to delay by in inspector
	private float ticks;																						//instantiate time since last toggle

	// Use this for initialization
	void Start ()
	{
		ticks = 0.0f;																							//initialize value for ticks and set it to be 0.0f
	}
	
	// Update is called once per frame
	void Update ()
	{
		//<not needed>
		//if(light_flicker)
		//{
			//Flicker(delay, 0.0f, Subs.RandomBool(), Environment_Light.WhiteLight);
		//}
	}

	//function to enable the environment lights in TilePrefabs under TestObjects in GameScene depending on boolean passed and Lights selected
	public void EnableLights(bool on, Environment_Light EL)
	{
		switch(EL)
		{
		case Environment_Light.WhiteLight:
			//as long as there is a white_lights
			if(white_lights.Count > 0)
			{				
				//traverse through the list of white_lights
				for(int i = 0; i < white_lights.Count; i++)
				{
					//as long as there is a white_lights, set value of enabled to value of on
					if(white_lights[i] != null)
					{
						white_lights[i].light.enabled = on;
						//Debug.Log("WHITE_LIGHTS[" + i + "] ON: " + white_lights[i].light.enabled);			//debug to display value of white_lights enabled
					}
				}
			}
			break;
		case Environment_Light.OrangeLight:
			//as long as there is a white_lights
			if(orange_lights.Count > 0)
			{
				//traverse through the list of white_lights
				for(int i = 0; i < orange_lights.Count; i++)
				{
					//as long as there is a white_lights, set value of enabled to value of on
					if(orange_lights[i] != null)
					{
						orange_lights[i].light.enabled = on;
						//Debug.Log("ORANGE_LIGHTS[" + i + "] ON: " + orange_lights[i].light.enabled);			//debug to display value of white_lights enabled
					}
				}
			}
			break;
		}
	}

	//function to enable Lights selected to flicker based on a delay set in the inspector in GameScene
	public void Flicker(float delay, bool on, Environment_Light EL)
	{/*
		//toggling of enabled value of the environment lights in TilePrefabs under TestObjects in GameScene depending on boolean passed and Lights selected
		if(ticks >= delay)
		{
			if(on)
			{
				on = false;
			}
			else
			{
				on = true;
			}

			ticks = 0.0f;
			EnableLights(on, EL);
		}
		//increment ticks every second
		else
		{
			ticks += Time.deltaTime;
			//Debug.Log("TICKS: " + ticks);																		//display value of ticks
		}
		*/
	}
}

