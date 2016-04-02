using UnityEngine;
using System.Collections;

public class TouchController : MonoBehaviour
{
    public bool fakeTouched = false;

    public void CheckForTouches()
    {
        if (Input.touches.Length <= 0)
        {
            if (!fakeTouched) OnNoTouches();
        }
        else
        {
            for (int i = 0; i < Input.touches.Length; i++)
            {
                if (Input.GetTouch(i).phase == TouchPhase.Began) { OnTouchBegin(); }
                if (Input.GetTouch(i).phase == TouchPhase.Ended) { OnTouchEnded(); }
            }
        }
    }

    public virtual void OnNoTouches() { }
    public virtual void OnTouchBegin() { }
    public virtual void OnTouchEnded() { }
}
