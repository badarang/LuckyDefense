using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitMovement : MonoBehaviour
{
    public float cellSize = 1f;
    private bool placeUpper;
    private Vector3 offset;
    private float yOffset = -.3f;
    private bool isDragging = false;
    public bool IsDragging => isDragging;
    private Animator animator;
    private Unit unit;
    private Coroutine moveCoroutine;
    

    public void Init(bool isMyPlayer)
    {
        animator = GetComponentInChildren<Animator>();
        unit = GetComponent<Unit>();
        placeUpper = !isMyPlayer;
    }

    void OnMouseDown()
    {
        if (GameManager.Instance.CurrentState != GameState.InGame) return;
        if (placeUpper) return; // AI 유닛은 드래그 불가
        if (isDragging) return;
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("UI");
            return;
        }
        UnitManager.Instance.StartDragPosition(unit.GridPosition);
    }

    public void StartDrag()
    {
        offset = transform.position - GetMouseWorldPosition();
        isDragging = true;
        animator.SetTrigger("Walk");
    }

    void OnMouseDrag()
    {
        if (GameManager.Instance.CurrentState != GameState.InGame) return;
        if (!isDragging) return;

        Vector3 newPosition = GetMouseWorldPosition() + offset;
        Vector2 gridPosition = WorldToGrid(newPosition);
        
        gridPosition.x = Mathf.Round(gridPosition.x + 2.5f) - 2.5f;
        gridPosition.x = Mathf.Clamp(gridPosition.x, -2.5f, 2.5f);

        if (placeUpper)
        {
            gridPosition.y = Mathf.Clamp(gridPosition.y, -1, 1);
            gridPosition.y = Mathf.Round(gridPosition.y) + yOffset;
        }
        else
        {
            gridPosition.y = Mathf.Clamp(gridPosition.y, -5.5f, -3.5f);
            gridPosition.y = Mathf.Round(gridPosition.y - 0.5f) + 0.5f + yOffset;
        }

        transform.position = GridToWorld(gridPosition);
    }

    void OnMouseUp()
    {
        if (GameManager.Instance.CurrentState != GameState.InGame) return;
        if (isDragging) return;
        if (placeUpper)
        {
            UnitManager.Instance.SelectPosition(unit.GridPosition, isMyPlayer: !placeUpper);
            return;
        }
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        UnitManager.Instance.EndDragPosition();
    }

    public void StartMove(Vector2 target, float fixedSpeed = 1)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = StartCoroutine(MoveCO(target, fixedSpeed));
    }

    public IEnumerator MoveCO(Vector2 target, float fixedSpeed)
    {
        Vector2 startPosition = transform.position;
        var isFacingRight = startPosition.x < target.x ? true : false;
        unit.Flip(isFacingRight);
        float distance = Vector2.Distance(startPosition, target);
        float speed = unit.MoveSpeed;
        if (Mathf.Abs(fixedSpeed - 1) > .01f) speed = fixedSpeed;
        float time = distance / speed;
        float elapsedTime = 0;
        StartDrag();
        
        while (elapsedTime < time)
        {
            transform.position = Vector2.Lerp(startPosition, target, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        EndDrag();
        transform.position = target;
    }
    
    public void EndDrag()
    {
        isDragging = false;
        animator.SetTrigger("Idle");
        animator.ResetTrigger("Walk");
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    Vector2 WorldToGrid(Vector3 worldPos)
    {
        float x = Mathf.Round(worldPos.x + 2.5f) - 2.5f;
        float y = worldPos.y;
        return new Vector2(x, y);
    }

    Vector3 GridToWorld(Vector2 gridPos)
    {
        return new Vector3(gridPos.x, gridPos.y, 0);
    }
}