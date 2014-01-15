using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(XML_sys))]
public class XMLsysEditor : Editor{

	public override void OnInspectorGUI(){
		DrawDefaultInspector();
		
		EditorGUILayout.Space();
		
		if (GUILayout.Button ("Read XML")){
			var xml=(XML_sys)target;
			xml.readXML();
		}
		
		EditorGUILayout.Space();
		
		if (GUILayout.Button ("Write XML")){
			var xml=(XML_sys)target;
			xml.writeXML();
		}
		
		
	}
}
