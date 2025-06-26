using System;
using R3;
using UnityEngine;

namespace App.Scenes.MergeGame
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Block : MonoBehaviour
    {
        private readonly Subject<Block> _onClicked = new Subject<Block>();
        public Observable<Block> OnClicked => _onClicked;

        private void OnMouseDown()
        {
            _onClicked.OnNext(this);
        }
    }
}
