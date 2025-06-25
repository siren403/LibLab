using System;
using UnityEngine;

namespace App.Scenes.MergeGame
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Block : MonoBehaviour
    {
        public event Action<Block>? OnBlockClicked;

        private void OnMouseDown()
        {
            OnBlockClicked?.Invoke(this);
        }
    }
}
