using UnityEngine;

namespace Base
{
    public class Helper: MonoBehaviour
    {
        public Camera mainCam;

        void Awake()
        {
            ServiceLocator.Register<Helper>(this);
            mainCam = FindObjectOfType<Camera>();
        }
    }
}