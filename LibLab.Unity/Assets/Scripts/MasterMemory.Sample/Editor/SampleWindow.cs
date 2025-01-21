using System;
using System.Collections.Generic;
using MasterMemory.Tables;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VitalRouter;
using VitalRouter.MRuby;

namespace MasterMemory.Sample.Editor
{
    public class SampleWindow : EditorWindow
    {

        private readonly DataSource _dataSource = new();
        private MRubyContext _context;
        private MemoryDatabase _database;

        private void OnEnable()
        {
            if (_dataSource.data == null)
            {
                DatabaseBuilder builder = new();
                int id = 0;
                builder.Append(new Person[]
                {
                    new()
                    {
                        PersonId = id++, Age = 13, Gender = Gender.Male, Name = "Bruce"
                    },
                    new()
                    {
                        PersonId = id++, Age = 14, Gender = Gender.Male, Name = "Clark"
                    },
                    new()
                    {
                        PersonId = id++, Age = 15, Gender = Gender.Female, Name = "Diana"
                    },
                    new()
                    {
                        PersonId = id++, Age = 14, Gender = Gender.Male, Name = "Jack"
                    }
                });

                _dataSource.data = builder.Build();
            }

            _database = new MemoryDatabase(_dataSource.data);

            _context = MRubyContext.Create();
            _context.Router = Router.Default;
            _context.CommandPreset = new Preset();

            _dataSource.MapTo(Router.Default);
            _dataSource.Database = _database;
        }

        private void OnDisable()
        {
            _context?.Dispose();
        }

        private void CreateGUI()
        {
            VisualTreeAsset uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/Scripts/MasterMemory.Sample/Editor/SampleWindow.uxml");

            VisualElement root = rootVisualElement;
            root.dataSource = _dataSource;
            root.Add(uxml.Instantiate());

            {
                VisualElement container = root.Q<VisualElement>("findById");
                IntegerField field = container.Q<IntegerField>();
                // RegisterEnterCallback(field, (_dataSource.findById, _database));
                Button button = container.Q<Button>();
                // RegisterClickCallback(button, (_dataSource.findById, _database));

                container.RegisterCallback((ClickEvent e) =>
                {
                    if (e.target is not Button btn || string.IsNullOrWhiteSpace(btn.name))
                    {
                        return;
                    }

                    if (e.currentTarget is not VisualElement ve)
                    {
                        return;
                    }

                    Debug.Log($"Clicked {ve.name}.{btn.name}");
                    using MRubyScript script = _context.CompileScript(@"cmd :query, type: ""findById""");
                    script.RunAsync();
                });
            }

            {
                VisualElement container = root.Q<VisualElement>("findGenderAndAge");

                IntegerField ageField = container.Q<IntegerField>();
                RegisterEnterCallback(ageField, (_dataSource.findGenderAndAge, _database));

                Button button = container.Q<Button>();
                RegisterClickCallback(button, (_dataSource.findGenderAndAge, _database));

            }

            {
                VisualElement container = root.Q<VisualElement>("findClosestByAge");
                IntegerField ageField = container.Q<IntegerField>();
                RegisterEnterCallback(ageField, (_dataSource.findClosestByAge, _database));

                Button button = container.Q<Button>();
                RegisterClickCallback(button, (_dataSource.findClosestByAge, _database));
            }

            {
                VisualElement container = root.Q<VisualElement>("findRangeByAge");
                Button button = container.Q<Button>();
                RegisterClickCallback(button, (_dataSource.findRangeByAge, _database));
            }

            void Execute(ValueTuple<Query, MemoryDatabase> args)
            {
                (Query query, MemoryDatabase database) = args;
                query.Execute(database);
            }

            void RegisterEnterCallback(
                VisualElement element,
                ValueTuple<Query, MemoryDatabase> userArgs
            )
            {
                element.RegisterCallback((KeyDownEvent e, ValueTuple<Query, MemoryDatabase> args) =>
                {
                    if (e.keyCode != KeyCode.Return)
                    {
                        return;
                    }
                    Execute(args);
                }, userArgs, TrickleDown.TrickleDown);
            }

            void RegisterClickCallback(
                Button button,
                ValueTuple<Query, MemoryDatabase> userArgs
            )
            {
                button.RegisterCallback((ClickEvent e, ValueTuple<Query, MemoryDatabase> args) =>
                {
                    Execute(args);
                }, userArgs);
            }
        }


        [InitializeOnLoadMethod]
        private static void Initialize()
        {
        }

        [MenuItem("Window/MasterMemory/Sample")]
        private static void ShowWindow()
        {
            SampleWindow window = GetWindow<SampleWindow>();
            window.titleContent = new GUIContent(nameof(SampleWindow));
            window.Show();
        }
    }

    [MRubyObject]
    internal partial struct QueryCommand : ICommand
    {
        public string Type;
    }

    [MRubyCommand("query", typeof(QueryCommand))]
    internal partial class Preset : MRubyCommandPreset
    {
    }

    internal enum QueryStatus
    {
        None,
        Success,
        Failure
    }

    [Serializable]
    [Routes]
    internal partial class DataSource
    {
        public int id;
        public byte[] data;
        public Query.FindById findById = new();
        public Query.FindGenderAndAge findGenderAndAge = new();
        public Query.FindClosestByAge findClosestByAge = new();
        public Query.FindRangeByAge findRangeByAge = new();

        public MemoryDatabase Database;

        [Route]
        private void On(QueryCommand cmd)
        {
            if (Database == null) return;
            switch (cmd.Type)
            {
                case nameof(findById):
                    findById.Execute(Database);
                    break;
                case nameof(findGenderAndAge):
                    findGenderAndAge.Execute(Database);
                    break;
                case nameof(findClosestByAge):
                    findClosestByAge.Execute(Database);
                    break;
                case nameof(findRangeByAge):
                    findRangeByAge.Execute(Database);
                    break;
            }
        }
    }

    [Serializable]
    internal abstract class Query
    {

        private readonly List<string> _output = new();
        private QueryStatus _status;

        [CreateProperty]
        public StyleColor StatusColor => _status switch
        {
            QueryStatus.Success => new StyleColor(Color.green),
            QueryStatus.Failure => new StyleColor(Color.red),
            _ => new StyleColor()
        };

        [CreateProperty]
        public bool UpdatedOutput { get; set; }

        [CreateProperty]
        public string Output => string.Join("\n", _output);

        private bool Success()
        {
            _status = QueryStatus.Success;
            return true;
        }

        private bool Success(string output)
        {
            _output.Clear();
            _output.Add(output);
            UpdatedOutput = true;
            return Success();
        }

        private bool Success<T>(RangeView<T> results) where T : class
        {
            _output.Clear();
            for (int i = 0; i < results.Count; i++)
            {
                _output.Add(results[i].ToString());
            }
            UpdatedOutput = true;
            return Success();
        }

        private bool Failure()
        {
            _status = QueryStatus.Failure;
            return false;
        }

        private bool Failure(string error)
        {
            _output.Clear();
            _output.Add(error);
            UpdatedOutput = true;
            return Failure();
        }

        public abstract void Execute(MemoryDatabase database);

        [Serializable]
        internal partial class FindById : Query
        {
            public int id;

            private bool TryExecute(MemoryDatabase database, out Person result)
            {
                PersonTable table = database.PersonTable;
                return table.TryFindByPersonId(id, out result)
                    ? Success(result.ToString())
                    : Failure($"PersonId {id} not found.");
            }

            public override void Execute(MemoryDatabase database)
            {
                TryExecute(database, out Person p);
            }
        }

        [Serializable]
        internal class FindGenderAndAge : Query
        {
            public Gender gender;
            public int age;

            private bool TryExecute(MemoryDatabase database, out RangeView<Person> results)
            {
                PersonTable table = database.PersonTable;
                results = table.FindByGenderAndAge((gender, age));
                return results.Any()
                    ? Success(results)
                    : Failure($"Not found. {gender}, {age}");
            }

            public override void Execute(MemoryDatabase database)
            {
                TryExecute(database, out RangeView<Person> p);
            }
        }

        [Serializable]
        internal class FindClosestByAge : Query
        {
            public int age;
            public bool selectLower = true;

            private bool TryExecute(MemoryDatabase database, out RangeView<Person> results)
            {
                PersonTable table = database.PersonTable;
                results = table.FindClosestByAge(age, selectLower);
                return results.Any() ? Success(results) : Failure($"Not found. {age}");
            }

            public override void Execute(MemoryDatabase database)
            {
                TryExecute(database, out RangeView<Person> p);
            }
        }

        [Serializable]
        internal class FindRangeByAge : Query
        {
            public bool ascendant = true;

            [CreateProperty]
            public int Min { get; private set; } = 10;

            [CreateProperty]
            public int Max { get; private set; } = 20;

            [CreateProperty]
            public Vector2 Value
            {
                get => new(Min, Max);
                set
                {
                    Min = (int)value.x;
                    Max = (int)value.y;
                }
            }

            private bool TryExecute(MemoryDatabase database, out RangeView<Person> results)
            {
                PersonTable table = database.PersonTable;
                results = table.FindRangeByAge(Min, Max, ascendant);
                return results.Any() ? Success(results) : Failure($"Not found. {Min} - {Max}");
            }

            public override void Execute(MemoryDatabase database)
            {
                TryExecute(database, out RangeView<Person> p);
            }
        }
    }
}
