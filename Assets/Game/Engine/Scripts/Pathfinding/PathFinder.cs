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
	public static Point3D LatestDestination = null;

	const int blockedPenaltyStart = 100;
	const int blockedPenaltyIncrease = 10;

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
		int currentBlockedPenalty = blockedPenaltyStart;

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
				LatestDestination = start;
                return new SearchNode(end, current.pathCost + 1, current.cost + 1, current);
            }

            for (int i = 0; i < surrounding.Length; i++)
            {
                Surr surr = surrounding[i];
                Point3D tmp = new Point3D(current.position, surr.Point);

                if (tmp.X < 0 || tmp.X > sx || tmp.Y < 0 || tmp.Y > sy || tmp.Z < 0 || tmp.Z > 0)
                    continue;

                int brWorldIdx = tmp.X + (tmp.Y + tmp.Z * sy) * sx;

				try
				{
	                if (PositionIsWalkable(tmp, world, sx, sy, sz) && brWorld[brWorldIdx] == false)
	                {
	                    brWorld[brWorldIdx] = true;
	                    int pathCost = current.pathCost + surr.Cost;
	                    int cost = pathCost + tmp.GetDistanceSquared(end);

						//if someone is blocking the way, make sure that this route
						//doesn't get picked unless its only reasonable route
							if (world[tmp.X, tmp.Y].BlockedForMovement)
							{
								cost += currentBlockedPenalty;
								currentBlockedPenalty += blockedPenaltyIncrease;
							}

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

	/// <summary>
	/// 
	/// </summary>
	/// <param name="world">reference to tiles</param>
	/// <param name="startPoint">point of caller entity</param>
	/// <param name="pointToAvoid">point where enemy that we wan't to get away from is</param>
	/// <param name="distance">0 if no restricted distance</param>
	/// <returns></returns>
	public static SearchNode FindPathToSafety(TileMain[,] world, Point3D startPoint, 
                  Point3D pointToAvoid, EntityMain avoider, EntityMain toAvoid, int distance, LayerMask layerMask)
	{
		SearchNode startNode = new SearchNode(startPoint, 0, 0, null);
		
		MinHeap openList = new MinHeap();
		openList.Add(startNode);
		
		int sx = world.GetLength(0);
		int sy = world.GetLength(1);
		int sz = 1;
		bool[] brWorld = new bool[sx * sy * sz];
		int startPos = startPoint.X + (startPoint.Y + startPoint.Z * sy) * sx;
		brWorld[startPos] = true;
		
		int curDist = 0;
		List<SearchNode> allSearched = new List<SearchNode>();
		
		while (openList.HasNext())
		{
			SearchNode current = openList.ExtractFirst();
			
			if (curDist > distance)
			{
				SearchNode best = allSearched[0];
				current = current.next;
				
				foreach (SearchNode node in allSearched)
				{
					if (node.cost < best.cost)
						best = node;
				}
				
				if (best.position.GetDistanceSquared(pointToAvoid) <= startNode.position.GetDistanceSquared(pointToAvoid))
					return null;
				
				return FindPath(world, startPoint, best.position, -1);
			}
			
			if (!CanSeeFromTileToTile(toAvoid, avoider, current.position, distance, 
			                          layerMask, true))
			{
				return FindPath(world, startPoint, current.position, -1);
			}
			
			for (int i = 0; i < surrounding.Length; i++)
			{
				Surr surr = surrounding[i];
				Point3D tmp = new Point3D(current.position, surr.Point);
				
				if (tmp.X < 0 || tmp.X > sx-1 || tmp.Y < 0 || tmp.Y > sy-1 || tmp.Z < 0 || tmp.Z > 0)
					continue;
				
				curDist++;
				int brWorldIdx = tmp.X + (tmp.Y + tmp.Z * sy) * sx;
				
				try{
					if (PositionIsFree(tmp, world, sx, sy, sz) && brWorld[brWorldIdx] == false)
					{
						brWorld[brWorldIdx] = true;
						int pathCost = current.pathCost;// + surr.Cost;
						int cost = pathCost - tmp.GetDistanceSquared(pointToAvoid);
						SearchNode node = new SearchNode(tmp, cost, pathCost, current);
						openList.Add(node);
						allSearched.Add(node);
					}
				}
				catch{
					Debug.Log("brWorldIdx out of range: " + brWorldIdx + " max is: " + brWorld.GetLength(0) + " X: " + tmp.X + " Y: " + tmp.Y);
				}
			}
		}
		return null; //no path found
	}

	public static bool CanSeeFromTileToTile(EntityMain toSee, EntityMain watcher, Point3D tileToCheck, float checkRadius, LayerMask mask, bool CheckWiderArea)
	{
		Vector3 adjustedtoToSeePos = toSee.transform.position + Vector3.up*0.6f;
		Vector3 adjustedWatcherPos = 
			new Vector3(tileToCheck.X * MapGenerator.TileSize.x,
			           	0.6f,
			            tileToCheck.Y * MapGenerator.TileSize.z);

		Vector3 rightOffsetCheck = watcher.transform.right *0.5f;
		Vector3 leftOffsetCheck = -watcher.transform.right *0.5f;

		//adjustedWatcherPos += (adjustedtoToSeePos - adjustedWatcherPos).normalized * 2.8f;
		Ray[] rays ={ new Ray(adjustedWatcherPos, adjustedtoToSeePos - adjustedWatcherPos),
					  new Ray(adjustedWatcherPos+rightOffsetCheck, (adjustedtoToSeePos+rightOffsetCheck)-(adjustedWatcherPos+rightOffsetCheck)),
					  new Ray(adjustedWatcherPos+leftOffsetCheck, (adjustedtoToSeePos+leftOffsetCheck)-(adjustedWatcherPos+leftOffsetCheck))
		};

		//Color[] rayColors = { Color.magenta, Color.blue, Color.cyan };
		RaycastHit hitInfo;

		int checkCount = 1;
		if (CheckWiderArea)
			checkCount = 3;

		for (int i = 0; i < checkCount; i++)
		{
			//Debug.DrawLine(rays[i].origin, rays[i].origin + rays[i].direction*checkRadius, rayColors[i], 5.0f);
			if (Physics.Raycast(rays[i], out hitInfo, checkRadius, mask))
			{ 
				//ray hit the entity to avoid -> can see!
				if (hitInfo.transform == toSee.transform ||
				    (hitInfo.transform.parent != null && hitInfo.transform.parent == toSee.transform))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool CanHearFromTileToTile(EntityMain toHear, Point3D tileToCheck, float checkRadius, int wallTolerance, LayerMask mask)
	{
		Vector3 adjustedtoAvoidPos = toHear.transform.position + Vector3.up*0.6f;
		Vector3 adjustedAvoiderPos = 
			new Vector3(tileToCheck.X * MapGenerator.TileSize.x,
			            0.6f,
			            tileToCheck.Y * MapGenerator.TileSize.z);
		
		Ray ray = new Ray(adjustedAvoiderPos, adjustedtoAvoidPos - adjustedAvoiderPos);
		RaycastHit[] hits = Physics.RaycastAll(ray, checkRadius, mask);

		
		//Debug.DrawLine(ray.origin, ray.origin + ray.direction*checkRadius, Color.yellow, 5.0f);

		//check if toHear was hit at all and cache the distance
		float distanceToObject = 0;
		bool wasToHearHit = false;
		for (int i = 0; i < hits.Count(); i++)
		{
			if (hits[i].transform == toHear.transform||
		    	(hits[i].transform.parent != null && hits[i].transform.parent == toHear.transform))
			{
				wasToHearHit = true;
				distanceToObject = hits[i].distance; 
				break;
			}
		}

		if (!wasToHearHit)
			return false;

		//check number of walls between object to hear and guy listening
		int walls = 0;
		LayerMask wallLayer = LayerMask.NameToLayer("Wall");
		for (int i = 0; i < hits.Count(); i++)
		{
			if (hits[i].distance < distanceToObject && hits[i].transform.gameObject.layer == wallLayer)
			{
				//double checking for hitting two pieces of same continuous wall
				bool tooClose = false;
				for (int j = 0; j < i; j++)
				{
					if (Mathf.Abs(hits[i].distance - hits[j].distance) < 0.6f)
					{
						tooClose = true;
						break;
					}
				}

				if (!tooClose)
					walls++;
			}
		}

		if (walls <= wallTolerance)
		{
			//Debug.DrawLine(ray.origin, ray.origin + ray.direction*checkRadius, Color.green, 5.0f);
			return true;
		}
	
		return false;
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

        return !nextTile.BlockedForMovement;
    }

	private static bool PositionIsWalkable(Point3D position, TileMain[,] world, int mapWidth, int mapHeight, int mapDepth)
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

        if (nextTile.entityOnTile != null)
            return true;

		if (nextTile.Data.TileType==TileObjData.Type.Door){
			return true;
		}

        return !nextTile.BlockedForMovement;
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
