using System.Collections;
using UnityEngine;

public class Game : MonoBehaviour
{
    public virtual void StartGameIntro() {}
    public virtual void AssignPlayer(PlayerControls playerControls, int id) {}

    public virtual void AssignPlayer(PlayerControls playerControls, int id, bool isMainPlayer) { }
}
