using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Cell[] cells;
    [SerializeField] private Chip chipPrefab;
    [SerializeField] private Transform chipsParent;
    
    public void Spawn()
    {
        var emptyCells = cells.Where(cell => cell.IsEmpty).ToList();

        if (emptyCells.Count == 0)
        {
            Debug.Log("Нет свободных клеток");
            return;
        }

        var target = emptyCells[Random.Range(0, emptyCells.Count)];
        var chip = SpawnChip();

        chip.UpdateLevel(1);
        target.PlaceChip(chip);
    }

    public Chip SpawnChip() => Instantiate(chipPrefab, chipsParent);

    public Cell GetBestCellForChip(Chip chip)
    {
        var chipRect = GetWorldRect(chip.Rect);

        var maxArea = 0f;
        Cell bestCell = null;

        foreach (var cell in cells)
        {
            var cellRect = GetWorldRect(cell.Rect);
            var area = GetIntersectionArea(chipRect, cellRect);

            if (!(area > maxArea)) continue;
            maxArea = area;
            bestCell = cell;
        }

        return maxArea > 0 ? bestCell : null;
    }

    private static Rect GetWorldRect(RectTransform rt)
    {
        var corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        return new Rect(
            corners[0].x,
            corners[0].y,
            corners[2].x - corners[0].x,
            corners[2].y - corners[0].y
        );
    }

    private static float GetIntersectionArea(Rect a, Rect b)
    {
        var xOverlap = Mathf.Max(0, Mathf.Min(a.xMax, b.xMax) - Mathf.Max(a.xMin, b.xMin));
        var yOverlap = Mathf.Max(0, Mathf.Min(a.yMax, b.yMax) - Mathf.Max(a.yMin, b.yMin));
        return xOverlap * yOverlap;
    }
}