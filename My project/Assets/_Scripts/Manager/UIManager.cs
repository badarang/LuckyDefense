using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject damageTextPrefab;
    public Dictionary<string, GameObject> UIDictionary = new Dictionary<string, GameObject>();
    public Dictionary<string, TextMeshProUGUI> UITextDictionary = new Dictionary<string, TextMeshProUGUI>();
    private WaitForSeconds oneSec = new WaitForSeconds(1f);
    private float enemyAlertDelay;
    private Queue<GameObject> displayedGUIQueue = new Queue<GameObject>();
    
    private Queue<UnitTypeEnum> congratQueue = new Queue<UnitTypeEnum>();
    private bool isCongratProcessing = false;


    private void Awake()
    {
        base.Awake();
    
        TextMeshProUGUI[] textMeshPros = canvas.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var textMeshPro in textMeshPros)
        {
            if (UITextDictionary.ContainsKey(textMeshPro.name)) continue;
            UITextDictionary.Add(textMeshPro.name, textMeshPro);
        }

        UIAnimationBase[] uiAnimationBases = canvas.GetComponentsInChildren<UIAnimationBase>(true);
        foreach (var uiAnimationBase in uiAnimationBases)
        {
            UIDictionary.Add(uiAnimationBase.name, uiAnimationBase.gameObject);
            uiAnimationBase.gameObject.SetActive(false);
        }
    }

    private void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        UITextDictionary.Clear();
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
        
        GameManager.Instance.OnGameEnd += () =>
        {
            StopAllCoroutines();
            PopGUIQueue(false);
        };
    }
    
    private void Update()
    {
        if (enemyAlertDelay > 0f)
        {
            enemyAlertDelay -= Time.deltaTime;
        }
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
            if (ui.Value.TryGetComponent<UIAnimationBase>(out var uiAnimationBase))
            {
                if (!uiAnimationBase.IsExpandedFirst) continue;
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
        GameObject damageText = PoolManager.Instance.GetDisplayText();
        damageText.transform.position = position + offset;
        if (damageText.TryGetComponent<DisplayText>(out var damageTextComponent))
        {
            damageTextComponent.Init();
            damageTextComponent.SetText(damage, isCritical);
        }
    }

    public void CreateDisplayText(string str, Color color, Vector3 position)
    {
        GameObject displayText = PoolManager.Instance.GetDisplayText();
        displayText.transform.position = position;
        if (displayText.TryGetComponent<DisplayText>(out var displayTextComponent))
        {
            displayTextComponent.Init();
            displayTextComponent.SetText(str, color);
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
    public void ChangeRequiredGoldTextColor()
    {
        if (GoodsManager.Instance.Gold >= GoodsManager.Instance.RequiredSummonGold)
        {
            UITextDictionary["RequiredGoldAmountText"].color = Color.white;
        }
        else
        {
            UITextDictionary["RequiredGoldAmountText"].color = Color.red;
        }
    }
    
    public void ChangeDiamondText(int diamond)
    {
        if (UITextDictionary.TryGetValue("DiamondText", out var diamondText))
        {
            diamondText.text = diamond.ToString();
        }
    }
    
    public void ChangeUnitCountText(int aliveUnits)
    {
        if (UITextDictionary.TryGetValue("UnitCountText", out var aliveUnitsText))
        {
            aliveUnitsText.text = $"{aliveUnits}/{Statics.InitialGameDataDic["MaxUnitCount"]}";
        }
    }
    
    public void ShowUnitInfo(Unit unit, int unitNum)
    {
        if (UIDictionary.TryGetValue("UnitInfo", out var unitInfoPanel))
        {
            if (unitInfoPanel.TryGetComponent<UnitInfo>(out var unitInfoPanelComponent))
            {
                unitInfoPanelComponent.SetUnitInfo(unit, unitNum);
                unitInfoPanel.GetComponent<UIAnimationBase>().Expand();
            }
        }
    }

    public void HideUnitInfo()
    {
        if (UIDictionary.TryGetValue("UnitInfo", out var unitInfoPanel))
        {
            unitInfoPanel.GetComponent<UIAnimationBase>().Shrink();
        }
    }
    
    public void ToggleWaveDisplayPanel(bool isOn)
    {
        if (UIDictionary.TryGetValue("WaveDisplayPanel", out var waveDisplayPanel))
        {
            if (isOn)
            {
                UITextDictionary["WaveDisplayText"].text = $"Wave {RoundManager.Instance.currentRound}";
                waveDisplayPanel.GetComponent<UIAnimationBase>().ExpandLikeTV();
                UITextDictionary["WaveDisplayText"].GetComponent<TextAnimationBase>().ExpandAlertDelayed(.5f);
            }
            else
            {
                waveDisplayPanel.GetComponent<UIAnimationBase>().ShrinkLikeTV();
            }
        }
    }
    
    public void ShowEnemyAlertPanel()
    {
        if (UIDictionary.TryGetValue("EnemyAlertPanel", out var enemyAlertPanel))
        {
            if (enemyAlertDelay > 0f) return;
            enemyAlertDelay = 20f;
            if (enemyAlertPanel.TryGetComponent(out UIAnimationBase uiAnimationBase))
            {
                uiAnimationBase.Expand();
                StartCoroutine(ShrinkAfterDelay(uiAnimationBase, 2f));
            }
        }
    }

    private IEnumerator ShrinkAfterDelay(UIAnimationBase uiAnimationBase, float delay)
    {
        yield return new WaitForSeconds(delay);
        uiAnimationBase.Shrink();
    }
    
    public void PushGUIQueue(GameObject gui)
    {
        displayedGUIQueue.Enqueue(gui);
    }
    
    public void PopGUIQueue(bool setActive)
    {
        if (displayedGUIQueue.Count > 0)
        {
            while (displayedGUIQueue.Count > 0)
            {
                var gui = displayedGUIQueue.Dequeue();
                //GUI가 이미 파괴된 경우
                if (gui == null) continue;
                if (TryGetComponent(out UIAnimationBase uiAnimationBase))
                {
                    if (setActive) uiAnimationBase.Expand();
                    else uiAnimationBase.Shrink();
                    return;
                }
                gui.SetActive(setActive);
            }
        }
    }

    
    public void ShowConfetti(UnitTypeEnum unitType)
    {
        switch (UnitManager.Instance.GetUnitGrade(unitType))
        {
            case Grade.Common:
                break;
            case Grade.Rare:
                break;
            case Grade.Epic:
                ShowUnitCongratPanel(unitType);
                break;
            case Grade.Mythic:
                ShowUnitCongratPanel(unitType);
                break;
        }
    }
    
    private void ShowUnitCongratPanel(UnitTypeEnum unitType)
    {
        congratQueue.Enqueue(unitType);
        if (!isCongratProcessing)
        {
            StartCoroutine(ProcessCongratQueue());
        }
    }
    
    private IEnumerator ProcessCongratQueue()
    {
        isCongratProcessing = true;
        while (congratQueue.Count > 0)
        {
            UnitTypeEnum currentType = congratQueue.Dequeue();
            yield return StartCoroutine(ShowUnitCongratPanelCO(currentType));
        }
        isCongratProcessing = false;
    }
    
    private IEnumerator ShowUnitCongratPanelCO(UnitTypeEnum unitType)
    {
        if (UnitManager.Instance.GetUnitGrade(unitType) == Grade.Mythic)
        {
            ParticleManager.Instance.SpawnParticle(ParticleType.VFX_Shine_Orange, Vector3.zero);
            ParticleManager.Instance.SpawnParticle(ParticleType.VFX_Star_Confetti, Vector3.zero);
        }
        else if (UnitManager.Instance.GetUnitGrade(unitType) == Grade.Epic)
        {
            ParticleManager.Instance.SpawnParticle(ParticleType.VFX_Shine_Purple, Vector3.zero);
        }
        
        if (UIDictionary.TryGetValue("UnitCongratPanel", out var unitCongratPanel))
        {
            if (unitCongratPanel.TryGetComponent<UnitCongratPanel>(out var unitCongratPanelComponent))
            {
                var sprite = UnitManager.Instance.GetUnitSprite(unitType);
                var grade = UnitManager.Instance.GetUnitGrade(unitType);
                var name = UnitManager.Instance.GetUnitName(unitType);

                unitCongratPanelComponent.SetUnitCongratPanel(grade, name, sprite);
                Sequence seq = unitCongratPanelComponent.StartAnimation();
                unitCongratPanel.SetActive(true);
                yield return seq.WaitForCompletion();
                yield return new WaitForSeconds(0.5f);
                unitCongratPanel.SetActive(false);
            }
        }
        yield return null;
    }
    
    public void ShowGameClearPanel()
    {
        if (UIDictionary.TryGetValue("GameClearPanel", out var gameClearPanel))
        {
            gameClearPanel.GetComponent<GameEndPanel>().Init();
            gameClearPanel.GetComponent<UIAnimationBase>().Expand();
        }
    }
    
    public void ShowGameOverPanel()
    {
        if (UIDictionary.TryGetValue("GameOverPanel", out var gameOverPanel))
        {
            gameOverPanel.GetComponent<GameEndPanel>().Init();
            gameOverPanel.GetComponent<UIAnimationBase>().Expand();
        }
    }
}
