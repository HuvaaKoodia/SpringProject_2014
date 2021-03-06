﻿using UnityEngine;
using System.Collections;

public class SharedSystemsMain : MonoBehaviour {

	public string Version= "1.0";

    public GameDB GDB;
    public XMLMapLoader XMAP;
	public PrefabStore PS;
	public MapGenerator MGen;
	public ShipGenerator SGen;
    public ShipDetailGenerator SDGen;
	public GameOptionsMain GOps;
	public MusicSys MusicSystem;
	public EscHudMain EscHud;

	public UIAtlas ItemAtlas;

	public static SharedSystemsMain I;

    void Awake(){
		if (I!=null) return;
		I=this;

        XMAP.LoadData();
		XmlDatabase.LoadData(ItemAtlas);

		GDB.CheckForGameOptions();
    }
}
