// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace App.UI.Pages
{
    [RequireComponent(typeof(Button))]
    public class PopButton : MonoBehaviour, IPointerClickHandler
    {
        private RouterProvider _router;

        private void Awake()
        {
            _router = new RouterProvider(gameObject);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _ = _router.PublishAsync(new PopCommand());
        }
    }
}
