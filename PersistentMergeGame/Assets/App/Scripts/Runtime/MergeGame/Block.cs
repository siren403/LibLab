using System;
using R3;
using UnityEngine;

namespace App.MergeGame
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Block : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sprite = null!;


        private readonly Subject<Block> _onClicked = new Subject<Block>();
        public Observable<Block> OnClicked => _onClicked;

        public Color Color
        {
            set => sprite.color = value;
        }

        private void OnMouseDown()
        {
            _onClicked.OnNext(this);
        }
    }
}
