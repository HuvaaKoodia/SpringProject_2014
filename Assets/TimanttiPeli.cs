using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ComputerSystems{

	public class TimanttiPeli : MonoBehaviour {
		
		public GameObject ScreenCell_prefab;
		public Transform CellParent;
		public UIPanel ScreenPanel;
		public int width=10,height=10,x_size=32,y_size=32;

		public float UpdateDelay=0.5f;

		ScreenContext context;
		CellObjData Selected,SelectedOld;

		GameGrid Grid;

		bool update_game=false;
		float update_tick=0,update_delay;

		//logic
		void Start () 
		{
			context=new ScreenContext(width,height);
			//instantiating screen context
			for (int x=0;x<width;++x){
				for (int y=0;y<height;++y){
					var cell=Instantiate(ScreenCell_prefab) as GameObject;

					var lab=cell.GetComponent<UILabel>();
					lab.width=x_size;lab.height=y_size;
					lab.pivot=UIWidget.Pivot.BottomLeft;

					cell.transform.parent=CellParent;
					cell.transform.localPosition=new Vector3(x*x_size,y*y_size);
					cell.transform.localScale=Vector3.one*(x_size/16);
					cell.transform.localRotation=CellParent.localRotation;

					var data=new CellObjData(x,y,lab);
					context.ScreenCells[x,y]=data;
				}
			}
			//creating randomized game objects

			Grid=new GameGrid(width,height-1);
			for (int x=0;x<Grid.Width;++x){
				for (int y=0;y<Grid.Height;++y){
					var data=new GameObjData(x,y,0,1);
					
					while (hasMatch(data)){
						data.Type=Subs.GetRandom(GameObjData.TypeAmount);
					}
				}
			}
		}
		
		void UpdateGame(){
			update_game=false;
			update_delay=UpdateDelay;
			Debug.Log("Update!");
			
			//drop tiles if over empty
			for (int x=0;x<Grid.Width;++x){
				for (int y=0;y<Grid.Height;++y){
					var obj=Grid.GetObj(x,y);
					obj.moving=false;
					if (obj.Empty){
						if (y+1==height){//in top row
							obj.Type=Subs.GetRandom(GameObjData.TypeAmount);
							update_game=true;
							obj.moving=true;
						}
						else{
							var above=Grid.GetObj(x,y+1);
							obj.Type=above.Type;
							above.Type=-1;

							update_game=true;
							obj.moving=true;
						}
					}
				}
			}
			//clear highlighted tiles
			for (int x=0;x<Grid.Width;++x){
				for (int y=0;y<Grid.Height;++y){
					var obj=Grid.GetObj(x,y);
					if (obj.HighLighted){
						update_game=true;
						obj.Clear();
					} 
				}
			}
			
			//check for matches
			for (int x=0;x<width;++x){
				for (int y=0;y<height;++y){
					var obj=Grid.GetObj(x,y);
					if (hasMatch(obj)){
						var matches=getMatches(obj);
						
						foreach(var m in matches){
							m.HighLight();
							update_game=true;
							update_delay=UpdateDelay*1.5f;
						}
					}
				}
			}

			//check for no moves left

			//TODO

			update_tick=update_delay;
		}
		
		void Update(){
			if (update_game){
				update_tick-=Time.deltaTime;
				if (update_tick<0){
					UpdateGame();
				}
			}
		}

		//functions

		/// <summary>
		/// Checks if there is a match for this cell
		/// </summary>
		/// <param name="y">The y coordinate.</param>
		bool hasMatch(GameObjData obj){
			int hm=0,vm=0;
			for (int i=-2;i<3;++i){
				var next=gameGrid.GetObj(obj.GX+i,obj.GY);
				if (next==null||next.IsEmpty||next.Type!=cell.Type||next.moving) 
					hm=0;
				else{
					if (next.Type==cell.Type) ++hm;
				}
				
				next=GetCell(cell.X,cell.Y+i);
				if (next==null||next.IsEmpty||next.Type!=cell.Type||next.moving)
					vm=0;
				else{
					if (next.Type==cell.Type) ++vm;
				}
				if (hm>2||vm>2)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Returns all matching objects.
		///</summary>
		List<CellObjData> getMatches(GameObjData obj){
			var allMatches=new List<GameObjData>();
			AddToList(obj,true,allMatches);
			AddToList(obj,false,allMatches);

			return allMatches;
		}

		/// <summary>
		/// Adds all matching adjacent cells to the list.
		/// </summary>
		void AddToList(GameObjData start_cell,bool horizontal,List<GameObjData> total_list){

			var temp_list=new List<CellObjData>();
			CellObjData next=null;
			for (int i=-2;i<3;++i){
				if (horizontal) next=Grid.GetObj(start_cell.X+i,start_cell.Y);
				else next=GetCell(start_cell.X,start_cell.Y+i);

				if (next==null||next.IsEmpty||next.Type!=start_cell.Type||next.moving){
					//save matches up until now
					if (temp_list.Count>2) foreach(var c in temp_list) total_list.Add(c);
					temp_list.Clear();
				}
				else{
					if (next.Type==start_cell.Type) temp_list.Add(next);
				}l
			}
			//save the rest
			if (temp_list.Count>2) foreach(var c in temp_list) total_list.Add(c);
		}

		public void Click (RaycastHit hit)
		{
			if (update_game) return;
			if (hit.transform!=null){
				var relative=hit.collider.transform.InverseTransformPoint(hit.point);

				int x=(int)(relative.x/(x_size));
				int y=(int)(relative.y/(y_size));

				SetSelected(x,y);

				UpdateGame();
			}
		}

		void SetSelected(int x,int y){
			SelectedOld=Selected;
			Selected=GetCell(x,y);
			 
			//change color
			if (Selected!=null){
				if (SelectedOld==Selected){
					Selected.DeSelect();
					Selected=null;
					return;//nothing to do anymore
				}
				else{
					if (SelectedOld==null){
						Selected.Select();
					}
					else{
						SelectedOld.DeSelect();
					}
				}
			}
			else{
				SelectedOld.DeSelect();
				return;
			}

			//swap places
			if (Selected!=null&&SelectedOld!=null){
				int xd=Selected.X-SelectedOld.X;
				int yd=Selected.Y-SelectedOld.Y;

				if ((Mathf.Abs(xd)<2&&yd==0)||(Mathf.Abs(yd)<2&&xd==0)){
					//next to each other.
					SwapCells(Selected,SelectedOld);

					//swap back if new positions don't have any matches
					if (!hasMatch(Selected)&&!hasMatch(SelectedOld)){
						SwapCells(Selected,SelectedOld);
					}
				}
				Selected=null;
			}
		}

		void SwapCells (CellObjData c1, CellObjData c2)
		{
			int type=c1.Type;
			c1.Type=c2.Type;
			c2.Type=type;
		}
	}

	class GameObjData{
		//data
		public static int TypeAmount=5;
		
		public int Type{get;set;}

		//game grid position
		public int GX{get;private set;}
		public int GY{get;private set;}

		//screen offset
		public int SX_off{get;private set;}
		public int SY_off{get;private set;}

		//screen position
		public int SX{get{return GX+X_off;}}
		public int SY{get{return GY+Y_off;}}

		//logic
		public GameObjData(int gx,int gy,int sx_off,int sy_off){
			GX=gx;GY=gy;
			SX_off=sx_off;SY_off=sy_off;
		}

		public void Select(){
			selected=true;
		}
		
		public void HighLight(){
			highlight=true;
		}

		public void Clear(){
			Type=-1;
			DeSelect();
		}

		public bool moving=false;
		bool highlight=false,selected=false;
		public bool HighLighted{get{return highlight;}}
		public bool Empty{get{return {Type<0;}}}

		public void DeSelect(){
			highlight=false;
			selected=false;
		}

		public void Draw(ScreenContext context){
			var cell=context.GetCell(SX,SY);

			cell.SetText(getTypeChar());
			cell.SetColor(getTypeColor());
		}

		
		string getTypeChar(){
			switch(Type){
			case 0: return "A";
			case 1: return "B";
			case 2: return "C";
			case 3: return "D";
			case 4: return "E";
			default: return "";
			}
		}

		
		string getTypeColor(){
			if (highlight) return Color.blue;
			if (selected) return Color.red;

			return Color.white;
		}
	}

	class GameGrid{
		GameObjData[,] Grid;

		public int Width {get{return Grid.GetLength(0);}}
		public int Height{get{return Grid.GetLength(1);}}

		public GameGrid(int w,int h){
			Grid=new GameObjData[w,h];
		}

		public GameObjData GetObj(int x,int y){
			if (!Subs.insideArea(x,y,0,0,Width,Height)) return null;
			return Grid[x,y];
		}
	}

	class ScreenContext{

		public CellObjData[,] ScreenCells;

		public ScreenContext(int w,int h){
			ScreenCells=new CellObjData[x_size,y_size];
		}

		public CellObjData GetCell (int x, int y)
		{
			return ScreenCells[x,y];
		}
	}

	class CellObjData{
		public UILabel label;

		public int X{get;private set;}
		public int Y{get;private set;}

		public CellObjData(int x,int y,UILabel l){
			label=l;
			X=x;Y=y;
		}
	}
}