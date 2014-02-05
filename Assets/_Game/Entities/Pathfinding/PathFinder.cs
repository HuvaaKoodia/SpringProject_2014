using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

/// <summary>
/// Author: Roy Triesscheijn (http://www.roy-t.nl)
/// Class providing 3D pathfinding capabilities using A*.
/// Heaviliy optimized for speed therefore uses slightly more memory
/// On rare cases finds the 'almost optimal' path instead of the perfect path
/// this is because we immediately return when we find the exit instead of finishing
/// 'neighbour' loop.
/// </summary>
public static class PathFinder
{                   
    /// <summary>
    /// Method that switfly finds the best path from start to end.
	/// -1 maxChecks checks for all possible paths
    /// </summary>
    /// <returns>The starting breadcrumb traversable via .next to the end or null if there is no path</returns>        
    public static SearchNode FindPath(TileMain[,] world, Point3D start, Point3D end, int maxChecks)
    {
        //note we just flip start and end here so you don't have to.            
		return FindPathReversed(world, end, start, maxChecks); 
    }        

    /// <summary>
    /// Method that switfly finds the best path from start to end. Doesn't reverse outcome
    /// </summary>
    /// <returns>The end breadcrump where each .next is a step back)</returns>
    private static SearchNode FindPathReversed(TileMain[,] world, Point3D start, Point3D end, int maxChecks)
    {

        SearchNode startNode = new SearchNode(start, 0, 0, null);

        MinHeap openList = new MinHeap();            
        openList.Add(startNode);

        int sx = world.GetLength(0);
        int sy = world.GetLength(1);
        int sz = 1;
        bool[] brWorld = new bool[sx * sy * sz];
        int startPos = start.X + (start.Y + start.Z * sy) * sx;
        brWorld[startPos] = true;

		int numCheckedTiles = 0;
        while (openList.HasNext())
        {                
			if (numCheckedTiles == maxChecks)
			{
				break;
			}

			numCheckedTiles++;
            SearchNode current = openList.ExtractFirst();
            Console.WriteLine(current.position);

            if (current.position.GetDistanceSquared(end) < 2)
            {
				//Debug.Log("Checked " + numCheckedTiles + " tiles and found a route");
                return new SearchNode(end, current.pathCost + 1, current.cost + 1, current);
            }

            for (int i = 0; i < surrounding.Length; i++)
            {
                Surr surr = surrounding[i];
                Point3D tmp = new Point3D(current.position, surr.Point);

                if (tmp.X < 0 || tmp.X > sx || tmp.Y < 0 || tmp.Y > sy || tmp.Z < 0 || tmp.Z > 0)
                    continue;

                int brWorldIdx = tmp.X + (tmp.Y + tmp.Z * sy) * sx;

				try{
                if (PositionIsFree(tmp, world, sx, sy, sz) && brWorld[brWorldIdx] == false)
                {
                    brWorld[brWorldIdx] = true;
                    int pathCost = current.pathCost + surr.Cost;
                    int cost = pathCost + tmp.GetDistanceSquared(end);
                    SearchNode node = new SearchNode(tmp, cost, pathCost, current);
                    openList.Add(node);
                }
				}
				catch{
					Debug.Log("brWorldIdx out of range: " + brWorldIdx + " max is: " + brWorld.GetLength(0) + " X: " + tmp.X + " Y: " + tmp.Y);
				}
            }
        }
		//Debug.Log("Checked " + numCheckedTiles + " tiles and didn't find a route");
        return null; //no path found
    }

    private static bool PositionIsFree(Point3D position, TileMain[,] world, int mapWidth, int mapHeight, int mapDepth)
    {
        if (position.X < 0 || position.X > mapWidth - 1 ||
            position.Y < 0 || position.Y > mapHeight - 1 ||
            position.Z < 0 || position.Z > mapDepth - 1)
        {
            //Debug.Log("Out of range, X: " + x + " Y: " + y);
            return false;
        }

		TileMain nextTile = null;

		try
		{
			nextTile = world[position.X, position.Y];
		}
		catch
		{
			Debug.Log(position + " is out of map range");
		}

		var door=nextTile.GetDoor();
		if (door!=null&&!door.IsOpen) 
			return false;
		
		if (nextTile.BlockedForMovement) 
			return false;
		else
			return true;
    }

    class Surr
    {
        public Surr(int x, int y, int z)
        {
            Point = new Point3D(x, y, z);
            Cost = x * x + y * y + z * z;
        }

        public Point3D Point;
        public int Cost;
    }

    //Neighbour options
    private static Surr[] surrounding = new Surr[]{                        
        //Top slice (Y=1)
        //new Surr(-1,1,1), new Surr(0,1,1), new Surr(1,1,1),
        /*new Surr(-1,1,0),*/ new Surr(0,1,0), //new Surr(1,1,0),
        //new Surr(-1,1,-1), new Surr(0,1,-1), new Surr(1,1,-1),
        //Middle slice (Y=0)
        //new Surr(-1,0,1), new Surr(0,0,1), new Surr(1,0,1),
        new Surr(-1,0,0), new Surr(1,0,0), //(0,0,0) is self
        //new Surr(-1,0,-1), new Surr(0,0,-1), new Surr(1,0,-1),
        //Bottom slice (Y=-1)
        //new Surr(-1,-1,1), new Surr(0,-1,1), new Surr(1,-1,1),
        /*new Surr(-1,-1,0),*/ new Surr(0,-1,0), //new Surr(1,-1,0),
        //new Surr(-1,-1,-1), new Surr(0,-1,-1), new Surr(1,-1,-1)            
    };
}           
