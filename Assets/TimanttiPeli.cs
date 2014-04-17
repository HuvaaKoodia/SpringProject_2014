using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimanttiPeli : MonoBehaviour {
	
	public GameObject ScreenCell_prefab;
	public Transform CellParent;
	public UIPanel ScreenPanel;
	public int width=10,height=10,x_size=32,y_size=32;

	public float UpdateDelay=0.5f;

	CellObjData[,] ScreenCells;
	CellObjData Selected,SelectedOld;
	
	bool update_game=false;
	float update_tick=0,update_delay;

	//logic
	void Start () 
	{
		//creating cells
		ScreenCells=new CellObjData[x_size,y_size];
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
				ScreenCells[x,y]=data;
			}
		}

		//randomize types
		for (int x=0;x<width;++x){
			for (int y=0;y<height;++y){
				var data=ScreenCells[x,y];
				
				while (hasMatch(data)){
					data.Type=Subs.GetRandom(CellObjData.TypeAmount);
				}
			}
		}
	}

	
	void UpdateGame(){
		update_game=false;
		update_delay=UpdateDelay;
		Debug.Log("Update!");
		
		//drop tiles if over empty
		for (int x=0;x<width;++x){
			for (int y=0;y<height;++y){
				var cell=GetCell(x,y);
				cell.moving=false;
				if (cell.IsEmpty){
					var above=GetCell(x,y+1);
					if (above==null){
						cell.Type=Subs.GetRandom(CellObjData.TypeAmount);
						update_game=true;
						cell.moving=true;
					}
					else if (!above.IsEmpty){
						cell.Type=above.Type;
						above.Type=-1;

						update_game=true;
						cell.moving=true;
					}
				}
			}
		}
		//clear highlighted tiles
		for (int x=0;x<width;++x){
			for (int y=0;y<height;++y){
				var cell=GetCell(x,y);
				if (cell.HighLighted){
					update_game=true;
					cell.Type=-1;
					cell.DeSelect();
				} 
			}
		}
		
		//check for matches
		for (int x=0;x<width;++x){
			for (int y=0;y<height;++y){
				var cell=GetCell(x,y);
				if (hasMatch(cell)){
					var matches=getMatches(cell);
					
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
	bool hasMatch(CellObjData cell){
		int hm=0,vm=0;
		for (int i=-2;i<3;++i){
			var next=GetCell(cell.X+i,cell.Y);
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
	List<CellObjData> getMatches(CellObjData cell){
		var allMatches=new List<CellObjData>();
		AddToList(cell,true,allMatches);
		AddToList(cell,false,allMatches);

		return allMatches;
	}

	/// <summary>
	/// Adds all matching adjacent cells to the list.
	/// </summary>
	void AddToList(CellObjData start_cell,bool horizontal,List<CellObjData> total_list){

		var temp_list=new List<CellObjData>();
		CellObjData next=null;
		for (int i=-2;i<3;++i){
			if (horizontal) next=GetCell(start_cell.X+i,start_cell.Y);
			else next=GetCell(start_cell.X,start_cell.Y+i);

			if (next==null||next.IsEmpty||next.Type!=start_cell.Type||next.moving){
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
		if (update_game) return;
		if (hit.transform!=null){
			var relative=hit.collider.transform.InverseTransformPoint(hit.point);

			int x=(int)(relative.x/(x_size));
			int y=(int)(relative.y/(y_size));

			SetSelected(x,y);

			UpdateGame();
		}
	}

	CellObjData GetCell(int x,int y){
		if (!Subs.insideArea(x,y,0,0,width,height)) return null;
		return ScreenCells[x,y];
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

class CellObjData{

	public static int TypeAmount=5;

	int type=0;

	public int Type{
		get{return type;}
		set{
			type=value;
			label.text=getTypeChar(type);
		}
	}

	public bool IsEmpty {
		get{return type==-1;}
	}

	public UILabel label;

	public int X{get;private set;}
	public int Y{get;private set;}

	public CellObjData(int x,int y,UILabel l){
		label=l;
		X=x;Y=y;
	}

	string getTypeChar(int t){
		switch(t){
			case 0: return "A";
			case 1: return "B";
			case 2: return "C";
			case 3: return "D";
			case 4: return "E";
			default: return "";
		}
	}

	public void Select(){
		label.color=Color.red;
	}
	
	public void HighLight(){
		label.color=Color.blue;
		highlight=true;
	}

	public bool moving=false;
	bool highlight=false;
	public bool HighLighted{get{return highlight;}}

	public void DeSelect(){
		label.color=Color.white;
		highlight=false;
	}
}
