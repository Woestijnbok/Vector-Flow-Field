using UnityEngine;

public class Cell
{
	private Vector3 m_WorldPosition;
	private Vector2Int m_Index;
	private byte m_Cost;
	private ushort m_BestCost;
	private Vector3 m_Direction;

    public Vector3 WorldPosition
	{
		get { return m_WorldPosition; }
		set { m_WorldPosition = value; }
	}

	public Vector2Int Index
	{
		get { return m_Index; }
		set { m_Index = value; }
	}

	public byte Cost
	{
		get { return m_Cost; }
		set { m_Cost = value; }
	}

    public ushort BestCost
    {
        get { return m_BestCost; }
        set { m_BestCost = value; }
    }

    public Vector3 Direction
	{
		get { return m_Direction; }
		set { m_Direction = value; }
	}

	public Cell(Vector3 worldPosition, Vector2Int index)
	{
		m_WorldPosition = worldPosition;
		m_Index = index;
		m_Cost = 1;
		m_BestCost = ushort.MaxValue;	
		m_Direction = Vector3.zero;
	}
}