using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipDetailGenerator : MonoBehaviour
{
    public PrefabStore MapPrefabs;

    /// <summary>
    /// Generates loot to the loot containers.
    /// Call after MapGen.GenerateSceneMap.
    /// DEV.Develop further
    /// </summary>
    public void GenerateLoot(GameController GC, ShipObjData ship)
    {
        foreach (var c in GC.LootCrates)
        {
            int a = Subs.GetRandom(1, 5);
            for (int i=0; i<a; i++)
            {
                c.Items.Add(InvGameItem.GetRandomItem(GC.SS.XDB));
            }
        }
    }
    
    /// <summary>
    /// Generates items (loot & enemies) to the room and corridors of the ship to the TileObjectMap
    /// Call after GenerateObjectDataMap and before GenerateSceneMap
    /// </summary>

    public void GenerateShipItems(GameController GC, ShipObjData ship)
    {
        int current_floor = 0;
        var xml_md = ship.XmlData.Floors [current_floor];

        int floor_amount_enemies = Subs.GetRandom(xml_md.EnemyAmountMin, xml_md.EnemyAmountMax);
        int floor_amount_loot = Subs.GetRandom(xml_md.LootAmountMin, xml_md.LootAmountMax);

        //rooms
        List<TileObjData> free_tiles = new List<TileObjData>();
        var rooms_list = ship.FloorRooms [current_floor];
        foreach (var room in rooms_list)
        {
            //loot crates
            GetFreeTilesOfType(GC,room, TileObjData.Type.Floor, free_tiles);
            int l_amount = Subs.GetRandom(room.RoomXmlData.LootAmountMin, room.RoomXmlData.LootAmountMax);
            
            while (free_tiles.Count>0)
            {
                if (floor_amount_loot == 0 || l_amount == 0)
                    break;
                
                var tile = Subs.GetRandom(free_tiles);
                free_tiles.Remove(tile);
                
                l_amount--;
                floor_amount_loot--;
                
                tile.SetObj(TileObjData.Obj.Loot);
            }

            //enemies
            GetFreeTilesOfType(GC,room, TileObjData.Type.Floor, free_tiles);
            int e_amount = Subs.GetRandom(room.RoomXmlData.EnemyAmountMin, room.RoomXmlData.EnemyAmountMax);

            while (free_tiles.Count>0)
            {
                if (e_amount == 0 || floor_amount_enemies == 0)
                    break;

                var tile = Subs.GetRandom(free_tiles);
                free_tiles.Remove(tile);

                e_amount--;
                floor_amount_enemies--;

                tile.SetObj(TileObjData.Obj.Enemy);
            }
        }


        //add remaining loot to random floors on the map (floor tiles outside rooms included)
        if (floor_amount_loot > 0)
        {
            GetFreeTilesOfType(GC,null, TileObjData.Type.Floor, free_tiles);
            while (free_tiles.Count>0)
            {
                if (floor_amount_loot == 0)
                    break;
                
                var tile = Subs.GetRandom(free_tiles);
                free_tiles.Remove(tile);

                floor_amount_loot--;
                
                tile.SetObj(TileObjData.Obj.Loot);
            }
        }

        //add remaining enemies to corridors
        if (floor_amount_enemies > 0)
        {
            GetFreeTilesOfType(GC,null, TileObjData.Type.Corridor, free_tiles);
            while (free_tiles.Count>0)
            {
                
                if (floor_amount_enemies == 0)
                    break;
                
                var tile = Subs.GetRandom(free_tiles);
                free_tiles.Remove(tile);
                
                floor_amount_enemies--;
                
                tile.SetObj(TileObjData.Obj.Enemy);
            }
        }
    }

    /// <summary>
    /// Returns all tiles of a certain type inside a room which don't have an object yet.
    /// If room is null the whole map is scanned.
    /// </summary>
    void GetFreeTilesOfType(GameController GC,CellData room, TileObjData.Type type, List<TileObjData> free_tiles)
    {
        free_tiles.Clear();

        int rx=0,ry=0,rw=GC.TileObjectMap.GetLength(0),rh=GC.TileObjectMap.GetLength(1);

        if (room!=null){
            rx=room.X+room.XOFF;ry=room.Y+room.YOFF;
            rw=room.W;rh=room.H;
        }

        for (int x = 0; x < rw; x++)
        {
            for (int y = 0; y < rh; y++)
            {
                var tile = GC.TileObjectMap [rx+x, ry+y];
                if (tile.TileType == type && tile.ObjType == TileObjData.Obj.None)
                {
                    free_tiles.Add(tile);
                }
            }
        }
    }

    public void GenerateMissionObjectives(GameController GC,MissionObjData mission,ShipObjData ship,XmlDatabase XDB){
        string obj_room=mission.XmlData.ObjectiveRoom;

        if (ContainsObjective(mission,MissionObjData.Objective.FindItem)){
            //generate item somewhere in ship
            List<CellData> LegitRooms=new List<CellData>();
            foreach(var r in ship.FloorRooms[0]){
                if (r.roomStats.type==obj_room) LegitRooms.Add(r);
            }
            if (LegitRooms.Count==0){
                Debug.LogError("Mission: "+mission.MissionType+" failed to create objective item in ship type "+ship.Name +" required room : "+obj_room);
                return;
            }

            var room=Subs.GetRandom(LegitRooms);
            int rx=room.X+room.XOFF,ry=room.Y+room.YOFF,rw=room.W,rh=room.H;

            List<LootCrateMain> loot=new List<LootCrateMain>();

            for (int x = 0; x < rw; x++)
            {
                for (int y = 0; y < rh; y++)
                {
                    var tile = GC.TileMainMap [rx+x,ry+y];
                    if (tile.Data.ObjType == TileObjData.Obj.Loot)
                    {
                        loot.Add(tile.TileObject.GetComponent<LootCrateMain>());
                    }
                }
            }

            if (loot.Count==0){
                Debug.LogWarning("Cannot generate quest item. No loot crates in room "+obj_room+ " ship type "+ship.Name);
                return;
            }

            var l=Subs.GetRandom(loot);
            var item=new InvGameItem(XDB.QuestItems["Blackbox"]);
            l.Items.Add(item);
        }
    }

    bool ContainsObjective(MissionObjData m,MissionObjData.Objective o){
        return m.PrimaryObjectives.Contains(o)||m.PrimaryObjectives.Contains(o);
    }
}

