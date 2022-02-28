using TMPro;
using UnityEngine;

namespace Units
{
	[RequireComponent(typeof(IUnit))]
	public class HealthBar : MonoBehaviour
	{
		[SerializeField] private GameObject bar;
		[SerializeField] private SpriteRenderer barImg;
		[SerializeField] private TMP_Text text;
		
		private float maxHP;

		private void Awake()
		{
			var unit = GetComponent<IUnit>();
			maxHP = unit.Attributes.MaxHP.Value;
			unit.Attributes.CurrentHP.OnChanged.AddListener(OnHPChange);
			OnHPChange(unit.Attributes.CurrentHP.Value);
		}

		public void OnDeath()
		{
			bar.SetActive(false);
		}

		private void LateUpdate()
		{
			bar.transform.rotation = Camera.main.transform.rotation;
		}

		private void OnHPChange(float health)
		{
			var frac = health / maxHP;
			text.text = $"{health:####}/{maxHP:####}";
			barImg.size = new Vector2(frac, barImg.size.y);
			var pos = barImg.transform.localPosition;
			pos.x = -(1 - frac) / 2;
			barImg.transform.localPosition = pos;
		}
	}
}