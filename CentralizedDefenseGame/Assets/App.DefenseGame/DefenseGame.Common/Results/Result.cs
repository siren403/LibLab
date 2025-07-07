// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace DefenseGame.Common.Results
{
    public record Result
    {
        private readonly List<Error>? _errors = null;

        public IReadOnlyList<Error> Errors
        {
            get
            {
                if (!IsError || _errors is null)
                {
                    throw new System.InvalidOperationException(
                        "Cannot access Errors on a successful result or when errors are null.");
                }

                return _errors.AsReadOnly();
            }
        }

        internal List<Error> GetErrors()
        {
            if (_errors is null)
            {
                throw new System.InvalidOperationException("Cannot access Errors when there are no errors.");
            }

            return _errors;
        }

        public bool IsError => _errors is not null;


        protected Result(string code)
        {
            _errors = new List<Error> { new Error { Code = code } };
        }

        protected Result(List<Error> errors)
        {
            _errors = errors;
        }

        protected Result()
        {
        }

        public static Result Ok()
        {
            return new Result();
        }

        public static Result Fail(string code)
        {
            return new Result(code);
        }
    }

    public abstract record Result<T> : Result
    {
        public static Ok<T> Ok(T data)
        {
            return new Ok<T>(data);
        }

        public static Fail<T> Fail(List<Error> errors)
        {
            return new Fail<T>(errors);
        }

        public static new Fail<T> Fail(string code)
        {
            return new Fail<T>(code);
        }

        public static implicit operator UniTask<Result<T>>(Result<T> result)
        {
            return UniTask.FromResult(result);
        }

        private readonly T? _value = default;

        public T Value
        {
            get
            {
                if (IsError || _value is null)
                {
                    throw new System.InvalidOperationException(
                        "Cannot access Value on an error result or when value is null.");
                }

                return _value;
            }
        }

        protected Result(T value)
        {
            _value = value;
        }

        protected Result(List<Error> errors) : base(errors)
        {
            _value = default;
        }

        protected Result(string code) : base(code)
        {
            _value = default;
        }
    }

    public sealed record Ok<T> : Result<T>
    {
        internal Ok(T value) : base(value)
        {
        }
    }

    public sealed record Fail<T> : Result<T>
    {
        internal Fail(List<Error> errors) : base(errors)
        {
        }

        internal Fail(string code) : base(code)
        {
        }
    }
}
