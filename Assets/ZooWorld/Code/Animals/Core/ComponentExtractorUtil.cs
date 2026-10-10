using System.Collections.Generic;
using UnityEngine;

namespace ZooWorld.Animals
{
    public static class ComponentExtractorUtil
    {
        public static T[] ExtractDirectChildren<T>(GameObject root, GameObject excludedDirectChild = null) where T : class
        {
            if (root == null)
                throw new System.ArgumentNullException(nameof(root));

            List<T> components = new();
            AddFrom(root, components);

            Transform rootTransform = root.transform;
            for (int i = 0; i < rootTransform.childCount; i++)
            {
                Transform child = rootTransform.GetChild(i);
                if (child.gameObject != excludedDirectChild && child.gameObject.activeInHierarchy)
                    AddFrom(child.gameObject, components);
            }

            return components.ToArray();
        }

        private static void AddFrom<T>(GameObject gameObject, List<T> results) where T : class
        {
            Component[] components = gameObject.GetComponents<Component>();
            foreach (Component component in components)
            {
                if (component is Behaviour behaviour && !behaviour.isActiveAndEnabled)
                    continue;

                if (component is T match)
                    results.Add(match);
            }
        }
    }
}
