using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private Weapon weapon;

    private bool editMode = false;

    private void Start()
    {
        weapon = FindAnyObjectByType<Weapon>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Toggle editMode state
            editMode = !editMode;

            // Set cursor visibility and lock state
            Cursor.visible = editMode;
            Cursor.lockState = editMode ? CursorLockMode.None : CursorLockMode.Locked;

            Debug.Log($"Edit Mode toggled: {editMode} (Cursor visible: {Cursor.visible}, Lock state: {Cursor.lockState})");
        }

        // Left mouse click behavior
        if (Input.GetMouseButtonDown(0) && !editMode)
        {
            if (weapon != null)
                weapon.TryFire();
        }
    }
}
