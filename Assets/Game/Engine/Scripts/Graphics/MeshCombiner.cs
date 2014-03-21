using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshCombiner : MonoBehaviour {

	public void Combine(GameController GC,int floor_index) {
		var meshFilters = new List<MeshFilter>();

		var f=GC.Floors[floor_index];

		foreach (var t in f.TileMainMap){
			if (t.TileGraphics!=null){
				foreach (var m in t.TileGraphics.GraphicsObject.GetComponentsInChildren<MeshFilter>()){
					meshFilters.Add(m);
				}
			}
		}

		CombineInstance[] combine = new CombineInstance[meshFilters.Count];
		int i = 0;
		while (i < meshFilters.Count) {
			combine[i].mesh = meshFilters[i].sharedMesh;
			combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
			//meshFilters[i].gameObject.active = false;
			i++;
		}
		transform.GetComponent<MeshFilter>().mesh = new Mesh();
		transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
		transform.gameObject.SetActive(true);
	}
}
