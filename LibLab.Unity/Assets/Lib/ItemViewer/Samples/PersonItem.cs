using UnityEngine;

namespace ItemViewer.Samples
{
    public class StudentVisuals : ItemVisuals
    {
        [field: SerializeField] public SpriteRenderer Icon { get; private set; } = null!;
    }

    public class TeacherVisuals : ItemVisuals
    {
        [field: SerializeField] public string Name { get; private set; } = string.Empty;
        [field: SerializeField] public SpriteRenderer Icon { get; private set; } = null!;
    }

    [ItemVisuals(typeof(StudentVisuals), typeof(TeacherVisuals))]
    public class PersonItem : ItemView
    {
    }
}
