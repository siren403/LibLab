using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using LunaWolfStudios.ScriptableSheets.Samples.RPG;
using MasterMemory;
using MessagePack;
using MessagePack.Resolvers;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.TestTools;
using ZLinq;

namespace App.Tests
{
    [MemoryTable("unit"), MessagePackObject(true)]
    public record UnitIndex
    {
        [PrimaryKey] public int UnitId { get; init; }

        [SecondaryKey(2, keyOrder: 1), NonUnique]
        public int Level { get; init; }

        [SecondaryKey(1), NonUnique]
        [SecondaryKey(2, keyOrder: 0), NonUnique]
        public bool IsShiny { get; init; }
    }

    public class ScriptableTableTest
    {
        private AsyncOperationHandle<IList<Unit>> _unitHandle;
        private Unit[] _units = Array.Empty<Unit>();
        private MemoryDatabase? _database;

        private static bool _isResolverSetup = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void SetupMessagePackResolver()
        {
            if (_isResolverSetup) return;

            // Create CompositeResolver
            StaticCompositeResolver.Instance.Register(new[]
            {
                MasterMemoryResolver.Instance, // set MasterMemory generated resolver
                StandardResolver.Instance // set default MessagePack resolver
            });

            // Create options with resolver
            var options = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);

            // Optional: as default.
            MessagePackSerializer.DefaultOptions = options;
            _isResolverSetup = true;
        }

        [UnityOneTimeSetUp]
        public IEnumerator UnityOneTimeSetUp() => UniTask.ToCoroutine(async () =>
        {
            var ct = Application.exitCancellationToken;
            var locations = await Addressables
                .LoadResourceLocationsAsync(new[] { "table", "unit" }, Addressables.MergeMode.Intersection)
                .WithCancellation(ct);

            AsyncOperationHandle<IList<Unit>> handle = Addressables.LoadAssetsAsync<Unit>(locations, null!);
            await handle.WithCancellation(ct);

            Debug.Log($"{nameof(handle.Result)}: {handle.Result.Count}");

            _unitHandle = handle;

            int concatCount = 10;
            var units = new List<Unit>(concatCount * _unitHandle.Result.Count);
            foreach (int _ in Enumerable.Range(0, concatCount))
            {
                units.AddRange(_unitHandle.Result);
            }

            _units = units.ToArray();
            Debug.Log($"{nameof(_units)}: {_units.Length}");
            _database = BuildDatabase(_units);
        });

        // [Test]
        // public void BuildDatabase()
        // {
        //     Debug.Log($"{nameof(_units)}: {_units.Length}");
        //     _database = BuildDatabase(_units);
        // }

        [Test]
        public void UsingZLinq()
        {
            Debug.Log($"{nameof(_units)}: {_units.Length}");
            var shinyUnits = _units.Where(unit => unit.Shiny);
            Debug.Log($"{nameof(shinyUnits)}: {shinyUnits.Count()}");
            // var shinyUnitsOverLevel10 = _units.Where(unit => unit is { Shiny: true, Level: > 10 });
            // Debug.Log($"{nameof(shinyUnitsOverLevel10)}: {shinyUnitsOverLevel10.Count()}");
            var shinyAndUnderLevel = _units
                .Where(unit => unit is { Shiny: true, Level: < 10 });
            Debug.Log($"{nameof(shinyAndUnderLevel)}: {shinyAndUnderLevel.Count()}");
        }

        [Test]
        public void UsingMasterMemory()
        {
            Debug.Log($"{nameof(_database.UnitIndexTable)}: {_database?.UnitIndexTable.Count ?? 0}");
            int shinyCount = _database?.UnitIndexTable.FindByIsShiny(true).Count ?? 0;
            Debug.Log($"{nameof(shinyCount)}: {shinyCount}");
            var shinyAndUnderLevel = _database!.UnitIndexTable
                .FindRangeByIsShinyAndLevel((true, 0), (true, 9));
            Debug.Log($"{nameof(shinyAndUnderLevel)}: {shinyAndUnderLevel.Count}");
        }

        [UnityOneTimeTearDown]
        public IEnumerator UnityOneTimeTearDown() => UniTask.ToCoroutine(async () =>
        {
            var ct = Application.exitCancellationToken;
            if (_unitHandle.IsValid())
            {
                _unitHandle.Release();
                Debug.Log($"{nameof(_unitHandle)} released.");
            }
        });

        private MemoryDatabase BuildDatabase(IList<Unit> units)
        {
            var builder = new DatabaseBuilder();
            var indexes = units.Select(unit => new UnitIndex()
            {
                UnitId = unit.GetInstanceID(), Level = unit.Level, IsShiny = unit.Shiny
            }).ToArray();
            builder.Append(indexes);
            byte[] binary = builder.Build();
            int processorCount = Environment.ProcessorCount;
            var database = new MemoryDatabase(binary, maxDegreeOfParallelism: processorCount);
            Debug.Log($"Database initialized with {processorCount} processors.");
            return database;
        }
        // [UnityTest]
        // public IEnumerator ZLinqQuery() => UniTask.ToCoroutine(async () =>
        // {
        //     var ct = Application.exitCancellationToken;
        //
        //     {
        //         var unitEntities = _unitHandle.Result;
        //
        //         Debug.Log($"{nameof(unitEntities)}: {unitEntities.Count}");
        //
        //         var shinyUnits = unitEntities.Where(unit => unit.Shiny);
        //         Debug.Log($"{nameof(shinyUnits)}: {shinyUnits.Count()}");
        //     }
        //     {
        //         var locations = await Addressables
        //             .LoadResourceLocationsAsync(new[] { "table", "weapon" }, Addressables.MergeMode.Intersection)
        //             .WithCancellation(ct);
        //
        //         Debug.Log($"{nameof(locations)}: {locations.Count}");
        //         var handle = Addressables.LoadAssetsAsync<Weapon>(locations, null!);
        //         var weaponEntities = await handle.WithCancellation(ct);
        //         Debug.Log($"{nameof(weaponEntities)}: {weaponEntities.Count}");
        //
        //         var rangedWeapons = weaponEntities.Where(weapon => weapon.WeaponCategory == WeaponCategory.Ranged);
        //         Debug.Log($"{nameof(rangedWeapons)}: {rangedWeapons.Count()}");
        //         await UniTask.DelayFrame(5, cancellationToken: ct);
        //         handle.Release();
        //         await UniTask.DelayFrame(5, cancellationToken: ct);
        //     }
        // });
    }
}
