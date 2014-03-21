using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//enumerator for the environment lights in TilePrefabs under TestObjects in GameScene
public enum Environment_Light
{
	WhiteLight = 0,
	OrangeLight,
	AllLight
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
	}

	//function to enable the environment lights in TilePrefabs under TestObjects in GameScene 
	//depending on boolean passed and Lights selected
	//switch case handles the differernt environment lights in TilePrefabs under TestObjects in GameScene
	//for all lights of the type selected in the TilePrefab
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
			//as long as there is a orange_lights
			if(orange_lights.Count > 0)
			{
				//traverse through the list of orange_lights
				for(int i = 0; i < orange_lights.Count; i++)
				{
					//as long as there is a orange_lights, set value of enabled to value of on
					if(orange_lights[i] != null)
					{
						orange_lights[i].light.enabled = on;
						//Debug.Log("ORANGE_LIGHTS[" + i + "] ON: " + orange_lights[i].light.enabled);			//debug to display value of orange_lights enabled
					}
				}
			}
			break;
		case Environment_Light.AllLight:
			EnableLights(on, Environment_Light.WhiteLight);
			EnableLights(on, Environment_Light.OrangeLight);
			break;
		default:
			Debug.Log("PASS IN WhiteLight, OrangeLight OR AllLight");
			Debug.Break();
			break;
		}
	}

	//function to enable the environment lights in TilePrefabs under TestObjects in GameScene 
	//depending on boolean passed and Lights selected
	//switch case handles the differernt environment lights in TilePrefabs under TestObjects in GameScene
	//for specific lights of the type selected in the TilePrefab
	public void EnableLights(bool on, Environment_Light EL, int index)
	{
		int size = 0;
		switch(EL)
		{
		case Environment_Light.WhiteLight:
			size = white_lights.Count;
			
			//as long as there is a orange_lights
			if(size > 0 && index >= 0 && index < size)
			{				
				//as long as there is a white_lights, set value of enabled to value of on
				if(white_lights[index] != null)
				{
					white_lights[index].light.enabled = on;
					//Debug.Log("WHITE_LIGHTS[" + i + "] ON: " + white_lights[i].light.enabled);			//debug to display value of white_lights enabled
				}
				else
				{
					Debug.Log("INDEX IS NOT RELEVANT TO WHITE_LIGHTS LIST");
					Debug.Break();
				}
			}
			break;
		case Environment_Light.OrangeLight:
			size = orange_lights.Count;

			//as long as there is a orange_lights
			if(size > 0 && index >= 0 && index < size)
			{
				//as long as there is a orange_lights, set value of enabled to value of on
				if(orange_lights[index] != null)
				{
					orange_lights[index].light.enabled = on;
					//Debug.Log("ORANGE_LIGHTS[" + i + "] ON: " + orange_lights[i].light.enabled);			//debug to display value of orange_lights enabled
				}
				else
				{
					Debug.Log("INDEX IS NOT RELEVANT TO ORANGE_LIGHTS LIST");
					Debug.Break();
				}
			}
			break;
		default:
			Debug.Log("PASS IN WhiteLight OR OrangeLight ONLY");
			Debug.Break();
			break;
		}
	}

	//function to enable Lights selected to flicker based on a delay set in the inspector in GameScene
	//for all lights of the type selected in the TilePrefab
	public void Flicker(float delay, bool on, Environment_Light EL)
	{
		//Debug.Log("TICKS: " + (int)ticks);																		//display value of ticks

		//toggling of enabled value of the environment lights in TilePrefabs under TestObjects in GameScene
		//depending on boolean passed and Lights selected
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
		}
	}

	//function to enable Lights selected to flicker based on a delay set in the inspector in GameScene
	//for specific lights of the type selected in the TilePrefab
	public void Flicker(float delay, bool on, Environment_Light EL, int index)
	{
		//Debug.Log("TICKS: " + (int)ticks);																		//display value of ticks
		
		//toggling of enabled value of the environment lights in TilePrefabs under TestObjects in GameScene
		//depending on boolean passed and Lights selected
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
			EnableLights(on, EL, index);
		}
		//increment ticks every second
		else
		{
			ticks += Time.deltaTime;
		}
	}

	//function to set value of light_flicker through code instead of in inspector
	//for all lights of the type selected in the TilePrefab
	public void SetFlicker(bool flicker)
	{
		light_flicker = flicker;
	}

	//function to get the value of light_flicker be it set through code or in inspector
	//for all lights of the type selected in the TilePrefab
	public bool GetFlicker()
	{
		return light_flicker;
	}

	//function to set value of delay through code instead of in inspector
	//for all lights of the type selected in the TilePrefab
	public void SetDelay(float newdelay)
	{
		delay = newdelay;
	}

	//function to get the value of delay be it set through code or in inspector
	//for all lights of the type selected in the TilePrefab
	public float GetDelay()
	{
		return delay;
	}

	//function to get the enabled state of a specific light of a specific types
	public bool GetEnableLightsState(Environment_Light EL, int index)
	{
		int size = 0;
		switch(EL)
		{
		case Environment_Light.WhiteLight:
			size = white_lights.Count;
			
			//as long as there is a orange_lights
			if(size > 0 && index >= 0 && index < size)
			{
				return white_lights[index].enabled;
			}
			else
			{
				Debug.Log("INDEX IS NOT RELEVANT TO WHITE_LIGHTS LIST");
				return false;
			}
		case Environment_Light.OrangeLight:
			size = orange_lights.Count;
			
			//as long as there is a orange_lights
			if(size > 0 && index >= 0 && index < size)
			{
				return orange_lights[index].enabled;
			}
			else
			{
				Debug.Log("INDEX IS NOT RELEVANT TO ORANGE_LIGHTS LIST");
				return false;
			}
		default:
			Debug.Log("PASS IN WhiteLight OR OrangeLight ONLY");
			return false;
		}
	}
}