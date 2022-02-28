using System.Collections;
using System.Collections.Generic;
using Units;
using UnityEngine;

public class Player : MonoBehaviour, IUnit
{
    public UnitData Attributes { get; } = new UnitData();
}
