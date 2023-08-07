using System;
using Unity.XR.OpenVR;
using UnityEngine;

public struct GridPosition : IEquatable<GridPosition>
{
    public int x;
    public int z;
    public int floor;

    public GridPosition(int x, int z, int floor)
    {
        this.x = x;
        this.z = z;
        this.floor = floor;
    }

    public override string ToString()
    {
        return $"{x}, {z}\nFloor: {floor}";
    }

    public static bool operator ==(GridPosition a, GridPosition b)
    {
        return a.x == b.x && a.z == b.z && a.floor == b.floor;
    }

    public static bool operator !=(GridPosition a, GridPosition b)
    {
        return !(a == b);
    }

    public bool Equals(GridPosition other)
    {
        return this == other;
    }

    public override bool Equals(object obj)
    {
        return obj is GridPosition other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, z);
    }

    public static GridPosition operator +(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x + b.x, a.z + b.z, a.floor + b.floor);
    }

    public static GridPosition operator -(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x - b.x, a.z - b.z, a.floor - b.floor);
    }

    public int Distance(GridPosition other)
    {
        return Math.Abs(x - other.x) + Math.Abs(z - other.z) + Math.Abs(floor - other.floor);
    }
    
    public bool FloorIsValid(int totalFloors)
    {
        return floor >= 0 && floor < totalFloors;
    }
    
    public Vector2Int ToVector2Int()
    {
        return new Vector2Int(x, z);
    }
}