using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public abstract class PhoneButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private bool _mouseHovering = false;

    public InputAction clickAction;

    public virtual void Awake()
    {

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        //enabled = true;
        _mouseHovering = true;
        // Debug.Log("Mouse entered button!");
        gameObject.GetComponent<Image>().color = Color.grey;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        //enabled = false;
        _mouseHovering = false;
        // Debug.Log("Mouse exited button!");
        gameObject.GetComponent<Image>().color = Color.black;

    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (_mouseHovering)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
                OnMousePressedThisFrame();

            if (Mouse.current.leftButton.isPressed)
                OnMouseDown();
        }
    }

    public virtual void OnMousePressedThisFrame()
    {

    }

    public virtual void OnMouseDown()
    {
        // Debug.Log("button on mouse down");
    }

    public virtual void OnButtonClick()
    {
        // Debug.Log("Button clicked!");
    }
}
