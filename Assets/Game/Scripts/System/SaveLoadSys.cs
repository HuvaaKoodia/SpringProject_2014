using UnityEngine;
using System.Collections;
using Polenter.Serialization;
using System.IO;
/// <summary>
/// Handles the game serialization
/// </summary>
public class SaveLoadSys:TXT_Loader {

	const string SaveLocation="Saves",SaveExtension=".sav";

#if !UNITY_WEBPLAYER
	public static void SaveGame(string savename,GameObjData obj){
		var data=SaveGameDataToString(obj);
		WriteDocument(SaveLocation,savename,SaveExtension,data);
		Debug.Log("GameSaved");
	}

	public static GameObjData LoadGame(string savename)
	{
		var data=ReadDocument(SaveLocation,savename,SaveExtension);
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

	
	public static void SaveOptions(GameOptionsObjData obj){
		var data=SaveOptionsDataToString(obj);
		PlayerPrefs.SetString("Options",data);
	}

	public static GameOptionsObjData LoadOptions ()
	{
		var data=PlayerPrefs.GetString("Options");
		return LoadOptionsDataFromString(data);
	}

	public static void RemoveSaveFile(string savename){
		var path=GetFilePath(savename);
		if (File.Exists(path)){
			File.Delete(path);
		}
	}

	public static void RemoveSavePlayerPref(string savename){
		if (PlayerPrefs.HasKey(savename)){
			PlayerPrefs.DeleteKey(savename);
		}
	}

	public static void ClearSaves(string savename){
		RemoveSaveFile(savename);
		RemoveSavePlayerPref(savename);
	}

	//subs
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

	static GameOptionsObjData LoadOptionsDataFromString(string data)
	{
		var serializer=new SharpSerializer(true);
		var mstream=StreamFromString(data);
		
		GameOptionsObjData obj=null;
		try{
			obj=serializer.Deserialize(mstream) as GameOptionsObjData;
		}
		catch{
			Debug.LogWarning("Game options load failed");
			return null;
		}
		return obj;
	}
	
	static string SaveOptionsDataToString(GameOptionsObjData data){
		var serializer=new SharpSerializer(true);
		var mstream=new MemoryStream();
		
		serializer.Serialize(data,mstream);
		return StreamToString(mstream);
	}

	static string GetFilePath(string savename){
		return SaveLocation+"/"+savename+SaveExtension;
	}

	static string StreamToString (MemoryStream mstream)
	{
		return System.Convert.ToBase64String(mstream.GetBuffer());
	}

	static MemoryStream StreamFromString(string data){
		return new MemoryStream(System.Convert.FromBase64String(data));
	}


}
