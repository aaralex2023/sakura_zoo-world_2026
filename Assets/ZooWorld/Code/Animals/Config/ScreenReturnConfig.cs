using UnityEngine;

namespace ZooWorld.Animals
{
    [System.Serializable]
    public sealed class ScreenReturnConfig
    {
        [SerializeField] private float _outsideMargin = -0.5f;
        [SerializeField] private float _insideMargin = 1.5f;

        public float OutsideMargin => _outsideMargin;
        public float InsideMargin => _insideMargin;
    }
}
