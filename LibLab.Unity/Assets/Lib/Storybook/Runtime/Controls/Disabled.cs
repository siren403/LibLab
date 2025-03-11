using UnityEngine;

namespace Storybook.Controls
{
    public abstract class Disabled : MonoBehaviour
    {
        public abstract bool Value { get; set; }

        protected void Dirty(Component component)
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(component);
#endif
        }
    }
}
