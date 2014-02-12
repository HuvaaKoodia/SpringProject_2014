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
            GetFreeTilesOfType(GC, TileObjData.Type.Floor, free_tiles);
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
            GetFreeTilesOfType(GC, TileObjData.Type.Floor, free_tiles);
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

        //add remaining enemies to corridors
        if (floor_amount_loot > 0)
        {
            GetFreeTilesOfType(GC, TileObjData.Type.Floor, free_tiles);
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

        //add remaining loot to random rooms
        if (floor_amount_enemies > 0)
        {
            GetFreeTilesOfType(GC, TileObjData.Type.Corridor, free_tiles);
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

    void GetFreeTilesOfType(GameController GC, TileObjData.Type type, List<TileObjData> free_tiles)
    {
        free_tiles.Clear();

        for (int x = 0; x < GC.TileObjectMap.GetLength(0); x++)
        {
            for (int y = 0; y < GC.TileObjectMap.GetLength(1); y++)
            {
                var tile = GC.TileObjectMap [x, y];
                if (tile.TileType == type && tile.ObjType == TileObjData.Obj.None)
                {
                    free_tiles.Add(tile);
                }
            }
        }
    }

    public void GenerateMissionObjectives(GameController GC,MissionObjData mission,ShipObjData ship){
        if (ContainsObjective(mission,MissionObjData.Objective.FindItem)){
            //generate item somewhere in ship
            List<CellData> LegitRooms=new List<CellData>();
            foreach(var r in ship.FloorRooms[0]){
                if (r.roomStats.type==mission.XmlData.ObjectiveRoom) LegitRooms.Add(r);
            }
            if (LegitRooms.Count==0){
                Debug.LogError("Mission: "+mission.MissionType+" failed to create objective item in ship type "+ship.Name);
            } 
            var room=Subs.GetRandom(LegitRooms);

            //DEV.HAX.WEIRD
            foreach(var l in GC.LootCrates){

            }

        }
    }

    bool ContainsObjective(MissionObjData m,MissionObjData.Objective o){
        return m.PrimaryObjectives.Contains(o)||m.PrimaryObjectives.Contains(o);
    }
}

