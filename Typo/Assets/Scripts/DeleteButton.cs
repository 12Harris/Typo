using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;


public class DeleteButton : PhoneButton
{

    private float _clickTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public override void OnMousePressedThisFrame()
    {
        base.OnMousePressedThisFrame();
        PhoneController_Web.Instance.RemoveText(0);
        PhoneController_Web.Instance.PlaySound(gameObject);
        _clickTimer = 0f;
    }

    public override void OnMouseDown()
    {
        _clickTimer += Time.deltaTime;
        // Debug.Log(_clickTimer);
        PhoneController_Web.Instance.RemoveText(_clickTimer);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        _clickTimer = 0f;
    }
}
