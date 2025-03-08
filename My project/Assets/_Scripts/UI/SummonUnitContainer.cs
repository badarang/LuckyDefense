using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SummonUnitContainer : MonoBehaviour, IUITextBase
{
    [SerializeField] private TextMeshProUGUI requiredGoldText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button summonButton;
    private WaitForSeconds twoSec = new WaitForSeconds(2f);

    private float shineLocation = 0;

    public void ResetText()
    {
        requiredGoldText.text = GoodsManager.Instance.RequiredSummonGold.ToString();
    }

    private void OnEnable()
    {
        StartShineUI();
        summonButton.onClick.AddListener(OnClickSummonBtn);
    }

    private void StartShineUI()
    {
        StartCoroutine(ShineUI());
    }

    private IEnumerator ShineUI()
    {
        while (true)
        {
            yield return LerpShine(0, 1, 0.5f);
            yield return twoSec;
        }
    }

    private IEnumerator LerpShine(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            shineLocation = Mathf.Lerp(from, to, elapsed / duration);
            iconImage.material.SetFloat("_ShineLocation", shineLocation);
            yield return null;
        }
    }
    
    private void OnClickSummonBtn()
    {
        if (GoodsManager.Instance.Gold >= GoodsManager.Instance.RequiredSummonGold)
        {
            GoodsManager.Instance.Gold -= GoodsManager.Instance.RequiredSummonGold;
            GoodsManager.Instance.IncreaseRequiredSummonGold();
            UIManager.Instance.ChangeRequiredGoldText(GoodsManager.Instance.RequiredSummonGold);
            UnitManager.Instance.SummonUnit(isMyPlayer: true);
        }
    }
}