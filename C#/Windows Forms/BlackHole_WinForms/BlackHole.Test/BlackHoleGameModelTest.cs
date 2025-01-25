using System;
using System.Threading.Tasks;
using BlackHole.Model;
using BlackHole.Persistence;
using Moq;

namespace BlackHole.Test
{
    [TestClass]
    public class BlackHoleGameModelTest
    {
        private BlackHoleGameModel _model = null!;
        private BlackHoleMap _mockedMap = null!;
        private Mock<IBlackHoleDataAccess> _mock = null!;

        [TestInitialize]
        public void Initialize()
        {
            _mockedMap = new BlackHoleMap(5);
            _mockedMap.SetFieldValue(0, 1, BlackHoleMap.Field.RED);
            _mockedMap.SetFieldValue(1, 2, BlackHoleMap.Field.BLUE);
            _mockedMap.SetFieldValue(4, 4, BlackHoleMap.Field.EMPTY);

            _mock = new Mock<IBlackHoleDataAccess>();
            _mock.Setup(mock => mock.LoadAsync(It.IsAny<string>()))
                .Returns(() => Task.FromResult(_mockedMap));

            _model = new BlackHoleGameModel(_mock.Object);

            _model.GameAdvanced += new EventHandler<BlackHoleEventArgs>(Model_GameAdvanced);
            _model.GameOver += new EventHandler<BlackHoleEventArgs>(Model_GameOver);
        }

        [TestMethod]
        public void BlackHoleModelNewGameTest3x3()
        {
            _model.NewGame(3);

            Assert.AreEqual(BlackHoleMap.Field.BLUE, _model.CurrentPlayer);
            Assert.AreEqual(3, _model.Map.GetMapSize());
            Assert.AreEqual(600*3, _model.GameTime);
            Assert.AreEqual(BlackHoleMap.Field.BLACKHOLE, _model.Map.GetFieldValue(1, 1));

            Int32 red = 0;
            Int32 blue = 0;
            Int32 empty = 0;

            for (Int32 i = 0; i < 3; i++)
            {
                for (Int32 j = 0; j < 3; j++)
                {
                    if (_model.Map.IsFieldEmpty(i, j))
                        empty++;
                    else if (_model.Map.GetFieldValue(i, j) == BlackHoleMap.Field.BLUE)
                        blue++;
                    else if (_model.Map.GetFieldValue(i, j) == BlackHoleMap.Field.RED)
                        red++;
                }
            }

            Assert.AreEqual(2, red);
            Assert.AreEqual(2, blue);
            Assert.AreEqual(4, empty);
        }

        [TestMethod]
        public void BlackHoleGameModelStepTest()
        {
            _model.NewGame(3);

            Assert.AreEqual(BlackHoleMap.Field.BLUE, _model.CurrentPlayer);

            _model.Step(0, 0, BlackHoleMap.Directions.LEFT);

            Assert.AreEqual(BlackHoleMap.Field.RED, _model.Map.GetFieldValue(0, 0));

            _model.Step(2, 0, BlackHoleMap.Directions.UP);

            Assert.AreEqual(BlackHoleMap.Field.BLUE, _model.Map.GetFieldValue(1, 0));
        }

        [TestMethod]
        public void BlackHoleGameModelAdvanceTimeTest()
        {
            _model.NewGame(5);

            Int32 time = _model.GameTime;
            while (!_model.IsGameOver)
            {
                _model.AdvanceTime();

                time--;

                Assert.AreEqual(time, _model.GameTime);
                Assert.AreEqual(BlackHoleMap.Field.BLUE, _model.CurrentPlayer);
            }

            Assert.AreEqual(0, _model.GameTime);
        }

        [TestMethod]
        public async Task BlackHoleGameModelLoadTest()
        {
            _model.NewGame(5);

            await _model.LoadGameAsync(String.Empty);

            for (Int32 i = 0; i < 5; i++)
            {
                for (Int32 j = 0; j < 5; j++)
                {
                    Assert.AreEqual(_mockedMap.GetFieldValue(i, j), _model.Map.GetFieldValue(i, j));
                }
            }

            Assert.AreEqual(BlackHoleMap.Field.BLUE, _model.CurrentPlayer);

            _mock.Verify(dataAccess => dataAccess.LoadAsync(String.Empty), Times.Once());
        }

        [TestMethod]
        private void Model_GameAdvanced(Object? sender, BlackHoleEventArgs e)
        {
            Assert.IsTrue(_model.GameTime >= 0);
            Assert.AreEqual(_model.GameTime == 0, _model.IsGameOver);

            Assert.AreEqual(e.CurrentPlayer, _model.CurrentPlayer);
            Assert.AreEqual(e.GameTime, _model.GameTime);
            Assert.IsFalse(e.IsWon);
        }

        private void Model_GameOver(Object? sender, BlackHoleEventArgs e)
        {
            Assert.IsTrue(_model.IsGameOver);
            Assert.AreEqual(0, e.GameTime);
            Assert.IsFalse(e.IsWon);
        }
    }
}