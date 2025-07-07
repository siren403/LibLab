// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using MergeGame.Core.Internal.Entities;
using UnitGenerator;

namespace MergeGame.Core.ValueObjects;

[UnitOf(typeof(string), UnitGenerateOptions.ImplicitOperator)]
public partial struct ErrorCode
{
    public static readonly ErrorCode CannotMergeBlock = new($"{nameof(Core)}.{nameof(BoardCell)}.NotMovableBlock");
    public static readonly ErrorCode CannotMovableCell = new($"{nameof(Core)}.{nameof(Board)}.CannotMovableCell");
}
