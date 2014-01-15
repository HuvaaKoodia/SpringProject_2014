using System;
using UnityEngine;
 
public class Drawing
{ 
    public static Texture2D lineTex;
 
    public static void DrawLine3D(Camera cam, Vector3 pointA, Vector3 pointB) { DrawLine3D(cam,pointA,pointB, GUI.contentColor, 1.0f); }
    public static void DrawLine3D(Camera cam, Vector3 pointA, Vector3 pointB, Color color) { DrawLine3D(cam,pointA,pointB, color, 1.0f); }
    public static void DrawLine3D(Camera cam, Vector3 pointA, Vector3 pointB, float width) { DrawLine3D(cam,pointA,pointB, GUI.contentColor, width); }
    public static void DrawLine3D(Camera cam, Vector3 pointA, Vector3 pointB, Color color, float width)
	{	
		
		
		Vector3 wp=cam.WorldToScreenPoint(pointA);
		Vector2 p1 =new Vector2(wp.x,Screen.height-wp.y);
		
		wp=cam.WorldToScreenPoint(pointB);
		Vector2 p2 =new Vector2(wp.x,Screen.height-wp.y);
		
	   	DrawLine(p1,p2,color,width);
		
		Debug.Log("Point A: "+p1+", point B "+p2);
    }
 
    public static void DrawLine(Rect rect) { DrawLine(rect, GUI.contentColor, 1.0f); }
    public static void DrawLine(Rect rect, Color color) { DrawLine(rect, color, 1.0f); }
    public static void DrawLine(Rect rect, float width) { DrawLine(rect, GUI.contentColor, width); }
    public static void DrawLine(Rect rect, Color color, float width) { DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y + rect.height), color, width); }
    public static void DrawLine(Vector2 pointA, Vector2 pointB) { DrawLine(pointA, pointB, GUI.contentColor, 1.0f); }
    public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color) { DrawLine(pointA, pointB, color, 1.0f); }
    public static void DrawLine(Vector2 pointA, Vector2 pointB, float width) { DrawLine(pointA, pointB, GUI.contentColor, width); }
    public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
    {
		pointA.x = (int)pointA.x; pointA.y = (int)pointA.y;
		pointB.x = (int)pointB.x; pointB.y = (int)pointB.y;
		
		if (!lineTex) { lineTex = new Texture2D(1, 1); }
		Color savedColor = GUI.color;
		GUI.color = color;
		
		Matrix4x4 matrixBackup = GUI.matrix;
		
		float angle = Mathf.Atan2(pointB.y-pointA.y, pointB.x-pointA.x)*180f/Mathf.PI;
		float length = (pointA-pointB).magnitude;
		GUIUtility.RotateAroundPivot(angle, pointA);
		GUI.DrawTexture(new Rect(pointA.x, pointA.y, length, width), lineTex);
		
		GUI.matrix = matrixBackup;
		GUI.color = savedColor;
    }
}
