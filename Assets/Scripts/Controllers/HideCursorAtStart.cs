using UnityEngine;

public class HideCursorAtStart : MonoBehaviour
{
    private void Awake() => CursorController.HideCursor();
}
