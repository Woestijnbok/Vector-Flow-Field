# Introduction to pathfinding

There are many pathfinding algorithms used for ai in games. Some of the more common ones are A star, Dijkstra and Breadth first search. The main problem these algorithms face is to account for their surroundings. The speed and complexity (big O notation) of these algorithms play a big role in chosing pathfinding algorithms. Should we use the same algorithms when working with multiple units?

<div align="center">
  <img src=https://github.com/Howest-DAE-GD/gpp-researchtopic-Woestijnbok/blob/main/Readme%20Resources/A%20star.gif width=250 height=auto>
  <img src=https://github.com/Howest-DAE-GD/gpp-researchtopic-Woestijnbok/blob/main/Readme%20Resources/Breath%20first%20search.gif width=250 height=auto>
  <img src=https://github.com/Howest-DAE-GD/gpp-researchtopic-Woestijnbok/blob/main/Readme%20Resources/Dijkstra.gif width=250 height=auto>
</div>

# Vector flow field

Answering the question of the previous paragraph, one of the main algorithms when working with multiple units is the vector flow field. Of course there is no indefinite answer to choosing algorithms. Pathfinding algorithms heavely depend on your situation / use case. But one of the major plus points in a flow field also know as vector field is that you only calculate the "path" once instead for every unit. The main principle of this algorithm is deviding the world into a grid. After the creation of the grid will make a heatmap for this grid. With this heatmap we can generate the direction map (hence the name vector field).

# Distance in a grid

When generating the heat map we will need to calculate the distance between two cells. There are many ways to calculate this some are "better" then others in certain cases. The main three are Tchebychev distance, Euclidian distance and the Manhattan distance. Following picture showcase the difference in each of them using a 2 dimensional grid.

<div align="center">
	<img src=https://github.com/Howest-DAE-GD/gpp-researchtopic-Woestijnbok/blob/main/Readme%20Resources/Distance.png>
</div>

# Heat map

Now we know how to calculate the distance we can start caclulating the heat map. In games certain terrain is more costly to pass then other terrain for example mud instead of grass. so when devide or world into a grid each cell can take this cost into acount. Following picture shows an example, the black indicates inpassible and orange is more costly then green.

<div align="center">
	<img src=https://github.com/Howest-DAE-GD/gpp-researchtopic-Woestijnbok/blob/main/Readme%20Resources/Cost%20Map.png>
</div>

The previous step can be ignored when each terrain has the same cost (flat field full of grass for example). Now comes the main algorithm, the principle is that we have an open list with cells to check and a closed list with already checked cells.
We start by adding the destination cell to the open list. While we have cells left in the open list we do the following. Get and remove a cell from the open list, will call that cell the current cell. Get the cells around the current cell, will call them the neighbour cells. For each neighbour node we execute the following logic.
	
```cs
	while(openList.Count > 0)
	{
		Cell current = openList.Dequeue();
  
		foreach (Cell neighbour in GetNeighbours(current.Index))	
		{
			if (neighbour.Cost == byte.MaxValue) continue;		
			else if (neighbour.Cost + current.BestCost < neighbour.BestCost)	
			{
				neighbour.BestCost = (ushort)(neighbour.Cost + current.BestCost);
				openList.Enqueue(neighbour);
			}
		}
	}

	// Note that there is no distance caclulations here since we are using chebychev distance
 ```

<div align="center">
   <img src=https://github.com/Howest-DAE-GD/gpp-researchtopic-Woestijnbok/blob/main/Readme%20Resources/Heat%20Map.png>
</div>

# Direction map

To generate the direction map we will use a technique called kernel convolution. This technique simplified is adapting your own information based on your surrounding. In our case changing our current direction (of the cell) based on its neighbours (who are one cell away). I choose for very simple logic, the direction stored in cell x is the direction from cell x to its neighbour cell y with the lowest cost.

```cs
	foreach(Cell current in m_Cells)
	{
		ushort bestCost = ushort.MaxValue;
		Cell target = null;

    		foreach (Cell neighbour in GetNeighbours(current.Index))
	   	 {
		        if (neighbour.BestCost < bestCost) 
			{
		            bestCost = neighbour.BestCost;
					target = neighbour;
		        }
		}
	
		if(target == null || target.WorldPosition == null)
		{
			current.Direction = (m_DestinationCell.WorldPosition - current.WorldPosition).normalized;
	    	}
		else 
		{
	        current.Direction = (target.WorldPosition - current.WorldPosition).normalized;
		}
	}

	// Note that there is no distance caclulations here since we are using chebychev distance
 ```

 <div align="center">
  <img src=https://github.com/Howest-DAE-GD/gpp-researchtopic-Woestijnbok/blob/main/Readme%20Resources/Direction%20Map.png>
</div>

# Conclusion

I found it a very fun algorithm to implement escpially in a 2D games since it makes it quite a bit easier. Of course my implemention can have a lot of imporvements. Instead of the direction being towards the lowest cost neighbour and thus completly ignoring the other neighbours. I could have instead used an other pattern / function who makes more use of the other neighbours. And in the future I will definetly use the different ways to calculate the distance since chebychev is very basic.


# Sources

PDN-PasDeNom: [How do vector field Pathfinding algorithm work?](https://www.youtube.com/watch?v=ZJZu3zLMYAc&t=0s&ab_channel=PDN-PasDeNom)

Turbo Makes Games: [How Flow Field Pathfinding Works - Flow Fields in Unity ep. 1](https://www.youtube.com/watch?v=zr6ObNVgytk&t=0s&ab_channel=TurboMakesGames)
