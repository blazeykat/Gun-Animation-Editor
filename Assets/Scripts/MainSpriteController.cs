﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;


public class MainSpriteController : MonoBehaviour
{
    public Image mainSprite;
    private GaeAnimationInfo currentAnimationInfo;
    private int index = 0;

    public static MainSpriteController instance;
    [SerializeField]
    private TMP_InputField xOffset;

    [SerializeField]
    private TMP_InputField yOffset;

    [SerializeField]
    private TextMeshProUGUI frameCounter;

    private static readonly CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");

    public void SetAnimation(GaeAnimationInfo animation)
    {
        currentAnimationInfo = animation;
        index = 0;
        UpdateSprite(true);
        if (animation == null)
        {
            mainSprite.sprite = null;
        }
    }
    public void NextFrame()
    {
        if (currentAnimationInfo!= null)
        {
            MoveIndex(1);
        }
    }
    public void PreviousFrame()
    {

        if (currentAnimationInfo != null)
        {
            MoveIndex(-1);
        }
    }
    private void MoveIndex(int i)
    {
        UpdateCurrentFrameHandData();
        index += i;
        if (currentAnimation != null)
        {
            

            if (index< 0)
            {
                index = currentAnimation.frames.Length-1;
            }
            else if(index >= currentAnimation.frames.Length)
            {
                index = 0;
            }
            
            UpdateSprite(true);
        }       
    }
    public void UpdateCurrentFrameHandData()
    {
        if (currentAnimation != null && currentFrame != null)
        {
            Debug.Log("update sprite data ran");
            Image mainSprite = StaticRefrences.Instance.MainSprite;
            RawImage hand1 = StaticRefrences.Instance.handIMG;
            RawImage hand2 = StaticRefrences.Instance.handIMG2;
            FrameInfo info = currentFrame;
            info.hand1PositionX = (hand1.rectTransform.anchoredPosition.x - mainSprite.rectTransform.anchoredPosition.x) / StaticRefrences.zoomScale;
            info.hand1PositionY = (hand1.rectTransform.anchoredPosition.y - mainSprite.rectTransform.anchoredPosition.y) / StaticRefrences.zoomScale;
            info.hand2PositionX = (hand2.rectTransform.anchoredPosition.x - mainSprite.rectTransform.anchoredPosition.x) / StaticRefrences.zoomScale;
            info.hand2PositionY = (hand2.rectTransform.anchoredPosition.y - mainSprite.rectTransform.anchoredPosition.y) / StaticRefrences.zoomScale;
            info.muzzleflashPositionX = (MuzzleFlashObject.anchoredPosition.x - mainSprite.rectTransform.anchoredPosition.x) / StaticRefrences.zoomScale;
            info.muzzleflashPositionY = (MuzzleFlashObject.anchoredPosition.y - mainSprite.rectTransform.anchoredPosition.y) / StaticRefrences.zoomScale;
            float.TryParse(xOffset.text,out info.offsetX);
            float.TryParse(yOffset.text,out info.offsetY);
            info.isTwoHanded = StaticRefrences.Instance.IsTwoHanded.isOn;
            
        }
    }
    [SerializeField]
    GameObject barreGenButton;
    [SerializeField]
    GameObject barrelInfoButton;
    [SerializeField]
    RectTransform MuzzleFlashObject;
    public void UpdateSprite(bool UpdateInputLabels)
    {
        Debug.Log("update sprite ran");
        if (currentAnimation != null && currentFrame!= null)
        {
            
            mainSprite.sprite = currentAnimationInfo.frames[index].sprite;

            mainSprite.SetNativeSize();

            Vector2 anchoredPos = mainSprite.rectTransform.anchoredPosition;
            
            Vector2 pos = new Vector2(-mainSprite.sprite.rect.width / 2 * StaticRefrences.zoomScale, -mainSprite.sprite.rect.height / 2 * StaticRefrences.zoomScale);
            
            pos = new Vector2(Mathf.Round(pos.x / StaticRefrences.zoomScale) * StaticRefrences.zoomScale, Mathf.Round(pos.y / StaticRefrences.zoomScale) * StaticRefrences.zoomScale);

            if (StaticRefrences.Instance.IsGungeoneerOn.isOn)
            {
                pos = StaticRefrences.Instance.Gungeoneer.anchoredPosition;
                pos = new Vector2(pos.x + currentFrame.offsetX* StaticRefrences.zoomScale, pos.y + currentFrame.offsetY* StaticRefrences.zoomScale);
            }

            mainSprite.rectTransform.anchoredPosition = pos;
            
            Vector2 handpos1 = new Vector2(pos.x + currentFrame.hand1PositionX * StaticRefrences.zoomScale, pos.y + currentFrame.hand1PositionY * StaticRefrences.zoomScale);
            Vector2 handpos2 = new Vector2(pos.x + currentFrame.hand2PositionX * StaticRefrences.zoomScale, pos.y + currentFrame.hand2PositionY * StaticRefrences.zoomScale);

            StaticRefrences.Instance.handIMG.rectTransform.anchoredPosition = handpos1;
            StaticRefrences.Instance.handIMG2.rectTransform.anchoredPosition = handpos2;
            StaticRefrences.Instance.IsTwoHanded.isOn = currentFrame.isTwoHanded;
            if(barrelInfoButton != null && barreGenButton != null && MuzzleFlashObject != null)
            {
                if (currentFrame.path.Contains("idle_001"))
                {
                    barreGenButton.SetActive(true);
                    barrelInfoButton.SetActive(true);
                    MuzzleFlashObject.gameObject.SetActive(true);
                    int zoomscale = StaticRefrences.zoomScale;
                    Vector2 muzzlepos = new Vector2(pos.x + currentFrame.muzzleflashPositionX * zoomscale, pos.y + currentFrame.muzzleflashPositionY * zoomscale);
                    MuzzleFlashObject.anchoredPosition = muzzlepos;
                }
                else
                {
                    barreGenButton.SetActive(false);
                    barrelInfoButton.SetActive(false);
                    MuzzleFlashObject.gameObject.SetActive(false);
                }
            }
            if (frameCounter != null)
            {
                frameCounter.text = (index + 1).ToString();
            }
            if (UpdateInputLabels)
            {
                //pulls both values before setting the variables because setting the input fields values triggers a "UpdateCurrentFrameHandData" call
                //this is also why this bit of code is right near the end
                string offsetx = currentFrame.offsetX.ToString("F4", culture);
                string offsety = currentFrame.offsetY.ToString("F4", culture);
                xOffset.text = offsetx;
                yOffset.text = offsety;
            }

        }
        else if (barrelInfoButton != null && barreGenButton != null && MuzzleFlashObject != null)
        {
            barreGenButton.SetActive(false);
            barrelInfoButton.SetActive(false);
            MuzzleFlashObject.gameObject.SetActive(false);
        }
        Debug.Log("end of updateSprite method");

    }
    public FrameInfo currentFrame 
    {
        get
        {
            return currentAnimationInfo.frames[index];
        }
    }
    public GaeAnimationInfo currentAnimation
    {
        get
        {
            return currentAnimationInfo;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        MainSpriteController.instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCurrentFrameHandData();
    }
}
