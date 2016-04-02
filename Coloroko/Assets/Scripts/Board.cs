using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum SquareType
{
    Normal,
    Abstract,
    Abstract2,
    Cheetah,
    Youtube,
    Zebra
}
public class Board : TouchController
{
    public static Board board;

    public SquareType squareType = SquareType.Normal;
    GameObject squarePrefab;

    Dictionary<Vector2, Square> squares;
    List<Vector2> squarePositions;

    static int width = 9, height = width;

    public Color[] colours;
    public Color emptyColor = Color.white;

    public int chosenColor = -1;
    public GameObject colorPanel;

    public bool colorPanelOpenEh;

    public List<Button> uiSquares;

    public static HashSet<int> centerPositions = new HashSet<int>() { 1, 4, 7 };
    [SerializeField]
    public HashSet<Square> centerSquares;

    Vector2 clampBounds = new Vector2(1.53f, 6.43f);
    bool touchE = false, touchB = false;
    void Awake()
    {
        board = this;
        squarePrefab = Resources.Load<GameObject>("Prefabs/" + squareType.ToString() + "Square");

        CreateBoardEh();
    }

    void Update()
    {
        if (!GameController.controller.GameStarted) return;
        CheckForTouches();

        //So touch controls work with the mouse
        if (Input.GetMouseButtonDown(0))
        {
            fakeTouched = true;
            OnTouchBegin();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            fakeTouched = false;
            OnTouchEnded();
        }
    }

    void CreateBoardEh()
    {
        colorPanel.transform.SetParent(transform);
        colorPanel.SetActive(false);
        squares = new Dictionary<Vector2, Square>();
        squarePositions = new List<Vector2>();
        centerSquares = new HashSet<Square>();
        Vector2 t = Vector2.zero;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                t = new Vector2(x, y);
                squares[t] = (Instantiate(squarePrefab, t + (Vector2)transform.position, Quaternion.identity) as GameObject).GetComponent<Square>();
                //squares[t].ChangeColor(colours[0], 0);
                squares[t].Empty();
                squares[t].transform.SetParent(transform);
                squares[t].location = t;
                squares[t].name = "Square " + x + "-" + y;
                squares[t].empty = true;

                squarePositions.Add(t);
            }
        }

        foreach (Vector2 s in squarePositions)
        {
            if (centerPositions.Contains((int)s.x) && centerPositions.Contains((int)s.y))
            {
                centerSquares.Add(squares[s]);
            }//both x and y belong to centerPositions, then it is a center position

        }
    }

    public Square[] GetColumn(int x)
    {
        Square[] temp = new Square[width];
        int count = 0;
        foreach (Vector2 v in squarePositions)
        {
            if (x == v.x)
            {
                temp[count] = squares[v];
                count++;
            }
        }
        return temp;
    }

    public Square[] GetRow(int y)
    {
        Square[] temp = new Square[height];
        int count = 0;
        foreach (Vector2 v in squarePositions)
        {
            if (y == v.y)
            {
                temp[count] = squares[v];
                count++;
            }
        }
        return temp;
    }

    public Square[] GetBigSquare(Vector2 centerLocation)
    {
        Square[] temp = new Square[width];
        int c = 0;

        for (int k = 0; k <= 2; k++)
        {
            for (int i = 0; i <= 2; i++)
            {
                temp[c] = squares[new Vector2(k + centerLocation.x - 1, i + centerLocation.y - 1)];
                c++;
            }
        }
        return temp;
    }

    public void SetColors(Vector2 location)
    {
        //set all buttons active, then disactive any colours found in that row, column or bigsquare
        foreach (Button sc in uiSquares)
        {
            sc.interactable = true;
        }

        foreach (Square s in Board.board.GetBigSquare(Board.board.CenterLocation(location)))
        {
            if (!s.empty) uiSquares[s.currentColor].interactable = false;
        }
        foreach (Square s in Board.board.GetRow((int)location.y))
        {
            if (!s.empty) uiSquares[s.currentColor].interactable = false;
        }
        foreach (Square s in Board.board.GetColumn((int)location.x))
        {
            if (!s.empty) uiSquares[s.currentColor].interactable = false;
        }
    }

    public void SetColorPanel(Vector2 location)
    {
        //location.x = Mathf.Clamp(location.x, clampBounds.x, clampBounds.y);
        colorPanel.transform.position = location;
        SetColors(location);
        colorPanel.SetActive(true);
        colorPanelOpenEh = true;
    }

    public void TurnPanelOff()
    {
        Debug.Log("Turning panel off");
        colorPanel.SetActive(false);
        colorPanelOpenEh = false;
    }

    public void SetNextColor(int n)
    {
        TurnPanelOff();
        //if (n == -2) return; //don't do anything because this is the cancel button

        chosenColor = n;
    }

    //Given a particular location, return the center square closest
    public Vector2 CenterLocation(Vector2 s)
    {
        foreach (Square v in centerSquares)
        {
            //to be "close" to a center, x and y must be 0 or 1 away from the center square location
            if (CloseEh((int)v.location.x, (int)s.x) && CloseEh((int)v.location.y, (int)s.y)) return v.location;
        }

        return Vector2.zero;
    }

    public bool CloseEh(int n1, int n2)
    {
        return Mathf.Abs(n1 - n2) < 2;
    }

    //To detect on mobile when a click has been released
    public override void OnNoTouches()
    {
        //Debug.Log("No touches");
        touchE = false;
        touchB = false;
    }
    public override void OnTouchBegin()
    {
        if (touchB) return;
        touchB = true;
        //Debug.Log("Touch begin" + Camera.main.ScreenToWorldPoint(Input.mousePosition));

        //cast a ray from the camera to see what the mouse has clicked on (if anything)
        //RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        //if (hit.collider != null)
        //{
        //    Square s = hit.collider.GetComponent<Square>();

        //    if (s != null && !colorPanelOpenEh)
        //    {
        //        StartCoroutine(s.StartClick());
        //    }
        //}

        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hits != null)
        {

            foreach (RaycastHit2D hit in hits)
            {
                Square s = hit.collider.GetComponent<Square>();

                if (s != null && !colorPanelOpenEh)
                {
                    //Debug.Log("Hit " + hit.collider.name);
                    if (!s.clueSquare) StartCoroutine(s.StartClick());
                }
            }
        }
    }
    public override void OnTouchEnded()
    {
        if (touchE) return;
        touchE = true;
        //Debug.Log("Touch ended" + Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //if mouse position is different here then the touch has moved
        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hits != null)
        {
            //if (colorPanelOpenEh) // don't check the start click
            bool b = colorPanelOpenEh;
            foreach (RaycastHit2D hit in hits)
            {
                SquareColor sc = hit.collider.GetComponent<SquareColor>();
                Square s = hit.collider.GetComponent<Square>();
                if (s != null && !colorPanelOpenEh)
                {
                    if (b) break;
                    //Debug.Log("Hit " + hit.collider.name);
                    if (!s.clueSquare) StartCoroutine(s.StartClick());
                    else
                    {

                    }
                }
                else if (sc != null && colorPanelOpenEh)
                {
                    Debug.Log(sc.GetComponent<Button>().IsInteractable());
                    if (sc.GetComponent<Button>().IsInteractable())
                    {
                        sc.ChangeColor();
                        Debug.Log("Hit " + hit.collider.name);
                    }
                }

            }
        }
    }
}
