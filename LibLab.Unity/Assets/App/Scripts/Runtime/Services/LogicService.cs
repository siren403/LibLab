// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace App.Services
{
    public enum LogicResult
    {
        Success,
        Failed
    }

    public interface ILogic
    {
        string Id { get; }
        LogicResult Process(CancellationToken cancellationToken);
    }

    public class LogicService
    {
        private readonly Dictionary<string, ILogic> _logics;

        public LogicService(IReadOnlyList<ILogic> logics)
        {
            _logics = logics.ToDictionary((logic) => logic.Id, logic => logic);
        }

        public async UniTask<LogicResult> To(string id, CancellationToken cancellationToken)
        {
            if (!_logics.TryGetValue(id, out ILogic logic))
            {
                return LogicResult.Success;
            }

            LogicResult result = logic.Process(cancellationToken);

            if (result == LogicResult.Success)
            {
                return LogicResult.Success;
            }

            if (_logics.TryGetValue($"{id}:fail", out logic))
            {
                return logic.Process(cancellationToken);
            }

            return LogicResult.Failed;
        }
    }
}
