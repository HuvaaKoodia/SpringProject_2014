using UnityEngine;
using System.Collections;

public class SharedSystemsMain : MonoBehaviour {

    public GameDB GDB;
    public XMLMapLoader XMAP;
	public PrefabStore PS;
	public MapGenerator MGen;
	public ShipGenerator SGen;
    public ShipDetailGenerator SDGen;

	public UIAtlas ItemAtlas;

    void Awake(){
        XMAP.LoadData();
		XmlDatabase.LoadData(ItemAtlas);
        GDB.StartNewGame();
    }
}
