using UnityEngine;
using System.Collections;
using System.IO;

public class TXT_Loader{

#if !UNITY_WEBPLAYER
	/// <summary>
	/// Reads a txt Documents from a path.
	/// From disc if PC. From resources if Android or Webplayer
	/// </summary>
	public static string ReadDocument(string path,string file,string file_extension){
		path=path+"/"+file;

		return File.ReadAllText(path+file_extension);

	}

	/// <summary>
	/// Writes a txt Documents to a path.
	/// </summary>
	public static void WriteDocument(string path,string file,string file_extension,string contents){
		XML_Loader.checkFolder(path);
		path=path+"/"+file;
		File.WriteAllText(path+file_extension,contents);
	}
#endif
}
