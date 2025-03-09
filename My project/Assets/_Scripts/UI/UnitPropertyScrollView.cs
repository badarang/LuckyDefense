using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPropertyScrollView : MonoBehaviour
{
    [SerializeField] private RectTransform scrollView;
    [SerializeField] private RectTransform content;
    
    [SerializeField] 
    public GameObject unitPropertyPrefab;

    public void ChangeProperty(UnitPropertyEnum propertyEnum, float value)
    {
        int idx = propertyEnum.GetHashCode();
        if (idx < 0 || idx >= content.transform.childCount) return;
        
        var child = content.GetChild(idx);

        child.gameObject.SetActive(value > .01f);

        var container = child.GetComponent<UnitPropertyIconContainer>();
        container.SetValue(value);

        SetSize();
    }
    
    public void SetSize()
    {
        int activeCount = 0;
        for (int i = 0; i < content.transform.childCount; i++)
        {
            var child = content.transform.GetChild(i);
            if (child.gameObject.activeSelf)
            {
                activeCount++;
            }
        }
        scrollView.sizeDelta = new Vector2(100 * activeCount, scrollView.sizeDelta.y);
    }

    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
