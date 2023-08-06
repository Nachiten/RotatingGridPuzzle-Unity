using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }

    private List<Unit> units;
    private List<Unit> friendlyUnits;
    private List<Unit> enemyUnits;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one instance of UnitActionSystem found!");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        units = new List<Unit>();
        friendlyUnits = new List<Unit>();
        enemyUnits = new List<Unit>();
    }

    private void Start()
    {
        Unit.OnAnyUnitSpawned += OnAnyUnitSpawned;
        Unit.OnAnyUnitDead += OnAnyUnitDead;
    }

    private void OnAnyUnitSpawned(Unit unit)
    {
        units.Add(unit);

        if (unit.IsEnemy())
            enemyUnits.Add(unit);
        else
            friendlyUnits.Add(unit);
    }

    private void OnAnyUnitDead(Unit unit)
    {
        units.Remove(unit);

        if (unit.IsEnemy())
            enemyUnits.Remove(unit);
        else
            friendlyUnits.Remove(unit);
    }

    public List<Unit> GetUnits()
    {
        return units;
    }

    public List<Unit> GetFriendlyUnits()
    {
        return friendlyUnits;
    }

    public List<Unit> GetEnemyUnits()
    {
        return enemyUnits;
    }
}