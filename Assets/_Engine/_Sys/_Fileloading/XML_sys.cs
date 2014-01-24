using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System;
using System.Linq;

public class XML_sys : MonoBehaviour {

    //game logic
	public static Action OnRead,OnWrite;
	
	void Awake () {
		readXML();
	}
	
	void OnDestroy(){
		writeXML();
	}
	
	public void readXML(){
		
		if (OnRead!=null){
			OnRead();
		}
	}
	
	public void writeXML(){
		
		if (OnWrite!=null){
			OnWrite();
		}
	}
}
