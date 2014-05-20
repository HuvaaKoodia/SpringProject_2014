using UnityEngine;
using System.Collections;
using Polenter.Serialization;
using System.IO;
/// <summary>
/// Handles the game serialization
/// </summary>
public class SaveLoadSys:TXT_Loader {

#if !UNITY_WEBPLAYER
	public static void SaveGame(string savename,GameObjData obj){
		var data=SaveGameDataToString(obj);
		WriteDocument("Saves",savename,".sav",data);
		Debug.Log("GameSaved");
	}

	public static GameObjData LoadGame(string savename)
	{
		var data=ReadDocument("Saves",savename,".sav");
		return LoadGameDataFromString(data);
	}
#endif

	public static void SaveGamePlayerPrefs(string savename,GameObjData obj){
		var data=SaveGameDataToString(obj);
		PlayerPrefs.SetString(savename,data);
	}
	
	public static GameObjData LoadGamePlayerPrefs(string savename)
	{
		var data=PlayerPrefs.GetString(savename);
		return LoadGameDataFromString(data);
	}

	static GameObjData LoadGameDataFromString(string data)
	{
		var serializer=new SharpSerializer(true);
		var mstream=StreamFromString(data);
		
		GameObjData obj=null;
		try{
			obj=serializer.Deserialize(mstream) as GameObjData;
		}
		catch{
			Debug.LogWarning("Game load failed");
			return null;
		}
		return obj;
	}

	static string SaveGameDataToString(GameObjData data){
		var serializer=new SharpSerializer(true);
		var mstream=new MemoryStream();
		
		serializer.Serialize(data,mstream);
		return StreamToString(mstream);
	}

	//subs
	static string StreamToString (MemoryStream mstream)
	{
		return System.Convert.ToBase64String(mstream.GetBuffer());
	}

	static MemoryStream StreamFromString(string data){
		return new MemoryStream(System.Convert.FromBase64String(data));
	}


}
