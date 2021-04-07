using System;
using MNS.Utils.Values;
using UnityEngine;
using UnityEngine.UI;

namespace D2Inventory
{

    public class Highlighter : MonoBehaviour
    {
        [SerializeField] InventoryController controller;
        [SerializeField] InventoryHighlightSettings settings;
        [SerializeField] Transform parentInHierarchy;

        private Image _image;
        private bool _imageActive;

        private void Awake()
        {
            _image = new GameObject("Highlight Area").AddComponent<Image>();
            _image.transform.SetParent(parentInHierarchy);
            _image.gameObject.SetActive(_imageActive = false);
        }

        private void OnEnable() 
            => controller.OnProjectionChanged.AddWithInvoke(NewHighlight);

        private void OnDisable() 
            => controller.OnProjectionChanged.RemoveListener(NewHighlight);

        public void NewHighlight(object sender, Projection proj)
        {
            if(proj.Empty && _imageActive)
                HideHighlight();
            else if(!proj.Same)
                SetHighlight(proj);
        }

        private void SetHighlight(Projection proj)
        {
            _image.color = proj.CanPlace ? settings.HighlightColor : settings.CantPlaceHereColor;
            _image.transform.position = proj.ScreenRect.center;
            _image.rectTransform.sizeDelta = proj.ScreenRect.size;
            _image.gameObject.SetActive(_imageActive = true);
        }

        public void HideHighlight()
            => _image.gameObject.SetActive(_imageActive = false);

    }
}