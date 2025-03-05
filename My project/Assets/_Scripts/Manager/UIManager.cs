using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject damageTextPrefab;
    
    public void CreateDamageText(Vector3 position, int damage, bool isCritical)
    {
        var offset = new Vector3(Random.Range(-0.1f, 0.1f), 1f, 0);
        GameObject damageText = Instantiate(damageTextPrefab, position + offset, Quaternion.identity);
        if (damageText.TryGetComponent<DamageText>(out var damageTextComponent))
        {
            damageTextComponent.Init();
            damageTextComponent.SetText(damage, isCritical);
        }

    }
}
