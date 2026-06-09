using UnityEngine;

public interface IBuildingState
{
    void EndState();
    void OnAction(Vector3Int gridPosition);
    void UpdateState(Vector3Int gridPosition);
    // Offset to apply to the mouse position before grid snapping so the cursor sits at the CENTER of the item being placed
    Vector2Int GetSize();
}