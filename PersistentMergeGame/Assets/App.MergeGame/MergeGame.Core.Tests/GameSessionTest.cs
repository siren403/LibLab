using System;
using MergeGame.Core.Internal.Extensions;
using MergeGame.Core.Enums;
using NUnit.Framework;
using UnityEngine;

namespace MergeGame.Core.Tests
{
    public class GameSessionTest
    {
        [Test]
        public void CreateSession()
        {
            var sessionManager = TestUtil.GetGameManager();
            var session = sessionManager.CreateGameSession(5, 5);
            Assert.IsNotNull(session);

            var result = sessionManager.GetSession(session.Id);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);

            Assert.AreSame(session, result.Value);
            Debug.Log(result.Value);
        }
    }
}
