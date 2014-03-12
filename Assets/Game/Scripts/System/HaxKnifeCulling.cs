using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HaxKnifeCulling : MonoBehaviour {

	int mask;
	// Use this for initialization
	void Start () {
		mask=1<<LayerMask.NameToLayer("Wall");
	}
	
	int legit_walls;
	RaycastHit WallHit,WallHit2;
	Ray ray;
	RaycastHit[] WallHits;
	Plane[] planes;
	TileMain tile;

	public void CullBasedOnCamera(Camera cam,GameController GC){
		planes=GeometryUtility.CalculateFrustumPlanes(cam);
		Debug.Log("---StartCull!!:");
		for(int y=0;y<GC.TileMapH;++y){
			for(int x=0;x<GC.TileMapW;++x){
				tile = GC.TileMainMap[x,y];

				if (tile.TileGraphics==null) continue;

				//if (GeometryUtility.TestPlanesAABB(planes,tile.transform.collider.bounds)){
					ray=new Ray(cam.transform.position,tile.transform.position-cam.transform.position);
					WallHits=Physics.RaycastAll(ray,cam.farClipPlane+1,mask);
					legit_walls=0;
					for (int w=0;w<WallHits.Length;w++){
						WallHit=WallHits[w];
						if (tile.TileObject!=null) continue;
						//is_legit=true;

						//check if in the last tile
						if (tile.TileGraphics.Colliders==WallHit.transform.gameObject) continue;

//						if (WallHits.Length==1){
//							legit_walls=1;break;
//						}

						//check if too close to another collider
//						for (int w2=0;w2<WallHits.Length;w2++){
//							WallHit2=WallHits[w2];
//							if (w==w2) continue;
//							if (Mathf.Abs(WallHit.distance-WallHit2.distance)<2f){
//								is_legit=false;
//								break;
//							}
//						}
//						if (is_legit)
							++legit_walls;
					}

					if (legit_walls>2){
						tile.ShowGraphicsUnsafe(false);
						continue;
					}
					tile.ShowGraphicsUnsafe(true);
				}
				//o.ShowGraphics(false);
	        	//}
		}
	}

	public void CullBasedOnPositions(Vector3 pos1,Vector3 pos2,float detect_radius,GameController GC){
		//Debug.Log("Start Culling.");
		for(int y=0;y<GC.TileMapH;++y){
			for(int x=0;x<GC.TileMapW;++x){
				tile = GC.TileMainMap[x,y];
				if (tile.TileGraphics==null) continue;
				if (IsCullable(tile,pos1,detect_radius)&&IsCullable(tile,pos2,detect_radius)){
					tile.ShowGraphicsUnsafe(false);
				}
				else{
					tile.ShowGraphicsUnsafe(true);
				}
			}
		}

//		for (int i=0;i<TilesToCull.Count;++i){
//			var tile=TilesToCull[i];
//			tile.ShowGraphicsUnsafe(false);
//		}
	}

	private static Vector3[] Positions={new Vector3(0,0,0),new Vector3(1,1,0),new Vector3(1,-1,0),new Vector3(-1,-1,0),new Vector3(-1,1,0)};

	//DEV. todo optimize
	private bool IsCullable(TileMain tile,Vector3 position,float detect_radius){
		bool cull_this=true;

		position+=Vector3.up;
		if (Vector3.Distance(tile.transform.position,position)>detect_radius){
			return true;
		}

		for(int i=0;i<5;++i){
			var tile_pos=Vector3.up+tile.transform.position+Positions[i]*1f;
			var direction=tile_pos-position;
			ray=new Ray(position,direction);
			WallHits=Physics.RaycastAll(ray,direction.magnitude,mask);

			legit_walls=0;
			legit_walls=WallHits.Length;
//			for (int w=0;w<WallHits.Length;w++){
//				WallHit=WallHits[w];
//
//				//if (tile.TileObject!=null) continue;//has door
//				//if (tile.TileGraphics.Colliders==WallHit.transform.gameObject) continue;//collider part of last tile
//				
//				++legit_walls;
//			}
			if (legit_walls<=1){
				cull_this=false;
				//Debug.DrawLine(ray.origin,tile_pos,Color.green,5);
				break;
			}else{
				//Debug.DrawLine(ray.origin,tile_pos,Color.red,5);
			}
		}
		return cull_this;
	}

	public void ResetCulling(GameController GC){

		foreach(var o in GC.TileMainMap){
			o.ShowGraphicsSafe(true);
		}
	}
}
