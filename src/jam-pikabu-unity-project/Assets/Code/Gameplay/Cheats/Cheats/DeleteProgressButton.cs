using System;
using Code.Progress;
using Code.Progress.SaveLoadService;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.Gameplay.Cheats.Cheats
{
    public class DeleteProgressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private const float TIME = 5;

        private float _timer;
        private bool _isDown;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _isDown = true;
        }

        private void Update()
        {
            if (_isDown == false)
                return;
            
            _timer += Time.deltaTime;

            if (_timer > TIME)
            {
                ProgressExtensions.DeleteProgress(SaveLoadService.PlayerProgressPath);
                PlayerPrefs.DeleteAll();
                Application.Quit();
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isDown = false;
        }
    }
}