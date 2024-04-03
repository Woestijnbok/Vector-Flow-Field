using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridController : MonoBehaviour
{
	[SerializeField] private Vector2Int m_GridSize = new Vector2Int( 40, 22);
	[SerializeField] private float m_CellSize = 1.0f;
    [SerializeField] private GridDebug m_GridDebug;
    private FlowField m_FlowField;

	public FlowField Field
	{
		get { return m_FlowField;  }
	}

    public Vector2Int GridSize
    {
        get { return m_GridSize; }
    }

	public float CellSize
	{
		get { return m_CellSize; }
	}

    private void Start()
	{
        if (m_GridDebug == null) Debug.LogWarning("No grid debug object stored in grid controller");
        m_FlowField = new FlowField(m_CellSize, m_GridSize);
    }

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			m_FlowField.CreateCells();
			m_FlowField.CreateCostField();
			m_FlowField.CreateIntegrationField(m_FlowField.GetCell(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
			m_FlowField.CreateFlowField();
		}

		m_GridDebug.Draw();
    }
}
