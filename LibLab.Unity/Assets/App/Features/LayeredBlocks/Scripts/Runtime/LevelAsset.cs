using System;
using UnityEngine;

namespace App.Features.LayeredBlocks
{

    [CreateAssetMenu(fileName = "Level", menuName = "App/Layered Blocks/Level")]
    public class LevelAsset : ScriptableObject
    {
        [SerializeField, HideInInspector] private string? id;

        public string Id
        {
            set => id = value;
            get => id ?? string.Empty;
        }
    }
}
