using System;
using UnityEngine;

namespace ZooWorld.World
{
    public sealed class CameraScreenBounds : IScreenBounds
    {
        private readonly Camera _camera;
        private readonly IRandom _random;

        public Rect Area
        {
            get
            {
                Vector3 bottomLeft = GetGroundPoint(0f, 0f);
                Vector3 topRight = GetGroundPoint(1f, 1f);
                return Rect.MinMaxRect(bottomLeft.x, bottomLeft.z, topRight.x, topRight.z);
            }
        }

        public CameraScreenBounds(Camera camera, IRandom random)
        {
            _camera = camera != null ? camera : throw new ArgumentNullException(nameof(camera));
            _random = random ?? throw new ArgumentNullException(nameof(random));

            Transform cameraTransform = _camera.transform;
            if (Vector3.Dot(cameraTransform.forward, Vector3.down) < 0.999f ||
                Vector3.Dot(cameraTransform.right, Vector3.right) < 0.999f ||
                Vector3.Dot(cameraTransform.up, Vector3.forward) < 0.999f)
                throw new ArgumentException("Screen bounds require a camera facing straight down with no roll or yaw.", nameof(camera));
            if (cameraTransform.position.y <= 0f)
                throw new ArgumentException("Screen bounds require a camera above the ground plane.", nameof(camera));
        }

        private Vector3 GetGroundPoint(float viewportX, float viewportY)
        {
            Ray ray = _camera.ViewportPointToRay(new Vector3(viewportX, viewportY, 0f));
            if (ray.direction.y >= 0f)
                throw new InvalidOperationException("The camera frustum does not intersect the ground plane.");

            float distance = -ray.origin.y / ray.direction.y;
            if (distance <= 0f)
                throw new InvalidOperationException("The camera frustum does not intersect the ground plane in front of the camera.");

            return ray.GetPoint(distance);
        }

        public bool Contains(Vector3 position, float inset = 0f)
        {
            Rect area = Area;
            return position.x >= area.xMin + inset && position.x <= area.xMax - inset &&
                   position.z >= area.yMin + inset && position.z <= area.yMax - inset;
        }

        public Vector3 RandomPoint(float inset = 0f)
        {
            Rect area = Area;
            float minX = area.xMin + inset;
            float maxX = area.xMax - inset;
            float minZ = area.yMin + inset;
            float maxZ = area.yMax - inset;
            if (minX > maxX || minZ > maxZ)
                throw new ArgumentOutOfRangeException(nameof(inset), inset, "The inset leaves no area to sample.");

            return new Vector3(
                _random.Range(minX, maxX),
                0f,
                _random.Range(minZ, maxZ));
        }
    }
}
