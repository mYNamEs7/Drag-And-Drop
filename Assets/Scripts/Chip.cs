using System;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class Chip : MonoBehaviour
{
    [SerializeField] private int level;
    [SerializeField] private TMP_Text levelText;

    public RectTransform Rect { get; private set; }

    private Cell currentCell;
    private Cell startCell;

    private bool isDragging;
    private Vector2 dragOffset;

    private GridManager grid;
    private LayoutElement element;

    private void Awake()
    {
        Rect = GetComponent<RectTransform>();
        grid = FindObjectOfType<GridManager>();
    }

    private void Start()
    {
        transform.localScale = Vector3.one;
        UpdateLevel(level);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            TryPick();

        if (Input.GetMouseButton(0) && isDragging)
            Drag();

        if (Input.GetMouseButtonUp(0) && isDragging)
            Drop();
    }
    
    public void UpdateLevel(int newLevel)
    {
        level = newLevel;
        levelText.text = level.ToString();
    }

    private void TryPick()
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(Rect, Input.mousePosition))
            return;

        currentCell?.Clear();
        isDragging = true;
        startCell = currentCell;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            Rect,
            Input.mousePosition,
            null,
            out dragOffset
        );
        
        Rect.SetAsLastSibling();

        DOTween.Kill(this);
        Rect.DOScale(1.2f, 0.15f).SetId(this);
    }

    private void Drag()
    {
        Rect.position = Input.mousePosition - (Vector3)dragOffset;
    }

    private void Drop()
    {
        isDragging = false;
        Rect.DOScale(1f, 0.15f).SetId(this);

        var bestCell = grid.GetBestCellForChip(this);

        if (bestCell == null)
        {
            ReturnBack();
            return;
        }

        HandleCell(bestCell);
    }

    private void HandleCell(Cell cell)
    {
        if (cell.IsEmpty)
        {
            MoveToCell(cell);
            return;
        }

        if (cell.OccupiedChip.level == level)
        {
            Merge(cell.OccupiedChip);
            return;
        }

        ReturnBack();
    }

    private void MoveToCell(Cell cell)
    {
        currentCell?.Clear();
        cell.PlaceChip(this);
    }

    private void ReturnBack()
    {
        startCell.PlaceChip(this);
    }

    private void Merge(Chip other)
    {
        var cell = other.currentCell;

        DOTween.Kill(other);

        other.Rect
            .DOScale(other.Rect.localScale * 1.3f, 0.15f)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() =>
            {
                Destroy(other.gameObject);
                Destroy(gameObject);

                var newChip = grid.SpawnChip();

                newChip.UpdateLevel(level + 1);
                cell.PlaceChip(newChip);
            });
    }

    public void SetCell(Cell cell) => currentCell = cell;
}
