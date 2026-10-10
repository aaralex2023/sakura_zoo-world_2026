using UnityEngine;

namespace ZooWorld.UI
{
    public interface IFloatingLabelService
    {
        void Show(string text, Transform anchor, Vector2 screenOffset, float duration);
    }
}
