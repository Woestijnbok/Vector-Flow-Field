using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField] private GridController m_GridController;
    [SerializeField] private GameObject m_Unit;
    [SerializeField] private int m_UnitsPerSpawn = 50;
    [SerializeField] private float m_UnitSpeed = 3.0f;
    private List<GameObject> m_Units = new List<GameObject>();

	private void Start()
	{
		if (m_GridController == null) Debug.LogWarning("No grid controller object stored in the unit controller");
        if (m_Unit == null) Debug.LogWarning("No game unit game object stored in the unit controller");
    }

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1)) SpawnUnits();
		else if (Input.GetKeyDown(KeyCode.Alpha2)) DestroyUnits();
	}

	private void FixedUpdate()
	{
		foreach (GameObject unit in m_Units)
		{
			if(unit.TryGetComponent<Rigidbody>(out Rigidbody rigidBody))
			{
				rigidBody.velocity = m_GridController.Field.GetCell(unit.transform.position).Direction * m_UnitSpeed;
			}
		}
	}

	private void SpawnUnits()
	{
		Vector2Int gridSize = m_GridController.Field.GridSize;
		float nodeRadius = m_GridController.Field.CellSize / 2.0f;
		float nodeSize = m_GridController.Field.CellSize;

		Vector3 maxSpawnPosition = new Vector3(gridSize.x * nodeSize + nodeRadius, 0.0f, gridSize.y * nodeSize + nodeRadius);
        Vector3 minSpawnPosition = new Vector3(0.0f, 0.0f, 0.0f);

        int collisionMask = LayerMask.GetMask("Impassible", "Units");

		for (int i = 0; i < m_UnitsPerSpawn; ++i)
		{
			GameObject unit = Instantiate(m_Unit);
			unit.transform.parent = transform;
			m_Units.Add(unit);
            Vector3 spwanPosition = Vector3.zero;

            do
			{
                spwanPosition = new Vector3
				(
					Random.Range(minSpawnPosition.x, maxSpawnPosition.x), 
					Random.Range(minSpawnPosition.y, maxSpawnPosition.y),
                    Random.Range(minSpawnPosition.z, maxSpawnPosition.z)
                );
				unit.transform.position = spwanPosition;
			}
			while (Physics.OverlapSphere(spwanPosition, 0.25f, collisionMask).Length > 0);
		}
	}

	private void DestroyUnits()
	{
		foreach (GameObject unit in m_Units)
		{
			Destroy(unit);
		}

		m_Units.Clear();
	}
}
