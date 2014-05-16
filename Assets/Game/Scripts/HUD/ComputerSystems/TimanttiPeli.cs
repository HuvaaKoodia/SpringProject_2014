using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ComputerSystems{

	public class TimanttiPeli : MonoBehaviour {
		public GameObject ScreenCell_prefab;
		public UIPanel ScreenPanelSmall,ScreenPanelBig;
		public int width=10,height=10,x_size_small=32,y_size_small=32,x_size_big=32,y_size_big=32;

		public int BaseScore=100;

		int score_multiplier=0;

		public float UpdateDelay=0.5f;

		ScreenContext context,contextBig,contextSmall;
		TPObjData Selected,SelectedOld;

		GameGrid Grid;

		bool update_game=false,gameover=false;
		float update_tick=0,update_delay;

		int points=0,scorebar_y;
		public GameObjData GameData;

		public Color Basic_color=Color.white,Select_color=Color.red,Highlight_color=Color.blue,Intro_color=Color.magenta;
		int intro_graphics_step=0,intro_step=0,intro_temp=0,intro_temp2=0;

		//logic
		void Start () 
		{
			scorebar_y=height-1;

			contextSmall=new ScreenContext(width,height,x_size_small,y_size_small);
			contextBig=new ScreenContext(width,height,x_size_big,y_size_big);
			//instantiating screen context
			InstantiateContext(ScreenPanelSmall,contextSmall);
			InstantiateContext(ScreenPanelBig,contextBig);

			SetContext(true);

			Grid=new GameGrid(this,0,0,width,height-1);

			ResetGame();
			UpdateGame();

			//gameover=true;
			//update_game=true;
			//points=100;
		}

		void Update(){
			if (update_game){
				update_tick-=Time.deltaTime;
				if (update_tick<0){
					UpdateGame();
				}
			}
#if UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.M)){
				gameover=true;
				update_game=true;
				intro_graphics_step=0;intro_step=0;intro_temp=0;intro_temp2=0;
			}
#endif
		}

		void UpdateGame(){
			update_game=false;
			update_delay=UpdateDelay;

			int added_points=0;

			//game logic
			if (gameover){
				if (intro_step==0){
					int x=intro_graphics_step%width;
					int y=intro_graphics_step/width;

					var cell=context.GetCell(x,y);
					var txt="GAMEOVER!".ToCharArray();
					cell.SetText(""+txt[intro_graphics_step%txt.Length]);
					cell.SetColor(Intro_color);

					update_delay=0.1f;
					++intro_graphics_step;

					if (intro_graphics_step==width*height){
						update_delay=3;
						intro_graphics_step=0;

						intro_step=1;
					}
				}
				else if (intro_step==1){
					if (AddHighScore(points)){
						intro_step=2;
						intro_graphics_step=context.Height-1;
						intro_temp=intro_temp2=0;
					}
					else{
						intro_step=3;
						intro_graphics_step=context.Height-1;
					}
				}
				if (intro_step==2){
					//print hiscore!
					context.ClearRow(intro_graphics_step);
					if (intro_temp==0){
						if (intro_graphics_step==4)
						context.Write(0,4,"NEW",Select_color);
						else if (intro_graphics_step==3)
						context.Write(0,3,"HIGH",Select_color);
						else if (intro_graphics_step==2)
						context.Write(0,2,"SCORE",Select_color);
						else if (intro_graphics_step==1)
						context.Write(0,1,""+points,Select_color);
					}
					update_delay=0.06f;

					--intro_graphics_step;
					if (intro_graphics_step<0){
						intro_temp=intro_temp==0?1:0;
						intro_graphics_step=context.Height-1;
						++intro_temp2;
						if (intro_temp2==9){
							++intro_step;
							intro_graphics_step=context.Height-1;
							update_delay=1.5f;
						}
					}
				}
				else if (intro_step==3){
					update_delay=0.3f;

					context.ClearRow(intro_graphics_step);
					if (intro_graphics_step== context.Height-1){
						var cell=context.GetCell(0,intro_graphics_step);
						cell.SetText("HIGHSCORES:");
						cell.SetColor(Intro_color);
						intro_temp=0;
					}
					else{
						if (intro_temp<GameData.TPhighScores.Length){
							context.Write(0,intro_graphics_step,""+GameData.TPhighScores[intro_temp],Basic_color);
							++intro_temp;
						}
					}

					--intro_graphics_step;

					if (intro_graphics_step<0){
						intro_step++;
						update_delay=5;
					}
				}
				else if (intro_step==4){
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
								obj.Type=Subs.GetRandom(TPObjData.TypeAmount);
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
						if (obj.HighLighted) continue;
						if (hasMatch(obj)){
							var matches=getMatches(obj);
							
							foreach(var m in matches){
								m.HighLight();
							}

							update_game=true;
							update_delay=UpdateDelay*1.5f;

							++score_multiplier;

							float add=(BaseScore*(1f+(matches.Count-3)*2f));
							added_points+=(int)add*score_multiplier;

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

				//draw game objects
				for (int x=0;x<Grid.Width;++x){
					for (int y=0;y<Grid.Height;++y){
						var obj=Grid.GetObj(x,y);
						obj.Draw(context);
					}
				}

				//set score bar
				if (added_points>0){
					SetScorebar("+",added_points,Select_color);
					context.Write(0,context.Height-2,score_multiplier.ToString()+"X",Select_color);
				}
				else{
					SetScorebar("",points,Basic_color);
				}
				points+=added_points;

			}
			update_tick=update_delay;

			if (!update_game) score_multiplier=0;
		}

		//functions
		
		void SetContext(bool small){
			if (small){
				contextSmall.SetData(contextBig);
				context=contextSmall;
				ScreenPanelSmall.gameObject.SetActive(true);
				ScreenPanelBig.gameObject.SetActive(false);
			}
			else{
				contextBig.SetData(contextSmall);
				context=contextBig;
				ScreenPanelSmall.gameObject.SetActive(false);
				ScreenPanelBig.gameObject.SetActive(true);
			}
			
			foreach(var c in context.ScreenCells){//HD haxy hax... But it works?!?
				c.label.fontSize=-1;
				c.label.ResizeCollider();
				c.label.fontSize=50;
				c.label.ResizeCollider();
			}
		}
		
		public void ToggleContext ()
		{
			SetContext(context==contextBig);
		}

		void InstantiateContext(UIPanel panel,ScreenContext con){
			for (int x=0;x<width;++x){
				for (int y=0;y<height;++y){
					var cell=Instantiate(ScreenCell_prefab) as GameObject;
					
					var lab=cell.GetComponent<UILabel>();
					//lab.width=cell_x_size;lab.height=cell_y_size;
					lab.pivot=UIWidget.Pivot.BottomLeft;
					
					cell.transform.parent=panel.transform;
					cell.transform.localPosition=new Vector3(x*con.CellWidth,y*con.CellHeight);
					cell.transform.localScale=Vector3.one*(con.CellWidth/16);
					cell.transform.localRotation=Quaternion.identity;
					
					var data=new CellObjData(x,y,lab);
					con.ScreenCells[x,y]=data;
				}
			}
		}

		bool AddHighScore(int score){
			bool added=false;
			int old_score=0;
			for (int i=0;i<GameData.TPhighScores.Length;++i){
				int hs=GameData.TPhighScores[i];
				if (added){
					GameData.TPhighScores[i]=old_score;
					old_score=hs;
				}
				else if (score>hs){
					added=true;
					old_score=hs;
					GameData.TPhighScores[i]=score;
				}
			}
			return added;
		}

		bool NoMovesRemaining(){
			for (int x=0;x<Grid.Width;++x){
				for (int y=0;y<Grid.Height;++y){
					var obj=Grid.GetObj(x,y);
					TPObjData next=null;
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
		bool hasMatch(TPObjData obj){
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
		List<TPObjData> getMatches(TPObjData obj){
			var allMatches=new List<TPObjData>();
			AddToList(obj,true,allMatches);
			AddToList(obj,false,allMatches);

			return allMatches;
		}

		/// <summary>
		/// Adds all matching adjacent cells to the list.
		/// </summary>
		void AddToList(TPObjData start_cell,bool horizontal,List<TPObjData> total_list){

			var temp_list=new List<TPObjData>();
			TPObjData next=null;
			for (int i=-4;i<5;++i){
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

				int x=(int)(relative.x/(context.CellWidth));
				int y=(int)(relative.y/(context.CellHeight));

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

		void SwapCells (TPObjData c1, TPObjData c2)
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
							data.Type=Subs.GetRandom(TPObjData.TypeAmount);
						}
						while (hasMatch(data));
					}
				}
			}
			while(NoMovesRemaining());
		}
	}

	class TPObjData{
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

		TimanttiPeli TP;
		List<string> GSet;
		//logic
		public TPObjData(List<string> gset,TimanttiPeli tp, int gx,int gy,int sx_off,int sy_off){
			GSet=gset;
			TP=tp;
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
			if (Type<0) return "";
			return GSet[Type];
		}

		Color getTypeColor(){
			if (highlight) return TP.Highlight_color;
			if (selected) return TP.Select_color;

			return TP.Basic_color;
		}
	}

	class GameGrid{
		List<List<string>> GSets;
		
		void initGSets(){
			GSets=new List<List<string>>();
			
			GSets.Add(new List<string>(){"$","±","■","♂","®"});
			GSets.Add(new List<string>(){"♥","♦","♣","♠","■"});
			GSets.Add(new List<string>(){"¥","$","%","€","£"});
			GSets.Add(new List<string>(){"Â","Ñ","Õ","Ü","Ð"});
			GSets.Add(new List<string>(){"Ø","■","#","¶","§"});
			
		}

		public TPObjData[,] Grid;

		//game grid position
		public int X{get;private set;}
		public int Y{get;private set;}

		public int Width {get{return Grid.GetLength(0);}}
		public int Height{get{return Grid.GetLength(1);}}

		public GameGrid(TimanttiPeli tp,int xpos,int ypos,int w,int h){
			X=xpos;Y=ypos;
			Grid=new TPObjData[w,h];
			
			initGSets();
			var set=Subs.GetRandom(GSets);

			for (int x=0;x<Width;++x){
				for (int y=0;y<Height;++y){
					Grid[x,y]=new TPObjData(set,tp,x,y,X,Y);
				}
			}
		}

		public TPObjData GetObj(int x,int y){
			if (!Subs.insideArea(x,y,0,0,Width,Height)) return null;
			return Grid[x,y];
		}
	}

	class ScreenContext{
		public CellObjData[,] ScreenCells;

		public int CellWidth {get;private set;}
		public int CellHeight{get;private set;}

		public int Width {get{return ScreenCells.GetLength(0);}}
		public int Height{get{return ScreenCells.GetLength(1);}}

		public ScreenContext(int w,int h,int cell_x_size,int cell_y_size){
			CellWidth=cell_x_size;CellHeight=cell_y_size;
			ScreenCells=new CellObjData[w,h];
		}

		public CellObjData GetCell(int x, int y)
		{
			if (!Subs.insideArea(x,y,0,0,Width,Height)) return null;
			return ScreenCells[x,y];
		}

		public void Clear ()
		{
			for (int y=0;y<Height;++y){
				ClearRow(y);
			}
		}

		public void ClearRow (int y)
		{
			for (int x=0;x<Width;++x){
				var cell=GetCell(x,y);
				cell.SetText("");
			}
		}

		public void SetData (ScreenContext other)
		{
			if (other.Width==Width&&other.Height==Height)
			{
				for (int x=0;x<Width;++x){
					for (int y=0;y<Height;++y){
						GetCell(x,y).SetText(other.GetCell(x,y).Text);
						GetCell(x,y).SetColor(other.GetCell(x,y).Color);
					}
				}
			}
			else{
				Debug.LogWarning("Screen contexts have a different size and cannot be copied");
			}
		}

		public void Write (int start_x, int start_y, string text, Color color)
		{
			var txt=text.ToCharArray();
			for (int i=0;i<txt.Length;++i){
				int x=(start_x+i)%Width;
				int y=start_y-x/Height;

				if (y<0) break;

				var cell=GetCell(x,y);

				cell.SetText(""+txt[i]);
				cell.SetColor(color);
			}
		}
	}

	class CellObjData{
		public UILabel label;

		public string Text{get{return label.text;}}
		public Color Color{get{return label.color;}}

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