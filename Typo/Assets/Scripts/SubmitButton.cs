using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SubmitButton : PhoneButton
{

    public override void OnButtonClick()
    {

        base.OnButtonClick();
        if (PhoneController_Web.Instance.InputWord.Length > 0)
        {
            GameManager_Web.Instance.ResetInput(PhoneController.Instance.InputWord);
            GameManager_Web.Instance.NextWord(PhoneController.Instance.InputWord);
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);//enabled = true;
        if (PhoneController_Web.Instance.InputWord.Length == 0)
            gameObject.GetComponent<Image>().color = Color.black;
    }

}
