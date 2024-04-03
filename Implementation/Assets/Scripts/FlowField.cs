using System.Collections.Generic;
using UnityEngine;

public class FlowField
{
    private Cell[,] m_Cells;
	private float m_CellSize;
	private Cell m_DestinationCell;

	public Cell[,] Cells
	{
		get { return m_Cells; }
	}

	public Vector2Int GridSize
	{
		get { return new Vector2Int(m_Cells.GetLength(0), m_Cells.GetLength(1)); }
	}
	
	public float CellSize
	{
		get { return m_CellSize; }
	}

	public Cell DestinationCell
	{
		get { return m_DestinationCell; }
		set { m_DestinationCell = value; }
	}

	public FlowField(float cellSize, Vector2Int gridSize)
	{
        m_Cells = new Cell[gridSize.x, gridSize.y];
		m_CellSize = cellSize;
		m_DestinationCell = null;

		CreateCells();
		CreateCostField();
    }

	public void CreateCells()
	{
        for (int x = 0; x < m_Cells.GetLength(0); ++x)
        {
            for (int y = 0; y < m_Cells.GetLength(1); ++y)
            {
                Vector3 worldPos = new Vector3(m_CellSize * x + (m_CellSize / 2.0f), 0, m_CellSize * y + (m_CellSize / 2.0f));
				m_Cells[x, y] = new Cell(worldPos, new Vector2Int(x, y));
			}
		}
	}

	public void CreateCostField()
	{
		Vector3 cellHalfExtents = Vector3.one * (m_CellSize / 2.0f);
		int terrainMask = LayerMask.GetMask("Impassible", "RoughTerrain");

		foreach (Cell cell in m_Cells)
		{
			Collider[] obstacles = Physics.OverlapBox(cell.WorldPosition, cellHalfExtents, Quaternion.identity, terrainMask);
			bool hasIncreasedCost = false;

			foreach (Collider col in obstacles)
			{
				if (col.gameObject.layer == 8)
				{
					cell.Cost = 255;
					break;
				}
				else if (col.gameObject.layer == 9 && !hasIncreasedCost)
				{
					cell.Cost = 4;
					hasIncreasedCost = true;
				}
			}
		}
	}

	public void CreateIntegrationField(Cell destination)
	{
        m_DestinationCell = destination;
        m_DestinationCell.Cost = 0;
        m_DestinationCell.BestCost = 0;

		Queue<Cell> openList = new Queue<Cell>();
		openList.Enqueue(m_DestinationCell);

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
	}

	public void CreateFlowField()
	{
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
	}

	private List<Cell> GetNeighbours(Vector2Int index)
	{
		List<Cell> neighborCells = new List<Cell>();

		// Check for edge cases with the x index
		bool hasNeighboursOnTheRight = true;
        bool hasNeighboursOnTheLeft = true;
		if(index.x == m_Cells.GetLength(0) - 1) hasNeighboursOnTheRight = false;
		else if(index.x == 0) hasNeighboursOnTheLeft = false;

        // Check for edge cases with the y index
        bool hasNeighboursAbove = true;
		bool hasNeighboursBelow = true;
        if(index.y == m_Cells.GetLength(1) - 1) hasNeighboursAbove = false;
        else if(index.y == 0) hasNeighboursBelow = false;

		if(hasNeighboursOnTheRight)
		{
			if(hasNeighboursAbove) neighborCells.Add(m_Cells[index.x + 1, index.y + 1]);        // Top right
            neighborCells.Add(m_Cells[index.x + 1, index.y]);									// Middle right
            if(hasNeighboursBelow) neighborCells.Add(m_Cells[index.x + 1, index.y - 1]);		// Bottom right
        }
		if(hasNeighboursAbove) neighborCells.Add(m_Cells[index.x, index.y + 1]);				// Top middle
        if(hasNeighboursBelow) neighborCells.Add(m_Cells[index.x, index.y - 1]);				// Bottom middle
        if(hasNeighboursOnTheLeft)
        {	
            if(hasNeighboursAbove) neighborCells.Add(m_Cells[index.x - 1, index.y + 1]);		// Top left
            neighborCells.Add(m_Cells[index.x - 1, index.y]);									// Middle left
            if(hasNeighboursBelow) neighborCells.Add(m_Cells[index.x - 1, index.y - 1]);		// Bottom left
        }

        return neighborCells;
	}

	public Cell GetCell(Vector3 worldPos)
	{
		float percentX = worldPos.x / (m_Cells.GetLength(0) * m_CellSize);
		float percentY = worldPos.z / (m_Cells.GetLength(1) * m_CellSize);

		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.Clamp(Mathf.FloorToInt((m_Cells.GetLength(0)) * percentX), 0, m_Cells.GetLength(0) - 1);
		int y = Mathf.Clamp(Mathf.FloorToInt((m_Cells.GetLength(1)) * percentY), 0, m_Cells.GetLength(1) - 1);
		return m_Cells[x, y];
	}
}
