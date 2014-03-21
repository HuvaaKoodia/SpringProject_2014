using UnityEngine;
using System.Collections;

public class MaterialDepthChange : MonoBehaviour {
	
	public bool RealtimeUpdate=false;
	public int QueueDepth=-1;
	public int RelativeQueueDepth=0;
	
	void Start () {
		if (QueueDepth<0)
			QueueDepth=renderer.material.renderQueue;
		UpdateDepth();
	}
	
	void Update(){
		if (RealtimeUpdate){
			UpdateDepth();
		}
	}
	
	public void UpdateDepth(){
		renderer.material.renderQueue= QueueDepth+RelativeQueueDepth;
	}

}
