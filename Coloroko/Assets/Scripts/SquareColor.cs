using UnityEngine;
using System.Collections;

public class SquareColor : MonoBehaviour
{

    public int color;

    public void ChangeColor()
    { 
            Debug.Log("Setting color " + color);
            Board.board.SetNextColor(color);
    }
}
