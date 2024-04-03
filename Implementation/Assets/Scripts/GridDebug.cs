using UnityEditor;
using UnityEngine;

public enum DrawMode
{
    None,
    CostField,
    IntegrationField,
    FlowField
};

public class GridDebug : MonoBehaviour
{
    [SerializeField] private GridController m_GridController;
    [SerializeField] private DrawMode m_DrawMode = DrawMode.FlowField;
    private bool m_Awake = false;

    private void Awake()
    {
        if (m_GridController == null) Debug.LogWarning("no grid controller stored in grid debug");
        m_Awake = true;
    }

    private void OnGUI()
    {
        if (m_Awake == false) return;

        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        switch (m_DrawMode)
        {
            case DrawMode.CostField:
                foreach (Cell curCell in m_GridController.Field.Cells)
                {
                    if (curCell.Cost >= byte.MaxValue) Handles.Label(curCell.WorldPosition, "x", GUI.skin.label);
                    else Handles.Label(curCell.WorldPosition, curCell.Cost.ToString(), GUI.skin.label);
                }
                break;
            case DrawMode.IntegrationField:
                foreach (Cell curCell in m_GridController.Field.Cells)
                {
                    if (curCell.BestCost >= ushort.MaxValue) Handles.Label(curCell.WorldPosition, "x", GUI.skin.label);
                    else Handles.Label(curCell.WorldPosition, curCell.BestCost.ToString(), GUI.skin.label);
                }
                break;
        }
    }

    private void OnDrawGizmos()
    {
        if (m_Awake == false) return;

        switch (m_DrawMode)
        {
            case DrawMode.CostField:
            case DrawMode.IntegrationField:
            case DrawMode.FlowField:
                Gizmos.color = UnityEngine.Color.red;
                foreach (Cell cell in m_GridController.Field.Cells)
                {
                    Gizmos.DrawWireCube(cell.WorldPosition, Vector3.one * m_GridController.CellSize);
                }
                break;
        }
    }

    public void Draw()
    {
        if (m_Awake == false) return;
        
        switch (m_DrawMode)
        {
            case DrawMode.FlowField:
                foreach (Cell cell in m_GridController.Field.Cells)
                {
                    if (cell.Cost == 0) DebugDestination(cell); // Destination cell
                    else if (cell.Cost == byte.MaxValue) DebugImpassible(cell); // Impassible cell
                    else DebugDirection(cell); // Normal cell
                }
                break;
        }
    }

    private void DebugDirection(Cell cell)
    {
        Vector3 start = cell.WorldPosition - ((m_GridController.CellSize / 4.0f) * cell.Direction);
        Vector3 middle = cell.WorldPosition;
        Vector3 end = cell.WorldPosition + ((m_GridController.CellSize / 4.0f) * cell.Direction);

        Debug.DrawLine(start, middle, UnityEngine.Color.blue);
        Debug.DrawLine(middle, end, UnityEngine.Color.cyan);
    }

    private void DebugDestination(Cell cell)
    {
        float cellRadius = (m_GridController.CellSize / 2.0f);

        Vector3 down = cell.WorldPosition - (cellRadius * Vector3.forward);
        Vector3 up = cell.WorldPosition + (cellRadius * Vector3.forward);
        Debug.DrawLine(down, up, UnityEngine.Color.green);

        Vector3 left = cell.WorldPosition - (cellRadius * Vector3.right);
        Vector3 right = cell.WorldPosition + (cellRadius * Vector3.right);
        Debug.DrawLine(left, right, UnityEngine.Color.green);

        Vector3 bottomleft = new Vector3(cell.WorldPosition.x - cellRadius, cell.WorldPosition.y, cell.WorldPosition.z - cellRadius);
        Vector3 topright = new Vector3(cell.WorldPosition.x + cellRadius, cell.WorldPosition.y, cell.WorldPosition.z + cellRadius);
        Debug.DrawLine(topright, bottomleft, UnityEngine.Color.green);

        Vector3 topleft = new Vector3(cell.WorldPosition.x - cellRadius, cell.WorldPosition.y, cell.WorldPosition.z + cellRadius);
        Vector3 bottomright = new Vector3(cell.WorldPosition.x + cellRadius, cell.WorldPosition.y, cell.WorldPosition.z - cellRadius);
        Debug.DrawLine(topleft, bottomright, UnityEngine.Color.green);
    }

    private void DebugImpassible(Cell cell)
    {
        float cellRadius = (m_GridController.CellSize / 2.0f);

        Vector3 topright = new Vector3(cell.WorldPosition.x + cellRadius, cell.WorldPosition.y, cell.WorldPosition.z + cellRadius);
        Vector3 bottomleft = new Vector3(cell.WorldPosition.x - cellRadius, cell.WorldPosition.y, cell.WorldPosition.z - cellRadius);
        Debug.DrawLine(topright, bottomleft, UnityEngine.Color.black);

        Vector3 topleft = new Vector3(cell.WorldPosition.x - cellRadius, cell.WorldPosition.y, cell.WorldPosition.z + cellRadius);
        Vector3 bottomright = new Vector3(cell.WorldPosition.x + cellRadius, cell.WorldPosition.y, cell.WorldPosition.z - cellRadius);
        Debug.DrawLine(topleft, bottomright, UnityEngine.Color.black);
    }
}
