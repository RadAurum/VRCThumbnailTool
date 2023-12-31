﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RadAngelZero.ThumbnailTool
{
    public class ThumbnailTool : MonoBehaviour
    {

        [SerializeField] public Texture thumbnail;
        [SerializeField] public float posX = 0f;
        [SerializeField] public float posY = 0f;
        [SerializeField] public float scale = 1f;
        [SerializeField] public bool forceThumbnailUpdate = true;
        bool forcedThumnail = false;

        bool hasCompletedSetup = false;

        private void Start()
        {

        }

        private void Update()
        {
            if (thumbnail == null)
            {
                return;
            }
            string gameObjectName = gameObject.name;
            if (hasCompletedSetup)
            {
                bool checkThumbnailImage = GameObject.Find("/" + gameObjectName + "/ThumbnailCanvas/ThumbnailImage") != null;
                if (checkThumbnailImage)
                {
                    GameObject thumbnailImage = GameObject.Find("/" + gameObjectName + "/ThumbnailCanvas/ThumbnailImage");
                    RectTransform thumbnailImageRecTransform = thumbnailImage.GetComponent<RectTransform>();
                    if (thumbnailImageRecTransform != null)
                    {
                        if (posX != thumbnailImageRecTransform.rect.x || posY != thumbnailImageRecTransform.rect.y)
                        {
                            thumbnailImageRecTransform.localPosition = new Vector3(posX, posY, 0);
                        }
                        if (scale != thumbnailImageRecTransform.localScale.x)
                        {
                            thumbnailImageRecTransform.localScale = new Vector3(scale, scale, 1);
                        }
                        if (thumbnail != thumbnailImage.GetComponent<RawImage>().texture)
                        {
                            thumbnailImage.GetComponent<RawImage>().texture = thumbnail;
                        }
                        if (thumbnail.width != thumbnailImageRecTransform.sizeDelta.x || thumbnail.height != thumbnailImageRecTransform.sizeDelta.y)
                        {
                            fixSize(gameObjectName);
                        }
                        if (forceThumbnailUpdate && !forcedThumnail)
                        {
                            bool checkVRCSDK = GameObject.Find("/VRCSDK") != null;
                            if (checkVRCSDK)
                            {
                                bool checkIfToggle = GameObject.Find("/VRCSDK/UI/Canvas/AvatarPanel/Avatar Info Panel/Thumbnail Section/ImageUploadToggle") != null;
                                if (checkIfToggle)
                                {
                                    GameObject uploadThumbnail = GameObject.Find("/VRCSDK/UI/Canvas/AvatarPanel/Avatar Info Panel/Thumbnail Section/ImageUploadToggle");
                                    Toggle uploadThumbnailToggle = uploadThumbnail.GetComponent<Toggle>();
                                    if (uploadThumbnailToggle.isOn == false)
                                    {
                                        uploadThumbnailToggle.isOn = true;
                                        forcedThumnail = true;
                                    }
                                }
                                
                            }
                        }
                    }
                }
            }
            if (!hasCompletedSetup)
            {
                if (Application.isPlaying)
                {
                    if (IsInPublish())
                    {
                        startSetUp(gameObjectName);
                    }
                }
            }
        }

        bool IsInPublish ()
        {
            return GameObject.Find("/VRCCam") != null;
        }

        bool thumbnailReady ()
        {
            return GameObject.Find("/ThumbnailTool/ThumbnailCanvas") != null && GameObject.Find("/ThumbnailTool/ThumbnailCanvas/ThumbnailImage") != null;
        }

        void fixSize (string gameObjectName)
        {
            GameObject thumbnailCanvas = GameObject.Find("/" + gameObjectName + "/ThumbnailCanvas");
            GameObject thumbnailImage = GameObject.Find("/" + gameObjectName + "/ThumbnailCanvas/ThumbnailImage");

            thumbnailCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(thumbnail.width, thumbnail.height);
            thumbnailCanvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(thumbnail.width, thumbnail.height);
            thumbnailImage.GetComponent<RectTransform>().sizeDelta = new Vector2(thumbnail.width, thumbnail.height);

            if (thumbnail.width <= thumbnail.height)
            {
                thumbnailCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0f;
            } else
            {
                thumbnailCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1f;
            }
        }

        void startSetUp (string gameObjectName)
        {
            Camera VRCCam = GameObject.Find("/VRCCam").GetComponent<Camera>();
            Canvas canvas;
            RawImage image;
            GameObject thumbnailCanvas = new GameObject();
            GameObject thumbnailImage = new GameObject();
            thumbnailCanvas.name = "ThumbnailCanvas";
            thumbnailImage.name = "ThumbnailImage";

            thumbnailCanvas.layer = 5;
            thumbnailImage.layer = 5;

            thumbnailCanvas.transform.SetParent(this.transform, false);
            thumbnailImage.transform.SetParent(thumbnailCanvas.transform, false);
            canvas = thumbnailCanvas.AddComponent<Canvas>();
            image = thumbnailImage.AddComponent<RawImage>();
            RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
            canvasRectTransform.position = Vector3.zero;
            CanvasScaler canvasScaler = thumbnailCanvas.AddComponent<CanvasScaler>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = VRCCam;

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

            image.texture = thumbnail;
            RectTransform imageRectTransform = image.GetComponent<RectTransform>();
            imageRectTransform.localPosition = new Vector3(posX, posY, 0f);
            imageRectTransform.sizeDelta = new Vector2(thumbnail.width, thumbnail.height);
            imageRectTransform.localScale = new Vector3(scale, scale, 1);

            VRCCam.cullingMask = 0 << 0x00000000;
            VRCCam.cullingMask |= 1 << LayerMask.NameToLayer("UI");
            VRCCam.nearClipPlane = 0.01f;
            VRCCam.farClipPlane = 1000f;

            fixSize(gameObjectName);

            hasCompletedSetup = true;
        }
    }
}
