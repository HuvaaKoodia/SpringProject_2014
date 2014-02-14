using UnityEngine;
using System.Collections;

public class SharedSystemsMain : MonoBehaviour {

    public GameDB GDB;
    public XMLMapLoader XMAP;
    public XmlDatabase XDB;
	public PrefabStore PS;
	public MapGenerator MGen;
	public ShipGenerator SGen;
    public ShipDetailGenerator SDGen;

    void Awake(){
        XMAP.LoadData();
        XDB.LoadData();
        GDB.StartNewGame();
    }
}
