using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
/// <summary>
/// Этот скрипт писал не я
/// </summary>
public class ButtonSpriteSwitcher : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public Sprite IdleSprite;
    public Sprite PressedSprite;
    public RectTransform IconRectMover;
    public float MoveYOffest = -5;

    public AudioClip ClickIn;
    public AudioClip ClickOut;

    Button targetButton;
    Image targetImage;
    Vector3 iconStartPos;
    Vector3 iconEndPos;


    private void Awake()
    {
        targetButton = this.GetComponent<Button>();
        targetImage = this.GetComponent<Image>();

        if (IconRectMover)
        {
            iconStartPos = IconRectMover.localPosition;
            iconEndPos = IconRectMover.localPosition + new Vector3(0, MoveYOffest, 0);
        }
    }


    //У юнити нет стандартного эвента для определения состояния кнопки, как вариант сделать Fixed update с отслеживанием кнопки,
    //Либо сделать скрипт наследование класса Button и по сути выполнить ровно те же эвенты что снизу
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!targetButton) return;

        if (IconRectMover)
        {
            IconRectMover.localPosition = iconStartPos;
        }

        if (ClickOut)
        {
            SoundManager.PlayButtonSound(ClickOut);
        }

        if (!PressedSprite) return;
        targetImage.sprite = IdleSprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!targetButton) return;

        if (IconRectMover)
        {
            IconRectMover.localPosition = iconEndPos;
        }

        if (ClickIn) SoundManager.PlayButtonSound(ClickIn);

        if (!targetButton.interactable) return;
        if (!PressedSprite) return;
        targetImage.sprite = PressedSprite;
    }
}




