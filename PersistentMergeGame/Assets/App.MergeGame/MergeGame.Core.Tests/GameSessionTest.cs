using MergeGame.Common;
using MergeGame.Core.Internal.Entities;
using NUnit.Framework;
using UnityEngine;

namespace MergeGame.Core.Tests
{
    public class GameSessionTest
    {
        [Test]
        public void CreateSession()
        {
            var sessionManager = TestUtil.GetGameManager(out _);
            var session = sessionManager.CreateGameSession(5, 5);
            Assert.IsNotNull(session);

            var result = sessionManager.GetSession(session.Id);
            Assert.IsTrue(result is Ok<GameSession>);
            if (result is Ok<GameSession>(var resultSession))
            {
                Assert.IsNotNull(resultSession);

                Assert.AreSame(session, resultSession);
                Debug.Log(resultSession);
            }
        }
    }
}
