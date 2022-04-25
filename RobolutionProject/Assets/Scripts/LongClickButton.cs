using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class LongClickButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    bool pointerDown;
    float pointerDownTimer;
    [SerializeField]
    float timeHold;
    private void Awake()
    {
        timeHold = 1;
    }
    public UnityEvent onLongClick;
    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDown = true;
        Debug.Log("OnPointerDown");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp");
    }
    private void Update()
    {
        if (pointerDown)
        {
            pointerDownTimer += Time.deltaTime;
            if (pointerDownTimer >= timeHold)
            {
                if (onLongClick != null)
                    onLongClick.Invoke();

                Reset();
            }

        }
    }

    private void Reset()
    {
        pointerDown = false;
        pointerDownTimer = 0;
    }
}
