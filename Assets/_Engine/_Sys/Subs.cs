using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Subs{

	#region Random Subs
	
	/// <summary>
	/// Random int from 0 to max (ex).
	/// </summary>
	public static int GetRandom(int max){
		return Random.Range(0,max);
	}
	
	/// <summary>
	/// Random float from 0 to max (ex).
	/// </summary>
	public static float GetRandom(float max){
		return Random.Range(0f,max);
	}
	
	/// <summary>
	/// Random float from 0f to 1f(ex).
	/// </summary>
	public static float RandomFloat(){
		return Random.Range(0f,1f);
	}
	/// <summary>
	/// Random int from 0 to 100(ex)
	/// </summary>
	public static float RandomPercent(){
		return Random.Range(0,100);
	}
	
	public static bool RandomBool(){
		if (RandomPercent()<50)
			return true;
		return false;
	}
	/// <summary>
	/// Random vector3. All values from -1f to 1f.
	/// </summary>

	public static Vector3 RandomVector3(){
		return new Vector3(Random.Range(-1f,1),Random.Range(-1f,1),Random.Range(-1f,1));
	}
	
	public static Color RandomColor(){
		return new Color(Random.Range(0,1f),Random.Range(0,1f),Random.Range(0,1f));
	}
	public static Color RandomColor(float alpha){
		return new Color(Random.Range(0,1f),Random.Range(0,1f),Random.Range(0,1f),alpha);
	}
	
	#endregion

	public static void SendMessageRecursive(Transform tform,string message){
		
		foreach(Transform t in tform){
			t.SendMessage(message,SendMessageOptions.DontRequireReceiver);
			SendMessageRecursive(t,message);
		}
	}

	public static Vector3 LengthDir(Transform transform,Vector3 dir)
	{
		return transform.position+transform.TransformDirection(dir);
	}
	
	/// <summary>
	/// Wraps the specified number according to min and max.
	/// Max exclusive.
	/// </summary>
	public static int Wrap(int number, int min, int max)
	{
		var b=number%(max-min);
		if (b>=0)
			return min+b;
		return max+b;
	}
	
	/// <summary>
	/// Adds and wraps the specified number according to min and max.
	/// Max exclusive.
	/// </summary>
	public static int Add(int number, int min, int max)
	{
		return Add(number,1,min,max);
	}
	
	/// <summary>
	/// Adds and wraps the specified number according to min and max.
	/// Max exclusive.
	/// </summary>
	public static int Add(int number,int amount, int min, int max)
	{
		return Wrap (number+amount,min,max);
	}

	public static Vector3 ClampVector3 (Vector3 vector, Vector3 max)
	{
		var v=vector;
		if (v.x>max.x)
			v.x=max.x;
		if (v.y>max.y)
			v.y=max.y;
		if (v.z>max.z)
			v.z=max.z;
		return v;
	}

	public static Vector3 Vector3Multi (Vector3 v1, Vector3 v2)
	{
		return new Vector3(v1.x*v2.x,v1.y*v2.y,v1.z*v2.z);
	}

	//area
    public static bool insideArea(Vector2 Position, Rect area)
    {
        return (Position.x >= area.x && Position.x < area.x + area.width && Position.y >= area.y && Position.y < area.y + area.height);
    }

    public static bool outsideArea(Vector2 Position, Rect area)
    {
        return !insideArea(Position,area);
    }
	
	public static void ChangeColor(Transform t, Color color){
		if (t.renderer!=null){
			t.renderer.material.color=color;
		}
		foreach (Transform tr in t){
			ChangeColor(tr,color);
		}
	}
	
	public static string ColorToHex(Color32 color)
	{
		return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
		
	}
	
	/// <summary>
    /// Autofits the text to fit in an area with the preferred width.
    /// </summary>
    /// <param name="width"></param>
    public static string autofit_text(string text,int width,UIFont font)
    {
        text=text.Replace("\n", " \n ").Replace("\\n", " \n ");
		
        float tw = 0;
        string all_words="",word="";
        string[] words = text.Split(new char[1]{' '});
        for (int i = 0; i < words.Length; i++)
        {
            word = words[i].Replace(" ", "");
            if (word == "") continue;

            if (word == "\n")
            {
                all_words += "\n";
                tw = 0;
                continue;
            }
            word+=" ";
            var measure=font.CalculatePrintedSize(word).x;
            tw += measure;
            if (tw >= width)
            {
                all_words += "\n";
                tw = measure;
            }
            all_words += word;
        }
		return all_words;
    }

	public static string[] Split(string str,string separator){
		return str.Split(new string[]{separator},System.StringSplitOptions.RemoveEmptyEntries);
	}

	//templates
	
	public static IEnumerable<T> EnumValues<T>() {
		return System.Enum.GetValues(typeof(T)).Cast<T>();
	}
	
	public static T GetRandom<T>(IEnumerable<T> enumerable){
		return enumerable.ElementAt(Random.Range(0,enumerable.Count()));
	}

	/// <summary>
	/// Get an object under mouse position in 3D space.
	/// </summary>
	/// <returns><c>true</c>, if object was under mouse position <c>false</c> otherwise.</returns>
	public static bool GetObjectMousePos(out Component obj,float distance,string layer){
		var ray=Camera.main.ScreenPointToRay(Input.mousePosition);
		int mask=1<<LayerMask.NameToLayer(layer);
		RaycastHit info;

		if (Physics.Raycast(ray,out info,distance,mask)){

			obj= info.collider.gameObject.GetComponent<Component>();
			return true;
		}
		obj=null;
		return false;
	}
}
