using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private GameObject damageTextPrefab;
    
    public void CreateDamageText(Vector3 position, int damage, bool isCritical)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(position);
        GameObject damageText = Instantiate(damageTextPrefab, pos, Quaternion.identity);
        if (damageText.TryGetComponent<DamageText>(out var damageTextComponent))
        {
            damageTextComponent.SetText(damage, isCritical);
        }

    }
}
