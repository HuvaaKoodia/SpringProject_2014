using UnityEngine;
using System.Collections;

public class SharedSystemsMain : MonoBehaviour {

    public GameDB GDB;
    public XMLMapLoader XMAP;
	public PrefabStore PS;
	public MapGenerator MGen;
	public ShipGenerator SGen;
    public ShipDetailGenerator SDGen;
	public GameOptionsMain GOps;
	public MusicSys MusicSystem;

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
