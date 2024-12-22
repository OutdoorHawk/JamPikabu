using System;
using Code.Progress;
using Code.Progress.SaveLoadService;
using Code.Progress.Writer;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Code.Gameplay.Cheats.Cheats
{
    public class DeleteProgressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private const float TIME = 5;

        private float _timer;
        private bool _isDown;
        private IProgressReadWrite _progressReadWrite;

        [Inject]
        private void Construct(IProgressReadWrite progressReadWrite)
        {
            _progressReadWrite = progressReadWrite;
        }
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
                _progressReadWrite.DeleteProgress();
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