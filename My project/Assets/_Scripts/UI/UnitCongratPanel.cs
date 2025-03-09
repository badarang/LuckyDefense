using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitCongratPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI unitGrade;
    [SerializeField] private TextMeshProUGUI unitName;
    [SerializeField] private Image unitImage;

    public void SetUnitCongratPanel(Grade grade, string unitName, Sprite unitSprite)
    {
        var gradeText = grade == Grade.Mythic ? "신화 등장" : "영웅 등장";
        unitGrade.text = gradeText;
        unitGrade.color = grade == Grade.Mythic ? new Color(1, 0.7f, 0) : new Color(0.6f, 0.4f, 1);
        this.unitName.text = unitName;
        unitImage.sprite = unitSprite;
    }
    
    public Sequence StartAnimation()
    {
        unitGrade.rectTransform.DOKill();
        unitName.rectTransform.DOKill();
        unitImage.rectTransform.DOKill();
        
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        canvasGroup.alpha = 1;

        var gradeOriginalPos = new Vector3(0f, 330f, -10f);
        var nameOriginalPos = new Vector3(0f, 0f, -10f);
        var imageOriginalScale = Vector3.one;

        unitGrade.rectTransform.localPosition = gradeOriginalPos + new Vector3(300f, 0f, 0f);
        unitName.rectTransform.localPosition = nameOriginalPos - new Vector3(300f, 0f, 0f);
        unitImage.rectTransform.localScale = imageOriginalScale;

        Sequence sequence = DOTween.Sequence();

        RectTransform gradeRect = unitGrade.rectTransform;
        sequence.Append(gradeRect.DOLocalMove(gradeOriginalPos, 1f).SetEase(Ease.OutBack));
        sequence.AppendInterval(0.7f);
        sequence.Append(gradeRect.DOLocalMove(gradeOriginalPos + new Vector3(-100f, 0f, 0f), 0.3f).SetEase(Ease.InBack));

        RectTransform nameRect = unitName.rectTransform;
        sequence.Insert(0, nameRect.DOLocalMove(nameOriginalPos, 1f).SetEase(Ease.OutBack));
        sequence.Insert(1.7f, nameRect.DOLocalMove(nameOriginalPos + new Vector3(100f, 0f, 0f), 0.3f).SetEase(Ease.InBack));

        RectTransform imageRect = unitImage.rectTransform;
        sequence.Insert(0, imageRect.DOScale(new Vector3(0.6f, 1.4f, imageOriginalScale.z), 0.5f).SetEase(Ease.OutBack));
        sequence.Insert(0.5f, imageRect.DOScale(imageOriginalScale, 0.5f).SetEase(Ease.InBack));
        sequence.Insert(1.7f, imageRect.DOScaleY(0, 0.3f).SetEase(Ease.InBack));

        sequence.Insert(1.7f, canvasGroup.DOFade(0, 0.3f));

        return sequence;
    }

}
