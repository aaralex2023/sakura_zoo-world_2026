using UnityEngine;

namespace ZooWorld.Animals
{
    [System.Serializable]
    public sealed class TastyLabelConfig : IAnimalConfig
    {
        [SerializeField] private string _text = "Tasty!";
        [SerializeField] private float _duration = 1f;
        [SerializeField] private Vector2 _screenOffset;

        public string Text => _text;
        public float Duration => _duration;
        public Vector2 ScreenOffset => _screenOffset;
    }
}
