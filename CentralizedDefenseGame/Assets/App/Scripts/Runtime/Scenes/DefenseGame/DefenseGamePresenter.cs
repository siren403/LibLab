using System;
using Drawing;
using UnityEngine;

namespace App.Scenes.DefenseGame
{
    public class DefenseGamePresenter : MonoBehaviour
    {
        [SerializeField] private float radius = 3f;

        public void SetRadius(float newRadius)
        {
            if (newRadius <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(newRadius), "Radius must be greater than zero.");
            }

            radius = newRadius;
        }

        private void Update()
        {
#if UNITY_EDITOR
            var draw = Draw.ingame;
            draw.Circle(transform.position, Vector3.up, radius);
#endif
        }
    }
}
