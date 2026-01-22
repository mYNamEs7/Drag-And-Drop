using UnityEngine;

public class Cell : MonoBehaviour
{
    public Chip OccupiedChip { get; private set; }
    public RectTransform Rect => (RectTransform)transform;
    public bool IsEmpty => OccupiedChip == null;

    public void PlaceChip(Chip chip)
    {
        OccupiedChip = chip;
        chip.SetCell(this);
        chip.Rect.position = Rect.position;
    }

    public void Clear() => OccupiedChip = null;
}