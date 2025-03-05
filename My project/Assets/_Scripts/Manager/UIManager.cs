using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject damageTextPrefab;
    public Dictionary<string, GameObject> UIDictionary = new Dictionary<string, GameObject>();

    private void Awake()
    {
        base.Awake();
        
        UIAnimationBase[] uiAnimationBases = canvas.GetComponentsInChildren<UIAnimationBase>();
        foreach (var uiAnimationBase in uiAnimationBases)
        {
            UIDictionary.Add(uiAnimationBase.name, uiAnimationBase.gameObject);
            uiAnimationBase.gameObject.SetActive(false);
        }
    }
    
    private void Start()
    {
        GameManager.Instance.OnGameStart += () =>
        {
            StartCoroutine(ExpandAllUISequencially());
        };
    }
    private IEnumerator ExpandAllUISequencially()
    {
        foreach (var ui in UIDictionary)
        {
            ui.Value.SetActive(true);
            if (ui.Value.TryGetComponent<UIAnimationBase>(out var uiAnimationBase))
            {
                uiAnimationBase.Expand();
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    
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
