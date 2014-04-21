using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipDetailGenerator : MonoBehaviour
{
    public PrefabStore MapPrefabs;

    /// <summary>
    /// Generates loot items to the loot containers.
    /// Call after MapGen.GenerateSceneMap.
    /// DEV.Develop further
    /// </summary>
	public void GenerateLoot(FloorObjData floor,string lootpool,string lootquality)
    {
		foreach (var c in floor.LootCrates)
        {
            int a = Subs.GetRandom(1, 5);
            for (int i=0; i<a; i++)
            {
				c.Items.Add(InvGameItem.GetRandomItem(lootpool,lootquality));
            }
        }
    }
    
    /// <summary>
    /// Generates items (loot & enemies) to the room and corridors of the ship to the TileObjectMap
    /// Call after GenerateObjectDataMap and before GenerateSceneMap
    /// </summary>

    public void GenerateShipItems(GameController GC, FloorObjData floor,ShipObjData ship)
    {
        int current_floor = floor.FloorIndex;
        var xml_md = ship.XmlData.Floors [current_floor];

        int floor_amount_enemies = Subs.GetRandom(xml_md.EnemyAmountMin, xml_md.EnemyAmountMax);
        int floor_amount_loot = Subs.GetRandom(xml_md.LootAmountMin, xml_md.LootAmountMax);

        //rooms
        List<TileObjData> free_tiles = new List<TileObjData>();
        var rooms_list = ship.FloorRooms [current_floor];
        foreach (var room in rooms_list)
        {
            //loot crates
			GetTilesOfTypeWithObject(floor,room, TileObjData.Type.Floor,TileObjData.Obj.LootArea, free_tiles);
            int l_amount = Subs.GetRandom(room.RoomXmlData.LootAmountMin, room.RoomXmlData.LootAmountMax);

            //remove free positions next to doors
            for (int t=0;t<free_tiles.Count;++t){
                var tile=free_tiles[t];

                for (int i=0;i<4;i++){
                    int x=MapGenerator.GetCardinalX(i),y=MapGenerator.GetCardinalY(i);
                    var index=floor.GetTileObj(tile.X+x,tile.Y+y);

                    if (index.TileType==TileObjData.Type.Door){
                        free_tiles.Remove(tile);
                        --t;
						break;
                    }
                }
            }

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
			GetFreeTilesOfType(floor,room, TileObjData.Type.Floor, free_tiles);
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


        //add remaining loot to random lootareas on the map (floor tiles outside rooms included)
        /*if (floor_amount_loot > 0)
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
        }*/

        //add remaining enemies to corridors
        if (floor_amount_enemies > 0)
        {
			GetFreeTilesOfType(floor,null, TileObjData.Type.Corridor, free_tiles);
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
	void GetFreeTilesOfType(FloorObjData floor,ShipRoomObjData room, TileObjData.Type type, List<TileObjData> free_tiles)
    {
		GetTilesOfTypeWithObject(floor,room,type,TileObjData.Obj.None,free_tiles);
    }

    /// <summary>
    /// Returns all tiles of a certain type inside a room which have an object.
    /// If room is null the whole map is scanned.
    /// </summary>
	void GetTilesOfTypeWithObject(FloorObjData floor,ShipRoomObjData room, TileObjData.Type type,TileObjData.Obj obj, List<TileObjData> free_tiles)
    {
        free_tiles.Clear();
        
		int rx=0,ry=0,rw=floor.TileMapW,rh=floor.TileMapH;
        
        if (room!=null){
            rx=room.X+room.XOFF;ry=room.Y+room.YOFF;
            rw=room.W;rh=room.H;
        }
        
        for (int x = 0; x < rw; x++)
        {
            for (int y = 0; y < rh; y++)
            {
				var tile = floor.TileObjectMap[rx+x, ry+y];
                if (tile.TileType == type && tile.ObjType == obj)
                {
                    free_tiles.Add(tile);
                }
            }
        }
    }

    public void GenerateMissionObjectives(GameController GC, MissionObjData mission, ShipObjData ship){

        if (mission.ContainsObjective(MissionObjData.Objective.FindItem)){
			var objective=XmlDatabase.Objectives[MissionObjData.Objective.FindItem];
			var quest_item=XmlDatabase.GetQuestItem(objective.Item);

			if (quest_item==null){
				Debug.LogError("Mission: "+mission.MissionType+" failed to create objective item "+objective.Item+" as it's not found in the xmldatabase");
				return;
			}

			//generate item somewhere in ship
            string obj_room=objective.Room;
			var LegitRooms=new Dictionary<int,List<ShipRoomObjData>>();
			bool nonefound=true;
			for (int i=0;i<ship.FloorRooms.Count;++i){
				LegitRooms.Add(i,new List<ShipRoomObjData>());
				foreach(var r in ship.FloorRooms[i]){
					if (r.roomStats.type==obj_room) 
					{
						LegitRooms[i].Add(r);
						nonefound=false;
					}
	            }
			}
            if (nonefound){
                Debug.LogError("Mission: "+mission.MissionType+" failed to create objective item in ship type "+ship.Name +" required room : "+obj_room);
                return;
            }

			var legit_floors=new List<int>();
			foreach(var f in LegitRooms){
				if (f.Value.Count>0){
					legit_floors.Add(f.Key);
				}
			}
			int index=Subs.GetRandom(legit_floors);
			var floor=GC.Floors[index];
            var room=Subs.GetRandom(LegitRooms[index]);
            int rx=room.X+room.XOFF,ry=room.Y+room.YOFF,rw=room.W,rh=room.H;

            List<LootCrateMain> loot=new List<LootCrateMain>();

            for (int x = 0; x < rw; x++)
            {
                for (int y = 0; y < rh; y++)
                {
					var tile = floor.TileMainMap[rx+x,ry+y];
                    if (tile.Data.ObjType == TileObjData.Obj.Loot)
                    {
                        loot.Add(tile.TileObject.GetComponent<LootCrateMain>());
                    }
                }
            

            if (loot.Count==0){
                Debug.LogWarning("Cannot generate quest item. No loot crates in room "+obj_room+ " ship type "+ship.Name);
                return;
            }

            //DEV. temp mission item type to xml objective data
            var l=Subs.GetRandom(loot);
			var item=new InvGameItem(quest_item);
            l.Items.Add(item);
        }
    }
	}
}

