using Base;
using UnityEngine;
using System.Collections.Generic;
namespace BackGround
{
    public class ParrallexController: MonoBehaviour
    {
        [SerializeField] private List<ParrallexBG> _parrallexBGs;
        private float _lastCamPositionX;
        private float _cameraHalfWidth;
        private Camera _camera;

        void Start()
        {
            var service = ServiceLocator.Get<Helper>();
            _camera = service.mainCam;

            _cameraHalfWidth = _camera.orthographicSize * _camera.aspect;
        }

        void FixedUpdate()
        {
            float currentCamPositionX = _camera.transform.position.x;
            float distanceToMove = currentCamPositionX - _lastCamPositionX;
            _lastCamPositionX = currentCamPositionX;
            float cameraRightEdge = currentCamPositionX + _cameraHalfWidth;
            float cameraLeftEdge = currentCamPositionX - _cameraHalfWidth;
            // DebugCustom.Log(cameraRightEdge.ToString());
            // DebugCustom.Log(cameraLeftEdge.ToString());
            foreach(var bg in _parrallexBGs)
                bg.Move(distanceToMove);

            foreach(var bg in _parrallexBGs)
                bg.LoopBackGround(cameraRightEdge, cameraLeftEdge);
        }
    }
}