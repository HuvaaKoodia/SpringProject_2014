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

	public UIAtlas ItemAtlas;

	bool loaded=false;

	public static SharedSystemsMain I;

    void Awake(){
		if (loaded) return;
		loaded=true;
		I=this;

        XMAP.LoadData();
		XmlDatabase.LoadData(ItemAtlas);

		GDB.CheckForGameoptions();
    }
}
