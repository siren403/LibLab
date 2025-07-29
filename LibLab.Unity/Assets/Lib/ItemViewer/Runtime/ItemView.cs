// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using UnityEngine;

namespace ItemViewer
{
    public abstract class ItemView : MonoBehaviour
    {
        public NullOr<ItemVisuals> Visuals
        {
            get
            {
                var nullOrVisuals = new NullOr<ItemVisuals>(visuals);
                if (nullOrVisuals)
                {
                    nullOrVisuals.Value.View = this;
                }

                return nullOrVisuals;
            }
        }

        [SerializeReference, SerializeField] private ItemVisuals? visuals;
    }
}
