using UnityEngine;
using System.Collections;

public class Square : MonoBehaviour
{
    SpriteRenderer sr;
    public int currentColor; //from array in Board
    public Vector2 location;

    [HideInInspector]
    public bool empty = true;

    public bool clueSquare = false;
    void Awake()
    {
        empty = true;
        sr = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        if (Random.Range(0, 100) % 10 == 0)
        {
            Debug.Log("Setting clue square " + name);
            clueSquare = true;
            int c = Random.Range(0, 9);
            sr.color = Board.board.colours[c];
            currentColor = c;
            empty = false;
        }
    }

    public void ChangeColor(Color newColor, int c)
    {
        sr.color = newColor;
        currentColor = c;
        empty = false;

    }
    public void ChangeColor(int c)
    {
        if (clueSquare) return;

        currentColor = c;
        if (currentColor == -1)
        {
            empty = true;
            sr.color = Board.board.emptyColor;
        }
        else if (currentColor == -2)
        {
            //this is the cancel button
            Debug.Log("Cancelling");
            //play a cancel sound
        }
        else
        {
            empty = false;
            sr.color = Board.board.colours[currentColor];
        }

    }
    public void ChangeAllColors(Color newColor, int c)
    {
        foreach (Square s in Board.board.GetBigSquare(Board.board.CenterLocation(location)))
        {
            s.ChangeColor(newColor, c);
        }

    }

    //public void OnMouseDown()
    //{
    //    //check if that colour already exists in the row, column, and square
    //    //currentColor++;
    //    //if (currentColor >= Board.board.colours.Length ) currentColor = 0;
    //    ////ChangeColor(Board.board.colours[currentColor], currentColor);
    //    //ChangeAllColors(Board.board.colours[currentColor], currentColor);
    //    if (Board.board.colorPanelOpenEh) return;
    //    StartCoroutine(StartClick());
    //}

    public void Empty()
    {
        empty = false;
        sr.color = Board.board.emptyColor;
        currentColor = Board.board.colours.Length - 1;
    }

    public IEnumerator StartClick()
    {
        Debug.Log("Clicking");
        Board.board.CenterLocation(location);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        Board.board.SetColorPanel(location);

        while (Board.board.colorPanelOpenEh)
        {
            yield return null;
        }

        ChangeColor(Board.board.chosenColor);
        yield return null;
    }
}
