using Units;
using UnityEngine;

public class View : MonoBehaviour
{
    
    public void Start()
    {
       var unitData = new UnitData();
       unitData.OnChanged.AddListener((data) => Debug.Log("Change" + data.AttackDamage + " " + data.CurrentHP));
       unitData.AttackDamage.Value = 3;
       unitData.CurrentHP.Value = 3;

    }
}
