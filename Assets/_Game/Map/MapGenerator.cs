using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Generates TileObjectData maps and TileObject maps from ShipObjectData.
/// </summary>
/// 
public class MapGenerator : MonoBehaviour
{
    public static Vector3 TileSize = new Vector3(3, 3, 3);
    public const string WallIcon = "w", DoorIcon = "d";
    public bool DEBUG_create_temp_tiles = false;
    public XMLMapLoader XmlMapRead;
    public PrefabStore MapPrefabs;
    
    /// <summary>
    /// Generates the TileObjectDataMap from a XMLMapData file.
    /// DEV. Debug.
    /// </summary>
    public void GenerateObjectDataMap(GameController GC)
    {
        
        //random generate map
        var md = XmlMapRead.Rooms ["room"] [0];
            
        GenerateObjectDataMap(GC, md);
    }
    /// <summary>
    /// Generates the TileObjectDataMap from a XMLMapData file.
    /// </summary>
    public void GenerateObjectDataMap(GameController GC, MapXmlData md)
    {
        
        int w = md.W;
        int h = md.H;
            
        GC.ResetTileObjectMap(w, h);

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                var data = GC.TileObjectMap [x, y] = new TileObjData();
                
                data.TilePosition = new Vector3(x, 0, y);
                
                switch (md.map_data [x, y])
                {
                    case "c":
                        data.SetType(TileObjData.Type.Corridor);
                        break;
                    case ".":
                        data.SetType(TileObjData.Type.Floor);
                        break;
                    case ",":
                        data.SetType(TileObjData.Type.Empty);
                        break;
                    case WallIcon:
                        data.SetType(TileObjData.Type.Wall);
                        break;
                    case DoorIcon:
                        data.SetType(TileObjData.Type.Door);
                        break;
                    case "p":
                        data.SetType(TileObjData.Type.Floor);
                        data.SetObj(TileObjData.Obj.Player);
                        break;
                    case "e":
                        data.SetType(TileObjData.Type.Floor);
                        data.SetObj(TileObjData.Obj.Enemy); 
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Generates the 3d world objects to the scene from the ObjectDataMap.
    /// </summary>
    public void GenerateSceneMap(GameController GC)
    {
        //ObjectContainers
        var clone_container = new GameObject("WorldObjects");
        var tile_container = new GameObject("Tiles");
        var enemy_container = new GameObject("Enemies");
        var object_container = new GameObject("Objects");
        object_container.transform.parent = clone_container.transform;
        enemy_container.transform.parent = clone_container.transform;
        tile_container.transform.parent = clone_container.transform;

        //map loading
        int w = GC.TileObjectMap.GetLength(0);
        int h = GC.TileObjectMap.GetLength(1);
        
        GC.TileMainMap = new TileMain[w, h];
        
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {   
                //tile
                int y_pos = h - 1 - y;
                var tile_pos = new Vector3(x * TileSize.x, 0, y * TileSize.z);
                var tile = GC.TileMainMap [x, y] = Instantiate(MapPrefabs.TilePrefab, tile_pos, Quaternion.identity) as TileMain;
                tile.SetData(GC.TileObjectMap [x, y]);
                tile.transform.parent = tile_container.transform;

                //tile mesh
                SetTileObject(x, y, tile, GC.TileObjectMap);
                if (tile.TileObject != null)
                    tile.TileObject.transform.parent = tile.transform;

                //game objects
                int[] entity_pos = { x, y };

                switch (tile.Data.ObjType)
                {
                    case TileObjData.Obj.Player:
                        var player = GameObject.Instantiate(MapPrefabs.PlayerPrefab, tile_pos, Quaternion.identity) as PlayerMain;
                        player.name = "Player";

                        GC.player = player;

                        player.movement.SetPositionInGrid(entity_pos);

                        player.transform.parent = clone_container.transform;
                        break;
                                        
                    case TileObjData.Obj.Enemy:
                        var newEnemy = GameObject.Instantiate(MapPrefabs.EnemyPrefab, tile_pos, Quaternion.identity) as EnemyMain;
                        newEnemy.name = "Enemy";
                        newEnemy.movement.SetPositionInGrid(entity_pos);
                        GC.aiController.AddEnemy(newEnemy);
                        
                        newEnemy.transform.parent = enemy_container.transform;
                        break;

                    case TileObjData.Obj.Loot:
                        var LootCrate = GameObject.Instantiate(MapPrefabs.LootCratePrefab, tile_pos, Quaternion.identity) as GameObject;
                        LootCrate.transform.parent = object_container.transform;

                        GC.LootCrates.Add(LootCrate.GetComponent<LootCrateMain>());
                        break;
                }
            }
        }
    }
    /// <summary>
    /// Generates loot to the loot containers.
    /// DEV.
    /// </summary>
    public void GenerateLoot(GameController GC, ShipObjData ship)
    {
        foreach (var c in GC.LootCrates)
        {
            int a = Subs.GetRandom(1, 5);
            for (int i=0; i<a; i++)
            {
                c.Items.Add(InvGameItem.GetRandomItem(GC.SS.GDB));
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
        var md = ship.Floors [current_floor];
        var xml_md = ship.XmlData.Floors [current_floor];

        int w = md.W;
        int h = md.H;

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
    /// <summary>
    /// Sets the correct graphics to tiles.
    /// </summary>
    void SetTileObject(int x, int y, TileMain tile, TileObjData[,] grid)
    {
        var rotation = Quaternion.identity;
        GameObject tileobj = null;//if floor or corridor

        //gather adjacent corridor types
        TileObjData.Type[] tile_types = new TileObjData.Type[9];
    
        int xx = 0, yy = 0;
        for (int i=0; i<8; i++)
        {
            if (i == 0)
            {
                xx = 1;
                yy = 0;
            }
            if (i == 1)
            {
                xx = 1;
                yy = 1;
            }
            if (i == 2)
            {
                xx = 0;
                yy = 1;
            }
            if (i == 3)
            {
                xx = -1;
                yy = 1;
            }
            if (i == 4)
            {
                xx = -1;
                yy = 0;
            }
            if (i == 5)
            {
                xx = -1;
                yy = -1;
            }
            if (i == 6)
            {
                xx = 0;
                yy = -1;
            }
            if (i == 7)
            {
                xx = 1;
                yy = -1;
            }

            if (Subs.insideArea(x + xx, y + yy, 0, 0, grid.GetLength(0), grid.GetLength(1)))
            {
                tile_types [i] = grid [x + xx, y + yy].TileType;
            } else
            {
                tile_types [i] = TileObjData.Type.Empty;
            }
        }
    
        //test functions
        System.Func<TileObjData.Type,bool> FloorOrCorridor =
            obj => {
            return obj == TileObjData.Type.Corridor || obj == TileObjData.Type.Floor || obj == TileObjData.Type.Door;};
        System.Func<TileObjData.Type,bool> Floor =
        obj => {
            return obj == TileObjData.Type.Floor;};
        System.Func<TileObjData.Type,bool> Wall =
            obj => {
            return obj == TileObjData.Type.Wall;};

        switch (tile.Data.TileType)
        {
            case TileObjData.Type.Wall:
                if (DEBUG_create_temp_tiles)
                    tileobj = MapPrefabs.BasicWall;
                break;
            case TileObjData.Type.Empty:
                if (DEBUG_create_temp_tiles)
                    tileobj = MapPrefabs.BasicEmpty;
                break;
            case TileObjData.Type.Door:
                tileobj = MapPrefabs.Corridor_Door;

                if (CheckTypeEqual(FloorOrCorridor, tile_types, 2, 6))
                {
                    rotation = Quaternion.AngleAxis(90, Vector3.up);
                }
                break;
            case TileObjData.Type.Floor:
            case TileObjData.Type.Corridor:
                    //check tile type
                if (CheckTypeEqual(FloorOrCorridor, tile_types, 0, 1, 2, 3, 4, 5, 6, 7))
                {
                    //Floor
                    tileobj = MapPrefabs.Corridor_Floor;
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 0, 1, 2, 3, 4, 5, 6))
                {
                    //Floor 1 edge 1
                    tileobj = MapPrefabs.Corridor_Floor1Edge;
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 0, 2, 3, 4, 5, 6, 7))
                {
                    //Floor 1 edge 2
                    tileobj = MapPrefabs.Corridor_Floor1Edge;
                    rotation = Quaternion.AngleAxis(-90, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 0, 1, 2, 4, 5, 6, 7))
                {
                    //Floor 1 edge 3
                    tileobj = MapPrefabs.Corridor_Floor1Edge;
                    rotation = Quaternion.AngleAxis(180, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 0, 1, 2, 3, 4, 6, 7))
                {
                    //Floor 1 edge 4
                    tileobj = MapPrefabs.Corridor_Floor1Edge;
                    rotation = Quaternion.AngleAxis(90, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 0, 1, 2, 3, 4, 6))
                {
                    //Floor 2 edges 1
                    tileobj = MapPrefabs.Corridor_Floor2Edges;
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 2, 3, 4, 5, 6, 0))
                {
                    //Floor 2 edges 2
                    tileobj = MapPrefabs.Corridor_Floor2Edges;
                    rotation = Quaternion.AngleAxis(-90, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 4, 5, 6, 7, 0, 2))
                {
                    //Floor 2 edges 3
                    tileobj = MapPrefabs.Corridor_Floor2Edges;
                    rotation = Quaternion.AngleAxis(180, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 6, 7, 0, 1, 2, 4))
                {
                    //Floor 2 edges 4
                    tileobj = MapPrefabs.Corridor_Floor2Edges;
                    rotation = Quaternion.AngleAxis(90, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 0, 1, 2, 4, 5, 6))
                {
                    //Floor opposite edges 1
                    tileobj = MapPrefabs.Corridor_FloorOppositeEdges;
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 2, 3, 4, 6, 7, 0))
                {
                    //Floor opposite edges 2
                    tileobj = MapPrefabs.Corridor_FloorOppositeEdges;
                    rotation = Quaternion.AngleAxis(-90, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 0, 1, 2, 4, 6))
                {
                    //Floor 3 edges 1
                    tileobj = MapPrefabs.Corridor_Floor3Edges;
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 2, 3, 4, 6, 0))
                {
                    //Floor 3 edges 2
                    tileobj = MapPrefabs.Corridor_Floor3Edges;
                    rotation = Quaternion.AngleAxis(-90, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 4, 5, 6, 0, 2))
                {
                    //Floor 3 edges 3
                    tileobj = MapPrefabs.Corridor_Floor3Edges;
                    rotation = Quaternion.AngleAxis(180, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 6, 7, 0, 2, 4))
                {
                    //Floor 3 edges 4
                    tileobj = MapPrefabs.Corridor_Floor3Edges;
                    rotation = Quaternion.AngleAxis(90, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 6, 7, 0, 1, 2))
                {
                    //Wall 1
                    tileobj = MapPrefabs.Corridor_OneWall;
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 0, 1, 2, 3, 4))
                {
                    //Wall 2
                    tileobj = MapPrefabs.Corridor_OneWall;
                    rotation = Quaternion.AngleAxis(-90, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 2, 3, 4, 5, 6))
                {
                    //Wall 3
                    tileobj = MapPrefabs.Corridor_OneWall;
                    rotation = Quaternion.AngleAxis(180, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 4, 5, 6, 7, 0))
                {
                    //Wall 4
                    tileobj = MapPrefabs.Corridor_OneWall;
                    rotation = Quaternion.AngleAxis(90, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 0, 2, 4, 6))
                {
                    //Crossroad
                    tileobj = MapPrefabs.Corridor_Crossroad;
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 0, 2, 3, 4))
                {
                    //wall corner 1
                    tileobj = MapPrefabs.Corridor_WallCorner;
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 2, 4, 5, 6))
                {
                    //wall corner 2
                    tileobj = MapPrefabs.Corridor_WallCorner;
                    rotation = Quaternion.AngleAxis(-90, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 4, 6, 7, 0))
                {
                    //wall corner 3
                    tileobj = MapPrefabs.Corridor_WallCorner;
                    rotation = Quaternion.AngleAxis(180, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 6, 0, 1, 2))
                {
                    //wall corner 4
                    tileobj = MapPrefabs.Corridor_WallCorner;
                    rotation = Quaternion.AngleAxis(90, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 0, 1, 2, 4))
                {
                    //wall corner Mirrored 1
                    tileobj = MapPrefabs.Corridor_WallCornerMirrored;
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 2, 3, 4, 6))
                {
                    //wall corner Mirrored 2
                    tileobj = MapPrefabs.Corridor_WallCornerMirrored;
                    rotation = Quaternion.AngleAxis(-90, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 4, 5, 6, 0))
                {
                    //wall corner Mirrored 3
                    tileobj = MapPrefabs.Corridor_WallCornerMirrored;
                    rotation = Quaternion.AngleAxis(180, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 6, 7, 0, 2))
                {
                    //wall corner Mirrored 4
                    tileobj = MapPrefabs.Corridor_WallCornerMirrored;
                    rotation = Quaternion.AngleAxis(90, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 0, 1, 2))
                {
                    //room corner 1
                    tileobj = MapPrefabs.Corridor_RoomCorner;
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 2, 3, 4))
                {
                    //room corner 2
                    tileobj = MapPrefabs.Corridor_RoomCorner;
                    rotation = Quaternion.AngleAxis(-90, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 4, 5, 6))
                {
                    //room corner 3
                    tileobj = MapPrefabs.Corridor_RoomCorner;
                    rotation = Quaternion.AngleAxis(180, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 6, 7, 0))
                {
                    //room corner 4
                    tileobj = MapPrefabs.Corridor_RoomCorner;
                    rotation = Quaternion.AngleAxis(90, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 0, 2, 4))
                {
                    //Tcrossing 1
                    tileobj = MapPrefabs.Corridor_TCrossing;
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 2, 4, 6))
                {
                    //Tcrossing 2
                    tileobj = MapPrefabs.Corridor_TCrossing;
                    rotation = Quaternion.AngleAxis(-90, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 4, 6, 0))
                {
                    //Tcrossing 3
                    tileobj = MapPrefabs.Corridor_TCrossing;
                    rotation = Quaternion.AngleAxis(180, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 6, 0, 2))
                {
                    //Tcrossing 4
                    tileobj = MapPrefabs.Corridor_TCrossing;
                    rotation = Quaternion.AngleAxis(90, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 0, 4))
                {
                    //horizontal corridor
                    tileobj = MapPrefabs.Corridor_TwoWall;
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 2, 6))
                {
                    //vertical corridor
                    tileobj = MapPrefabs.Corridor_TwoWall;
                    rotation = Quaternion.AngleAxis(90, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 0, 2))
                {
                    //corner 1
                    tileobj = MapPrefabs.Corridor_Corner;
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 2, 4))
                {
                    //corner 2
                    tileobj = MapPrefabs.Corridor_Corner;
                    rotation = Quaternion.AngleAxis(-90, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 4, 6))
                {
                    //corner 3
                    tileobj = MapPrefabs.Corridor_Corner;
                    rotation = Quaternion.AngleAxis(180, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 6, 0))
                {
                    //corner 4
                    tileobj = MapPrefabs.Corridor_Corner;
                    rotation = Quaternion.AngleAxis(90, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 0))
                {
                    //DeadEnd 1
                    tileobj = MapPrefabs.Corridor_Deadend;
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 2))
                {
                    //DeadEnd 2
                    tileobj = MapPrefabs.Corridor_Deadend;
                    rotation = Quaternion.AngleAxis(-90, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 4))
                {
                    //DeadEnd 3
                    tileobj = MapPrefabs.Corridor_Deadend;
                    rotation = Quaternion.AngleAxis(180, Vector3.up);
                } else if (CheckTypeEqual(FloorOrCorridor, tile_types, 6))
                {
                    //DeadEnd 4
                    tileobj = MapPrefabs.Corridor_Deadend;
                    rotation = Quaternion.AngleAxis(90, Vector3.up);
                }

                break;
        }
        if (tileobj != null)
            tile.TileObject = Instantiate(tileobj, tile.transform.position, rotation) as GameObject;
    }

    bool CheckTypeEqual(System.Func<TileObjData.Type,bool> test, TileObjData.Type[] tile_types, params int[] dirs)
    {
        foreach (var dir in dirs)
        {
            if (!test(tile_types [dir]))
            {
                return false;
            }
        }
        return true;
    }
}

