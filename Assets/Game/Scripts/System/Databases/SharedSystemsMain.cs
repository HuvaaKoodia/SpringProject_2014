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

    void Awake(){
		if (loaded) return;
		loaded=true;

        XMAP.LoadData();
		XmlDatabase.LoadData(ItemAtlas);
        GDB.StartNewGame();
    }
}
