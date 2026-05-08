using UnityEngine;

namespace BackGround
{
    [System.Serializable]
    public class ParrallexBG
    {
        public Transform _transform;
        public float _parrallexMultiplier;
        private float _imageFullWidth;
        

        public void Move(float distance)
        {
            _transform.position += Vector3.right *(distance * _parrallexMultiplier );
        }


        public void LoopBackGround(float cameraRightEdge, float cameraLeftEdge)
        {
            _imageFullWidth = _transform.GetComponent<SpriteRenderer>().bounds.size.x;

            float imageRightEdge = _transform.position.x + _imageFullWidth/2;
            float imageLeftEdge = _transform.position.x - _imageFullWidth/2;


            if(imageRightEdge < cameraLeftEdge)
                _transform.position += Vector3.right * (_imageFullWidth);
            else if(imageLeftEdge > cameraRightEdge)
                _transform.position += Vector3.right * -(_imageFullWidth);


        }
        

    }
}

