// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace App.UI.Pages
{
    [RequireComponent(typeof(Button))]
    public class PushButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private string id;

        private RouterProvider _router;

        private void Awake()
        {
            _router = new RouterProvider(gameObject);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return;
            }
            _ = _router.PublishAsync(new PushCommand()
            {
                Id = id
            });
        }
    }
}
