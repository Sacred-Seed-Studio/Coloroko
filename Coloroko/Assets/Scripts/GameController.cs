using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour
{
    public static GameController controller;
    public bool GameStarted { get; set; }
    public bool Audio_BackgroundMusicOn { get; set; }
    public bool Audio_SoundsOn { get; set; }

    public Image titleBackground;
    public Text touchText;


    void Awake()
    {
        if (controller == null)
        {
            controller = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (controller != this)
        {
            Destroy(gameObject);
        }

        ResetGame();
    }

    void Update()
    {
        if (!GameStarted && (Input.GetButtonDown("StartGame") || Input.touches.Length > 0)) start = true;

    }
    public void ResetGame()
    {
        GameStarted = false;
        start = false;
        StartCoroutine(WaitForStart());
    }

    bool start = false;
    IEnumerator WaitForStart()
    {
        while (!start)
        {
            yield return null;
        }
        //fade the black to white and start game
        float inc = 0.02f;
        float p = 0;
        while (p < 1)
        {
            titleBackground.color = Color.Lerp(Color.black, Color.white, p);
            touchText.color = new Color(0, 0, 0, 0);
            p += inc;
            yield return null;
        }
        titleBackground.transform.parent.gameObject.SetActive(false);
        GameStarted = true;
        yield return null;
    }
}
