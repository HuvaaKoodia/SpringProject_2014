using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    /// Generates items (loot & enemies) to the rooms and corridors of the ship's TileObjectMap
    /// Call after GenerateObjectDataMap and before GenerateSceneMap
    /// </summary>

    public void GenerateShipItems(FloorObjData floor,ShipObjData ship,MissionObjData mission)
    {
        int current_floor = floor.FloorIndex;
        var xml_md = ship.XmlData.Floors [current_floor];

		Debug.Log("mission enemy types: "+mission.MissionEnemyTypes);

		float alien_percent=mission.GetAlienPercent();
		Debug.Log("Alien percent: "+alien_percent);
        int floor_amount_enemies = (int)(Subs.GetRandom(xml_md.EnemyAmountMin, xml_md.EnemyAmountMax)*alien_percent);
        int floor_amount_loot = Subs.GetRandom(xml_md.LootAmountMin, xml_md.LootAmountMax);

		int force_enemy_amount_to_corridors=(int)(floor_amount_enemies*Subs.GetRandom(XmlDatabase.MaxEnemyPercentageOnCorridors));

        //rooms
        List<TileObjData> free_tiles = new List<TileObjData>();

		//reference
		var rooms_list = ship.FloorRooms[current_floor];
		//shallow copy
		var rooms_list_copy = new List<ShipRoomObjData>(rooms_list);

		//loot
        while(rooms_list_copy.Count>0)
        {
			var room = Subs.GetRandomAndRemove(rooms_list_copy);

			GetTilesOfTypeWithObject(free_tiles, floor,room, TileObjData.Type.Floor,TileObjData.Obj.LootArea);
            int l_amount = Subs.GetRandom(room.RoomXmlData.LootAmountMin, room.RoomXmlData.LootAmountMax);

			while (free_tiles.Count>0&&floor_amount_loot > 0 && l_amount > 0)
            {                
                var tile = Subs.GetRandomAndRemove(free_tiles);
                
                l_amount--;
                floor_amount_loot--;

                tile.SetObj(TileObjData.Obj.Loot);
				room.LootCrateTiles.Add(tile);
            }
		}

		//security systems
		float percent=mission.GetSecurityPercent();
		Debug.Log("SecuritySystem percent: "+percent);
		int amount=0;
		foreach (var room in rooms_list)
		{
			GetTilesOfTypeWithObject(free_tiles, floor,room, TileObjData.Type.Floor,TileObjData.Obj.GatlingGunArea);

			int l_amount =(int)Mathf.Ceil(free_tiles.Count()*percent);

			if (free_tiles.Count()>0) Debug.Log("SecuritySystem amount: "+free_tiles.Count()+" -> "+l_amount);

			while (free_tiles.Count>0&&l_amount>0)
			{	

				var tile = Subs.GetRandom(free_tiles);
				free_tiles.Remove(tile);
				
				--l_amount;
				--floor_amount_loot;
				
				tile.SetObj(TileObjData.Obj.GatlingGun);
				++amount;
			}
		}
		Debug.Log("security amount: "+amount);

		//change some unused loot areas to clutter areas

		GetTilesOfTypeWithObject(free_tiles,floor,null, TileObjData.Type.Floor,TileObjData.Obj.LootArea);
		float clutter_percent=XmlDatabase.ClutterChance;
		int clutter_amount=(int)(free_tiles.Count*clutter_percent);

		while (clutter_amount>0){
			var t=Subs.GetRandomAndRemove(free_tiles);
			t.SetObj(TileObjData.Obj.Clutter);
			--clutter_amount;
		}

		//change unused loot and security areas to empty (so that other things can be spawned on them).

		GetTilesOfTypeWithObject(free_tiles,floor,null, TileObjData.Type.Floor,TileObjData.Obj.LootArea,TileObjData.Obj.GatlingGunArea);
		foreach (var t in free_tiles){
			t.SetObj(TileObjData.Obj.None);
		}

		//Add enemies to rooms
		amount=0;
		rooms_list_copy = new List<ShipRoomObjData>(rooms_list);//shallow copy
		floor_amount_enemies-=force_enemy_amount_to_corridors;
	
		while(rooms_list_copy.Count>0)
		{
			var room = Subs.GetRandomAndRemove(rooms_list_copy);
			GetFreeTilesOfType(floor,room, TileObjData.Type.Floor, free_tiles);
			int e_amount = Subs.GetRandom(room.RoomXmlData.EnemyAmountMin, room.RoomXmlData.EnemyAmountMax);

			while (free_tiles.Count>0&&floor_amount_enemies>0&&e_amount>0)
		    {
		        var tile = Subs.GetRandomAndRemove(free_tiles);

		        --floor_amount_enemies;
				--e_amount;

		        tile.SetObj(TileObjData.Obj.Enemy);
				++amount;
		    }
		}

		//add remaining enemies to corridors
		floor_amount_enemies+=force_enemy_amount_to_corridors;
        
        if (floor_amount_enemies > 0)
        {
			GetFreeTilesOfType(floor,null, TileObjData.Type.Corridor, free_tiles);
            while (free_tiles.Count>0)
            {
                if (floor_amount_enemies == 0) break;
                
                var tile = Subs.GetRandom(free_tiles);
                free_tiles.Remove(tile);
                
                floor_amount_enemies--;
                
                tile.SetObj(TileObjData.Obj.Enemy);
				++amount;
            }
        }

		Debug.Log("Alien amount: "+amount);
    }
	
    public void GenerateMissionObjectives(GameController GC, MissionObjData mission, ShipObjData ship){

		foreach(var o in mission.PrimaryObjectives){
			var objective=XmlDatabase.Objectives[o.Objective];

			if (objective.Item!=""){
				//generate quest item
				
				var quest_item=XmlDatabase.GetQuestItem(objective.Item);

				if (quest_item==null){
					Debug.LogError("Mission: "+mission.XmlData.Name+" failed to create objective item "+objective.Item+" as it's not found in the xmldatabase");
					return;
				}

				//find legit rooms in all floors
			    string obj_room=objective.Room;
				var LegitRooms=new Dictionary<int,List<ShipRoomObjData>>();
				bool nonefound=true;
				for (int i=0;i<ship.FloorRooms.Count;++i){
					LegitRooms.Add(i,new List<ShipRoomObjData>());
					foreach(var r in ship.FloorRooms[i]){
						if (r.roomStats.type==obj_room&&r.LootCrateTiles.Count>0) 
						{
							LegitRooms[i].Add(r);
							nonefound=false;
						}
		            }
				}
			    if (nonefound){
					Debug.LogWarning("Mission: "+mission.XmlData.Name+" failed to create objective item in ship type "+ship.Name +" required room : "+obj_room);
					Debug.LogWarning("A random room selected for the item");

					foreach (var lrlist in LegitRooms){
						lrlist.Value.Clear();
					}

					for (int i=0;i<ship.FloorRooms.Count;++i){
						foreach(var r in ship.FloorRooms[i]){
							if (r.LootCrateTiles.Count>0)
							{
								LegitRooms[i].Add(r);
							}
						}
					}

					if (LegitRooms.Count==0){
						Debug.LogWarning("Cannot generate the quest item. No loot crates in the ship "+ship.Name);
						continue;
					}
			    }

				//select one room
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

				//find all loot crates in room
			    List<LootCrateMain> loot_crates=new List<LootCrateMain>();

			    for (int x = 0; x < rw; x++)
			    {
			        for (int y = 0; y < rh; y++)
			        {
						var tile = floor.TileMainMap[rx+x,ry+y];
			            if (tile.Data.ObjType == TileObjData.Obj.Loot)
			            {
							loot_crates.Add(tile.TileObject.GetComponent<LootCrateMain>());
			            }
			        }
				}

				//select one loot crate and add item
				var l=Subs.GetRandom(loot_crates);
				var item=new InvGameItem(quest_item);
		    	l.Items.Add(item);
				Debug.Log("Quest item generated successfully!");
			}
		}
	}

	/// <summary>
	/// Randomizes the door states (open,closed,locked,broken) in all ship floors
	/// </summary>
	public void RandomizeDoorStates(GameController GC, ShipObjData shi){
		foreach (FloorObjData f in GC.Floors){
			foreach(var t in f.TileMainMap){
				var door=t.GetDoor();
				if (door!=null&&door.canOpenDoorOnStartUp&&!door.isAirlockDoor){
					if (Subs.RandomPercent()<XmlDatabase.OpenDoorChange){
						door.ForceOpen();
					}
				}
			}
		}
	}

	//private subs
	
	/// <summary>
	/// Returns all tiles of a certain type inside a room which don't have an object yet.
	/// If room is null the whole map is scanned.
	/// </summary>
	void GetFreeTilesOfType(FloorObjData floor,ShipRoomObjData room, TileObjData.Type type, List<TileObjData> free_tiles)
	{
		GetTilesOfTypeWithObject(free_tiles,floor,room,type,TileObjData.Obj.None);
	}
	
	/// <summary>
	/// Returns all tiles of a certain type with an object inside a room.
	/// If room is null the whole map is scanned.
	/// </summary>
	void GetTilesOfTypeWithObject(List<TileObjData> free_tiles,FloorObjData floor,ShipRoomObjData room, TileObjData.Type type,params TileObjData.Obj[] objs)
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
				if (tile.TileType == type&&objs.Contains(tile.ObjType))
				{
					free_tiles.Add(tile);
				}
			}
		}
	}
}

