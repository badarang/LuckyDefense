using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject damageTextPrefab;
    public Dictionary<string, GameObject> UIDictionary = new Dictionary<string, GameObject>();
    public Dictionary<string, TextMeshProUGUI> UITextDictionary = new Dictionary<string, TextMeshProUGUI>();
    private WaitForSeconds oneSec = new WaitForSeconds(1f);


    private void Awake()
    {
        base.Awake();
        
        TextMeshProUGUI[] textMeshPros = canvas.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var textMeshPro in textMeshPros)
        {
            UITextDictionary.Add(textMeshPro.name, textMeshPro);
        }

        UIAnimationBase[] uiAnimationBases = canvas.GetComponentsInChildren<UIAnimationBase>();
        foreach (var uiAnimationBase in uiAnimationBases)
        {
            UIDictionary.Add(uiAnimationBase.name, uiAnimationBase.gameObject);
            uiAnimationBase.gameObject.SetActive(false);
        }
    }
    
    private void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        UIDictionary.Clear();
    }
    
    public void OnLoad()
    {
        Statics.DebugColor("UIManager Loaded", new Color(1, .5f, 1f));
    }
    
    private void Start()
    {
        GameManager.Instance.OnGameStart += () =>
        {
            SetAllUIText();
            StartCoroutine(ExpandAllUISequencially());
        };
    }
    
    private void SetAllUIText()
    {
        foreach (var ui in UIDictionary)
        {
            if (ui.Value.TryGetComponent<IUITextBase>(out var uiTextBase))
            {
                uiTextBase.ResetText();
            }
        }
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
    
    public void SetRoundTime(float time)
    {
        if (UITextDictionary.TryGetValue("RoundTimeText", out var roundTimeText))
        {
            StartCoroutine(SetRoundTimeCO(time, roundTimeText));
        }
    }
    
    private IEnumerator SetRoundTimeCO(float time, TextMeshProUGUI roundTimeText)
    {
        var remainTime = Mathf.CeilToInt(time);
        while (remainTime > 0)
        {
            int minutes = remainTime / 60;
            int seconds = remainTime % 60;
            roundTimeText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
            remainTime--;
            yield return oneSec;
        }
    }
    
    public void SetAliveEnemiesBar(int current, int max)
    {
        if (UIDictionary.TryGetValue("AliveEnemyContainer", out var aliveEnemyContainer))
        {
            if (aliveEnemyContainer.TryGetComponent<AliveEnemyContainer>(out var aliveEnemyBarComponent))
            {
                aliveEnemyBarComponent.SetBar(current,  max);
            }
        }
    }
    
    public void SetRoundText(int wave)
    {
        if (UITextDictionary.TryGetValue("CurrentWaveText", out var waveText))
        {
            waveText.text = $"Wave {wave}";
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
    
    public void ChangeGoldText(int gold)
    {
        if (UITextDictionary.TryGetValue("GoldText", out var goldText))
        {
            goldText.text = gold.ToString();
        }
    }
    public void ChangeRequiredGoldText(int gold)
    {
        if (UITextDictionary.TryGetValue("RequiredGoldAmountText", out var goldText))
        {
            goldText.text = gold.ToString();
        }
    }
}
