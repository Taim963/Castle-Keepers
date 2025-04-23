using UnityEngine;
using UnityEngine.Events;

public class InputHandler : MonoBehaviour
{
    public UnityEvent onLeftClick;
    public UnityEvent onEClick;

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
    }

    public void SetLostGame()
    {
        lostGame = true;
    }
}
