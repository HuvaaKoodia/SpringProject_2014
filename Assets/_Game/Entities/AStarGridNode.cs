using UnityEngine;
using System.Collections;

public class AStarGridNode : AStarNode {

    public TileMain[,] tilemap;
    int mapWidth, mapHeight;

    public TileMain Tile;
    public TileObjData Data;
    public int PosX { get; private set; }
    public int PosY { get; private set; }
    //public int PosZ { get; private set; }

    public AStarGridNode(AStarNode parent, AStarNode goal, TileMain tile)
        : base(parent, goal, 1)
    {
        tilemap = GameObject.Find("SharedSystems").GetComponentInChildren<GameController>().TileMainMap;
        mapWidth = tilemap.GetLength(0);
        mapHeight = tilemap.GetLength(1);

        this.Tile = tile;
        this.Data = tile.Data;
        this.PosX = (int)Data.TilePosition.x;
        this.PosY = (int)Data.TilePosition.y;
    }

    public virtual bool IsSameState(AStarNode other)
    {
        if (other == null)
            return false;
        else
        {
            return
                (((AStarGridNode)other).PosX == PosX &&
				 ((AStarGridNode)other).PosY == PosY );
        }
    }

    public virtual void Calculate()
    {
        if (GoalNode != null)
        {
            float xd = PosX - ((AStarGridNode)GoalNode).PosX;
            float yd = PosY - ((AStarGridNode)GoalNode).PosY;

            GoalEstimate = Mathf.Max(Mathf.Abs(xd), Mathf.Abs(yd));
        }
        else
            GoalEstimate = 0;
    }

    public override void GetSuccessors(ArrayList ASuccessors)
    {
        //No diagonal movement, take out comments to get them
        ASuccessors.Clear();
        AddSuccessor(ASuccessors, PosX - 1, PosY);
        //AddSuccessor(ASuccessors, PosX - 1, PosY - 1);
        AddSuccessor(ASuccessors, PosX, PosY - 1);
        //AddSuccessor(ASuccessors, PosX + 1, PosY - 1);
        AddSuccessor(ASuccessors, PosX + 1, PosY);
        //AddSuccessor(ASuccessors, PosX + 1, PosY + 1);
        AddSuccessor(ASuccessors, PosX, PosY + 1);
        //AddSuccessor(ASuccessors, PosX - 1, PosY + 1);
    }

    private void AddSuccessor(ArrayList ASuccessors, int aX, int aY)
    {
		if (aX < 0 || aX > mapWidth || aY < 0 || aY > mapHeight)
			return;
		TileMain successorTile = new TileMain();
		try
		{
			successorTile = tilemap[aX, aY];
		}
		catch
		{
			Debug.Log ("mitvit");
		}
        if (successorTile.Data.TileType != TileObjData.Type.Floor)
        {
            return;
        }

        AStarGridNode newNode = new AStarGridNode(this, GoalNode, successorTile);
        if (newNode.IsSameState(Parent))
        {
            return;
        }

        ASuccessors.Add(newNode);
    }

    public override void PrintNodeInfo()
    {
        Debug.Log("X: " + PosX + " Y: " + PosY + " Est: " + GoalEstimate + " Total: " + TotalCost);
    }

    //private bool CanMoveToSuccessor(int x, int y)
    //{
    //    if (x < 0 || x > mapWidth - 1 ||
    //        y < 0 || y > mapHeight - 1)
    //    {
    //        //Debug.Log("Out of range, X: " + x + " Y: " + y);
    //        return false;
    //    }

    //    TileMain nextTile = tilemap[x, y];
    //    if (nextTile.Data.TileType == TileObjData.Type.Floor && nextTile.entityOnTile == null)
    //        return true;
    //    else
    //        return false;
    //}
}
