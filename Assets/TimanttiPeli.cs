using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ComputerSystems{

	public class TimanttiPeli : MonoBehaviour {
		public GameObject ScreenCell_prefab;
		public Transform CellParent;
		public UIPanel ScreenPanel;
		public int width=10,height=10,x_size=32,y_size=32;

		public int BaseScore=100;

		public float UpdateDelay=0.5f;

		ScreenContext context;
		GameObjData Selected,SelectedOld;

		GameGrid Grid;

		bool update_game=false,gameover=false;
		float update_tick=0,update_delay;

		int points=0,scorebar_y;

		//logic
		void Start () 
		{
			scorebar_y=height-1;

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

			Grid=new GameGrid(0,0,width,height-1);

			ResetGame();
			UpdateGame();
		}

		int gameover_step=0;

		void UpdateGame(){
			update_game=false;
			update_delay=UpdateDelay;

			int added_points=0;


			//game logic
			if (gameover){
				int x=gameover_step%width;
				int y=gameover_step/width;

				var cell=context.GetCell(x,y);
				var txt="GAMEOVER!".ToCharArray();
				cell.SetText(""+txt[gameover_step%txt.Length]);
				cell.SetColor(Color.magenta);

				update_delay=0.1f;
				++gameover_step;

				if (gameover_step==width*height){
					update_delay=3;
					gameover_step=0;
					ResetGame();
				}
				update_game=true;
			}
			else{
				//drop tiles if over empty
				for (int x=0;x<Grid.Width;++x){
					for (int y=0;y<Grid.Height;++y){
						var obj=Grid.GetObj(x,y);
						obj.moving=false;
						if (obj.Empty){

							if (y+1==Grid.Height){//in top row
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
				for (int x=0;x<Grid.Width;++x){
					for (int y=0;y<Grid.Height;++y){
						var obj=Grid.GetObj(x,y);
						if (hasMatch(obj)){
							var matches=getMatches(obj);
							
							foreach(var m in matches){
								m.HighLight();
								update_game=true;
								update_delay=UpdateDelay*1.5f;
							}

							added_points+=BaseScore*(1+(matches.Count-3));
						}
					}
				}

				//check for no moves left
				if (!update_game){
					if (NoMovesRemaining()){
						gameover=true;
						update_delay=2.700f;
						for (int x=0;x<Grid.Width;++x){
							for (int y=0;y<Grid.Height;++y){
								var obj=Grid.GetObj(x,y);
								obj.Select();
							}
						}
						update_game=true;
					}
				}

				
				//basic drawing
				context.Clear();
				
				//set score bar
				if (added_points>0){
					SetScorebar("+",added_points,Color.red);
				}
				else{
					SetScorebar("",points,Color.white);
				}
				points+=added_points;

				//draw game objects
				for (int x=0;x<Grid.Width;++x){
					for (int y=0;y<Grid.Height;++y){
						var obj=Grid.GetObj(x,y);
						obj.Draw(context);
					}
				}
			}
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

		bool NoMovesRemaining(){
			for (int x=0;x<Grid.Width;++x){
				for (int y=0;y<Grid.Height;++y){
					var obj=Grid.GetObj(x,y);
					GameObjData next=null;
					int dx,dy;
					for (int d=0;d<4;++d){
						dx=MapGenerator.GetCardinalX(d);
						dy=MapGenerator.GetCardinalY(d);

						next=Grid.GetObj(obj.GX+dx,obj.GY+dy);
						if (next==null) continue;
						SwapCells(obj,next);
						bool match=hasMatch(obj);
						SwapCells(next,obj);

						if (match) {
							return false;
						}
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Checks if there is a match for this cell
		/// </summary>
		/// <param name="y">The y coordinate.</param>
		bool hasMatch(GameObjData obj){
			int hm=0,vm=0;
			for (int i=-2;i<3;++i){
				var next=Grid.GetObj(obj.GX+i,obj.GY);
				if (next==null||next.Empty||next.Type!=obj.Type||next.moving) 
					hm=0;
				else{
					if (next.Type==obj.Type) ++hm;
				}
				
				next=Grid.GetObj(obj.GX,obj.GY+i);
				if (next==null||next.Empty||next.Type!=obj.Type||next.moving)
					vm=0;
				else{
					if (next.Type==obj.Type) ++vm;
				}
				if (hm>2||vm>2)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Returns all matching objects.
		///</summary>
		List<GameObjData> getMatches(GameObjData obj){
			var allMatches=new List<GameObjData>();
			AddToList(obj,true,allMatches);
			AddToList(obj,false,allMatches);

			return allMatches;
		}

		/// <summary>
		/// Adds all matching adjacent cells to the list.
		/// </summary>
		void AddToList(GameObjData start_cell,bool horizontal,List<GameObjData> total_list){

			var temp_list=new List<GameObjData>();
			GameObjData next=null;
			for (int i=-2;i<3;++i){
				if (horizontal) next=Grid.GetObj(start_cell.GX+i,start_cell.GY);
				else next=Grid.GetObj(start_cell.GX,start_cell.GY+i);

				if (next==null||next.Empty||next.Type!=start_cell.Type||next.moving){
					//save matches up until now
					if (temp_list.Count>2) foreach(var c in temp_list) total_list.Add(c);
					temp_list.Clear();
				}
				else{
					if (next.Type==start_cell.Type) temp_list.Add(next);
				}
			}
			//save the rest
			if (temp_list.Count>2) foreach(var c in temp_list) total_list.Add(c);
		}

		public void Click (RaycastHit hit)
		{
			if (update_game||gameover) return;
			if (hit.transform!=null){
				var relative=hit.collider.transform.InverseTransformPoint(hit.point);

				int x=(int)(relative.x/(x_size));
				int y=(int)(relative.y/(y_size));

				SetSelected(x,y);

				UpdateGame();
			}
		}

		void SetSelected(int sx,int sy){
			int x=sx-Grid.X;
			int y=sy-Grid.Y;

			SelectedOld=Selected;
			Selected=Grid.GetObj(x,y);
			 
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
				if (SelectedOld!=null)SelectedOld.DeSelect();
				return;
			}

			//swap places
			if (Selected!=null&&SelectedOld!=null){
				int xd=Selected.GX-SelectedOld.GX;
				int yd=Selected.GY-SelectedOld.GY;

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

		void SwapCells (GameObjData c1, GameObjData c2)
		{
			int type=c1.Type;
			c1.Type=c2.Type;
			c2.Type=type;
		}

		void SetScorebar (string str, int points,Color color)
		{
			var pnts=str+points;
			var carray=pnts.ToCharArray();
			int len=Mathf.Min(pnts.Length,width);     
            for (int i=0;i<len;++i){
				var cell=context.GetCell(i,scorebar_y);
				cell.SetText(""+carray[i]);
				cell.SetColor(color);
			}
		}

		void ResetGame ()
		{
			gameover=false;
			points=0;
			//randomized game object types
			do {
				for (int x=0;x<Grid.Width;++x){
					for (int y=0;y<Grid.Height;++y){
						var data=Grid.GetObj(x,y);
						data.DeSelect();
						do{
							data.Type=Subs.GetRandom(GameObjData.TypeAmount);
						}
						while (hasMatch(data));
					}
				}
			}
			while(NoMovesRemaining());
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
		public int SX{get{return GX+SX_off;}}
		public int SY{get{return GY+SY_off;}}

		//logic
		public GameObjData(int gx,int gy,int sx_off,int sy_off){
			Type=0;
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
		public bool Empty{get{return Type<0;}}

		public void DeSelect(){
			highlight=false;
			selected=false;
		}

		public void Draw(ScreenContext context){
			var cell=context.GetCell(SX,SY);

			if (cell!=null){
				cell.SetText(getTypeChar());
				cell.SetColor(getTypeColor());
			}
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

		Color getTypeColor(){
			if (highlight) return Color.blue;
			if (selected) return Color.red;

			return Color.white;
		}
	}

	class GameGrid{
		public GameObjData[,] Grid;

		//game grid position
		public int X{get;private set;}
		public int Y{get;private set;}

		public int Width {get{return Grid.GetLength(0);}}
		public int Height{get{return Grid.GetLength(1);}}

		public GameGrid(int xpos,int ypos,int w,int h){
			X=xpos;Y=ypos;
			Grid=new GameObjData[w,h];

			for (int x=0;x<Width;++x){
				for (int y=0;y<Height;++y){
					Grid[x,y]=new GameObjData(x,y,X,Y);
				}
			}
		}

		public GameObjData GetObj(int x,int y){
			if (!Subs.insideArea(x,y,0,0,Width,Height)) return null;
			return Grid[x,y];
		}
	}

	class ScreenContext{
		public CellObjData[,] ScreenCells;

		public int Width {get{return ScreenCells.GetLength(0);}}
		public int Height{get{return ScreenCells.GetLength(1);}}

		public ScreenContext(int w,int h){
			ScreenCells=new CellObjData[w,h];
		}

		public CellObjData GetCell (int x, int y)
		{
			if (!Subs.insideArea(x,y,0,0,Width,Height)) return null;
			return ScreenCells[x,y];
		}

		public void Clear ()
		{
			for (int x=0;x<Width;++x){
				for (int y=0;y<Height;++y){
					var cell=GetCell(x,y);
					cell.SetText("");
				}
			}
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

		public void SetText (string str)
		{
			label.text=str;
		}

		public void SetColor (Color col)
		{
			label.color=col;
		}
	}
}