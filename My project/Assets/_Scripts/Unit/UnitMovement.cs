using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    public bool PlaceUpper = true;
    public float cellSize = 1f;
    
    private Vector3 offset;
    private float yOffset = -.3f;
    private bool isDragging = false;
    public bool IsDragging => isDragging;
    private Animator animator;
    

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void OnMouseDown()
    {
        offset = transform.position - GetMouseWorldPosition();
        isDragging = true;
        animator.SetTrigger("Walk");
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;

        Vector3 newPosition = GetMouseWorldPosition() + offset;
        Vector2 gridPosition = WorldToGrid(newPosition);
        
        gridPosition.x = Mathf.Round(gridPosition.x + 2.5f) - 2.5f;
        gridPosition.x = Mathf.Clamp(gridPosition.x, -2.5f, 2.5f);

        if (PlaceUpper)
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