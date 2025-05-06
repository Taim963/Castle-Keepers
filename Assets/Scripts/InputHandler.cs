using UnityEngine;
using UnityEngine.Events;

public class InputHandler : MonoBehaviour
{
    public UnityEvent onLeftClick;
    public UnityEvent onEClick;
    public UnityEvent onCTRLClick;
    public UnityEvent onMWD;
    public UnityEvent onMWU;

    private bool editMode = false;
    private bool lostGame = false;


    private void Awake()
    {
        if (onLeftClick == null)
        {
            onLeftClick = new UnityEvent(); // Initialize to prevent null reference
        }
    }

    private void Update()
    {
        float scrollValue = Input.GetAxis("Mouse ScrollWheel");

        if (lostGame)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            editMode = !editMode;
        }

        if (Input.GetMouseButtonDown(0) && !editMode)
        {
            onLeftClick.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            onCTRLClick.Invoke();
        }

        if (scrollValue > 0)
        {
            onMWU.Invoke();
        }

        if (scrollValue < 0)
        {
            onMWD.Invoke();
        }
    }

    public void SetLostGame()
    {
        lostGame = true;
    }
}
