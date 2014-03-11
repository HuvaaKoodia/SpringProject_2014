using UnityEngine;
using System.Collections;

public class LevelPlaylist : MonoBehaviour {
	
	public bool DEBUG_KEYS=false;
	
	int current_scene,current_data;
	public string[] Scene_list,Data_list;
	
	public int CurrentScene{
		get {return current_scene;}
	}
	
	public int CurrentData{
		get {return current_data;}
	}
	
	public int CurrentRound{
		get;private set;
	}
	
	void Start(){
		CurrentRound=1;
		current_scene=0;
		current_data=0;
	}
	
	bool temp=true;
	
	void Update(){
		if (DEBUG_KEYS){
			if (Input.GetKeyDown(KeyCode.N)){
				if (temp){
					LoadNextLevel();
					temp=false;
				}
			}
			if (Input.GetKeyUp(KeyCode.N)){
				temp=true;
			}
		}
	}
	
		
	public string getCurrentSceneName()
	{
		return Scene_list[current_scene];
	}
	
	public string getCurrentDataName ()
	{
		return Data_list[current_data];
	}
	
	/// <summary>
	/// Returns the next scene name and forwards the current map index.
	/// </returns>
	string getNextScene(){
		var s=Scene_list[current_scene];
		current_scene=Subs.Add(current_scene,0,Scene_list.Length);
		return s;
	}
	/// <summary>
	/// Returns the next scene name and forwards the current map index.
	/// </returns>
	string getNextData(){
		var s=Data_list[current_data];
		current_data=Subs.Add(current_data,0,Data_list.Length);
		return s;
	}
	
	//LOADING
	
	/// <summary>
	///Goes to the next scene.
	/// </returns>
	public void LoadNextScene(){
		CurrentRound++;
		Application.LoadLevel(getNextScene());
	}
	
	/// <summary>
	///Reloads the current scene
	/// </returns>
	public void LoadCurrentScene(){
		Application.LoadLevel(getCurrentSceneName());
	}
	
	/// <summary>
	///Additively loads the next data scene
	/// </returns>
	public void LoadNextDataAdditive(){
		Application.LoadLevelAdditive(getNextData());
	}
	
	/// <summary>
	/// Loads the next level.
	/// (Scene + additive data)
	/// </summary>
	
	public void LoadNextLevel(){
		
		Debug.Log("Loaded Scene: "+getCurrentSceneName()+" with Data: "+getCurrentDataName());
		
		LoadNextScene();
		LoadNextDataAdditive();
	}
}
