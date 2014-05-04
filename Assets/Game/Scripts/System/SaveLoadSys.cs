using UnityEngine;
using System.Collections;
using Polenter.Serialization;
using System.IO;
/// <summary>
/// Handles the game serialization
/// </summary>
public class SaveLoadSys:TXT_Loader {

#if !UNITY_WEBPLAYER
	public static void SaveGame(string filename,GameObjData data){
		var serializer=new SharpSerializer(true);
		var mstream=new MemoryStream();
		
		serializer.Serialize(data,mstream);
		
		WriteDocument("Saves",filename,".sav",StreamToString(mstream));
		Debug.Log("Game saved.");
	}

	public static GameObjData LoadGame(string filename)
	{
		var data=ReadDocument("Saves",filename,".sav");
		
		var serializer=new SharpSerializer(true);
		var mstream=StreamFromString(data);
		
		var obj=serializer.Deserialize(mstream) as GameObjData;
		Debug.Log("Game loaded.");
		return obj;
	}

#endif
	//subs
	static string StreamToString (MemoryStream mstream)
	{
		return System.Convert.ToBase64String(mstream.GetBuffer());
	}

	static MemoryStream StreamFromString(string data){
		return new MemoryStream(System.Convert.FromBase64String(data));
	}


}
