﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HaxKnifeCulling : MonoBehaviour {

	GameController GC;
	List<MiniMapTileData[,]> miniMapData;

	LayerMask mask;
	// Use this for initialization
	void Start () {
		mask=1<<LayerMask.NameToLayer("Wall");
	}

	public void Init(GameController GC)
	{
		this.GC = GC;
		miniMapData = GC.MiniMapData.mapFloors;
	}
	
	int legit_walls;
	RaycastHit WallHit,WallHit2;
	Ray ray;
	RaycastHit[] WallHits;
	Plane[] planes;
	TileMain tile;

	public bool DEBUG_ShowRays=true;
	public float TileCornerPosRadius=1f;

	public void DisableOtherFloors(int index,GameController GC){
		for(int f=0;f<GC.Floors.Count;++f){
			GC.FloorContainers[f].SetActive(index==f);
		}
	}

	public void CullBasedOnPositions(Vector3 pos1,Vector3 pos2,float detect_radius,GameController GC, bool StopAtDoors,bool updateMap){
		//Debug.Log("Start Culling.");

		for(int y=0;y<GC.CurrentFloorData.TileMapH;++y){
			for(int x=0;x<GC.CurrentFloorData.TileMapW;++x){
				tile = GC.CurrentFloorData.TileMainMap[x,y];
				if (tile.TileGraphics==null) continue;
				if (IsCullable(tile,pos1,detect_radius, StopAtDoors,updateMap)&&IsCullable(tile,pos2,detect_radius, StopAtDoors,updateMap)){
					tile.ShowGraphicsUnsafe(false);
				}
				else{
					tile.ShowGraphicsUnsafe(true);
				}
			}
		}

		CullEnemies(pos1, pos2, detect_radius, GC,StopAtDoors);
	}

	private static Vector3[] Positions={new Vector3(0,0,0),new Vector3(1,0,1),new Vector3(1,0,-1),new Vector3(-1,0,-1),new Vector3(-1,0,1)};
	
	//DEv. StopAtDoors isn't used anymode. Get loose of it
	private bool IsCullable(TileMain tile,Vector3 position,float detect_radius, bool StopAtDoors,bool updateMap){
		bool cull_this=true;

		position+=Vector3.up;
		if (Vector3.Distance(tile.transform.position,position)>detect_radius){
			return true;
		}

		for(int i=0;i<5;++i){
			var tile_pos=Vector3.up+tile.transform.position+Positions[i]*TileCornerPosRadius;
			var direction=tile_pos-position;
			ray=new Ray(position,direction);
			WallHits=Physics.RaycastAll(ray,direction.magnitude,mask);

			bool sees_tile=false;

			if (WallHits.Length == 0){
				sees_tile=true;//show directly seen tiles all the time
			}

			if (sees_tile){
				if (updateMap) miniMapData[GC.CurrentFloorIndex][tile.Data.X, tile.Data.Y].SeenByPlayer = true;
				cull_this=false;
				if (DEBUG_ShowRays) Debug.DrawLine(ray.origin,tile_pos,Color.green,5);
				break;
			}
		}
		return cull_this;
	}

	public void ResetCulling(GameController GC){
		foreach(var o in GC.CurrentFloorData.TileMainMap){
			o.ShowGraphicsSafe(true);
		}
	}

	private void CullEnemies(Vector3 pos1, Vector3 pos2, float detectRadius, GameController GC,bool StopAtDoors)
	{
		List<EnemyMain> enemies = GC.CurrentFloorData.Enemies;

		for (int i = 0; i < enemies.Count; i++)
		{
			if (IsCullable(GC.CurrentFloorData.TileMainMap[enemies[i].movement.currentGridX, enemies[i].movement.currentGridY], pos1, detectRadius, StopAtDoors,false) &&
			    IsCullable(GC.CurrentFloorData.TileMainMap[enemies[i].movement.currentGridX, enemies[i].movement.currentGridY], pos2, detectRadius, StopAtDoors,false))
			{
				enemies[i].CullHide();
			}
			else
			{
				enemies[i].CullShow();
			}
		}
	}

	public void CullMovingEnemy(EnemyMain enemy, Vector3 playerPosition, Point3D enemyStartPosition, Point3D enemyEndPosition, float detectRadius, GameController GC)
	{
		if (IsCullable(GC.CurrentFloorData.TileMainMap[enemyStartPosition.X, enemyStartPosition.Y], playerPosition, detectRadius, false,false) &&
		    IsCullable(GC.CurrentFloorData.TileMainMap[enemyEndPosition.X, enemyEndPosition.Y], playerPosition, detectRadius, false,false))
		{
			enemy.CullHide();
		}
		else
		{
			enemy.CullShow();
		}
	}
}
