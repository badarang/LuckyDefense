using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EntityAnimator : MonoBehaviour
{
    private Transform flippable;
    private Animator animator;
    private Material spriteMat;
    private SpriteRenderer spriteRenderer;
    private Coroutine hitAnimationCoroutine;
    private float hitEffectValue = 0;
    private float attackAnimationLength;
    private Vector3 originalScale = new Vector3(1, 1, 1);
    public float AttackAnimationLength => attackAnimationLength;
    private Tween walkMotionTween;
    

    public void Init()
    {
        transform.localPosition = new Vector3(0, 0, 0);
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.white;
        spriteMat = Instantiate(spriteRenderer.material);
        spriteRenderer.material = spriteMat;
        flippable = transform.parent.transform;
    }

    public void InitUnit(Unit _unit)
    {
        AnimationClip attackClip = GetAttackAnimationClip();
        if (attackClip != null)
        {
            attackAnimationLength = attackClip.length;
        }

        var shadow = transform.parent.Find("Shadow").gameObject;
        if (shadow != null)
        {
            shadow.GetComponent<SpriteRenderer>().color = Statics.GradeColor(_unit.Grade);
        }
    }

    public void InitEnemy(Enemy _enemy)
    {
        
    }
    
    private AnimationClip GetAttackAnimationClip()
    {
        RuntimeAnimatorController controller = animator.runtimeAnimatorController;

        foreach (AnimationClip clip in controller.animationClips)
        {
            if (clip.name.Contains("Attack"))
            {
                return clip;
            }
        }

        return null; // 찾지 못한 경우
    }

    
    public void StartWalkShakeAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(flippable.DOLocalRotate(new Vector3(0, 0, 7), 0.3f));
        sequence.Append(flippable.DOLocalRotate(new Vector3(0, 0, -7), 0.3f));
        sequence.SetLoops(-1);
        walkMotionTween = sequence;
    }
    
    public void StartHitAnimation(bool isAttackerOnRight)
    {
        if (hitAnimationCoroutine != null)
        {
            StopCoroutine(hitAnimationCoroutine);
        }
        walkMotionTween.Kill();
        walkMotionTween = null;
        hitAnimationCoroutine = StartCoroutine(HitAnimation(isAttackerOnRight));
    }
    
    private IEnumerator HitAnimation(bool isAttackerOnRight)
    {
        hitEffectValue = .8f;
        
        Transform unitTransform = transform;
        Vector3 originalPosition = unitTransform.localPosition;
        float moveDistance = 0.005f;
        float rotationAngle = 15f;
        
        int direction = isAttackerOnRight ? -1 : 1;
        
        unitTransform.DOKill(true);

        Sequence sequence = DOTween.Sequence();

        sequence.Append(unitTransform.DOScale(new Vector3(0.6f, 1.4f, 1f), 0.07f));
        sequence.Join(unitTransform.DOLocalMoveX(originalPosition.x + direction * -moveDistance, 0.07f));
        sequence.Join(unitTransform.DOLocalRotate(new Vector3(0, 0, -direction * rotationAngle), 0.07f));

        sequence.Append(unitTransform.DOScale(new Vector3(1.2f, 0.7f, 1f), 0.07f));
        sequence.Join(unitTransform.DOLocalMoveX(originalPosition.x, 0.07f));
        sequence.Join(unitTransform.DOLocalRotate(Vector3.zero, 0.07f));

        sequence.Append(unitTransform.DOScale(originalScale, 0.07f));

        yield return sequence.WaitForCompletion();
        unitTransform.localScale = originalScale;
        if (walkMotionTween == null) StartWalkShakeAnimation();
    }
    
    public void StopAllCoroutines()
    {
        if (hitAnimationCoroutine != null)
        {
            StopCoroutine(hitAnimationCoroutine);
        }
    }
    
    public void ToggleOutline(bool toggle)
    {
        if (toggle)
        {
            spriteMat.SetFloat("_OutlineAlpha", 1);
        }
        else
        {
            spriteMat.SetFloat("_OutlineAlpha", 0);
        }
    }
    
    public void StartDieAnimation()
    {
        animator.SetTrigger("Die");
        StartCoroutine(DieAnimationCO());
    }

    private IEnumerator DieAnimationCO()
    {
        Color color = spriteRenderer.color;
        while (color.a > 0)
        {
            color.a -= 0.1f;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(0.05f);
        }
    }
    
    void Update()
    {
        if (hitEffectValue > 0)
        {
            if (hitEffectValue < .1f) hitEffectValue = 0;
            if (!spriteMat.HasFloat("_HitEffectBlend")) return;
            spriteMat.SetFloat("_HitEffectBlend", hitEffectValue);
            hitEffectValue -= Time.deltaTime * 8;
        }
    }
}
