using UnityEngine;

public interface IBuildingState
{
    void EndState(); //clean up when leaving the state
    void OnAction(Vector3Int gridPosition); //what happens when the player clicks
    void UpdateState(Vector3Int gridPosition); //what happens every frame the mouse move.
}