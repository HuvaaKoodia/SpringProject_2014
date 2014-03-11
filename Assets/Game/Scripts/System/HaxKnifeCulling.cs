using UnityEngine;
using System.Collections;

public class HaxKnifeCulling : MonoBehaviour {

	int mask;
	// Use this for initialization
	void Start () {
		mask=1<<LayerMask.NameToLayer("Wall");
	}

	bool is_legit=true;
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

	
	public void ResetCulling(GameController GC){

		foreach(var o in GC.TileMainMap){

			o.ShowGraphicsSafe(true);
		}
	}
}
