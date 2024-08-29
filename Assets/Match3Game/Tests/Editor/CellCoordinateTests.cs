using System.Linq;
using Match3Game.DataTypes;
using NUnit.Framework;

namespace Match3Game.Tests.Editor
{
    [TestFixture]
    public class CellCoordinateTests
    {
        [TestCase(0, 0, TestName = "Zeros")]
        [TestCase(1, -2, TestName = "Positive and negative values")]
        [TestCase(int.MinValue, int.MinValue, TestName = "MinValue")]
        [TestCase(int.MaxValue, int.MaxValue, TestName = "MaxValue")]
        public void Constructor_CalledWithValidArguments_CreatesInstance(int x, int y)
        {
            var instance = new CellCoordinate(x, y);

            Assert.AreEqual(x, instance.X);
            Assert.AreEqual(y, instance.Y);
        }

        [TestCase(0, 0, 1, 0, TestName ="Right top neighbour of even column" )]
        [TestCase(0, 0, -1, 0, TestName = "Left top neighbour of even column")] 
        [TestCase(0, 0, 1, -1, TestName = "Right bottom neighbour of even column")] 
        [TestCase(0, 0, -1, -1, TestName = "Left bottom neighbour of even column")] 
        [TestCase(0, 0, 0, 1, TestName = "Above neighbour of even column")] 
        [TestCase(0, 0, 0, -1, TestName = "Below neighbour of even column" )]
        [TestCase(1, 0, 2, 0, TestName = "Right top neighbour of odd column")]
        [TestCase(1, 0, 0, 0, TestName = "Left top neighbour of odd column")]
        [TestCase(1, 0, 2, 1, TestName = "Right bottom neighbour of odd column")]
        [TestCase(1, 0, 0, 1, TestName = "Left bottom neighbour of odd column")]
        [TestCase(1, 0, 1, 1, TestName = "Above neighbour of odd column")]
        [TestCase(1, 0, 1, -1, TestName = "Below neighbour of odd column")]
        [TestCase(1, 3, 2, 3, TestName = "Right top neighbour of further offset column")]
        [TestCase(1, 3, 0, 3, TestName = "Left top neighbour of further offset column")]
        [TestCase(1, 3, 2, 4, TestName = "Right bottom neighbour of further offset column")]
        [TestCase(1, 3, 0, 4, TestName = "Left bottom neighbour of further offset column")]
        [TestCase(1, 3, 1, 4, TestName = "Above neighbour of further offset column")]
        [TestCase(1, 3, 1, 2, TestName = "Below neighbour of further offset column")]
        public void IsNeighbour_CalledWithValidNeighbour_ReturnsTrue(int mainX, int mainY, int neighbourX, int neighbourY)
        {
            var main = new CellCoordinate(mainX, mainY);
            var neighbour = new CellCoordinate(neighbourX, neighbourY);

            Assert.IsTrue(main.IsNeighbour(neighbour));
        }

        [TestCase(0, 0, 1, 2)]
        [TestCase(0, 0, -1, 2)]
        [TestCase(0, 0, 2, -1)]
        [TestCase(0, 0, -2, -1)]
        [TestCase(4, 0, 1, 2)]
        [TestCase(4, 0, -1, 2)]
        [TestCase(4, 0, 2, -1)]
        [TestCase(4, 0, -2, -1)]
        [TestCase(4, -3, 1, 2)]
        [TestCase(4, -3, -1, 2)]
        [TestCase(4, -3, 2, -1)]
        [TestCase(4, -3, -2, -1)]
        public void IsNeighbour_CalledWithInvalidNeighbour_ReturnsFalse(int mainX, int mainY, int neighbourX, int neighbourY)
        {
            var main = new CellCoordinate(mainX, mainY);
            var neighbour = new CellCoordinate(neighbourX, neighbourY);

            Assert.IsFalse(main.IsNeighbour(neighbour));
        }

        [Test]
        public void GetNeighbours_ForEvenColumn_ReturnsExpectedNeighbours()
        {
            var main = new CellCoordinate(0, 0);

            var neighbours = main.GetNeighbours()
                .ToList();

            Assert.AreEqual(6, neighbours.Count);
            neighbours.Remove(new CellCoordinate(1, 0));
            neighbours.Remove(new CellCoordinate(1, -1));
            neighbours.Remove(new CellCoordinate(-1, 0));
            neighbours.Remove(new CellCoordinate(-1, -1));
            neighbours.Remove(new CellCoordinate(0, 1));
            neighbours.Remove(new CellCoordinate(0, -1));
            Assert.AreEqual(0, neighbours.Count);
        }

        [Test]
        public void GetNeighbours_ForOddColumn_ReturnsExpectedNeighbours()
        {
            var main = new CellCoordinate(1, 1);

            var neighbours = main.GetNeighbours()
                .ToList();

            Assert.AreEqual(6, neighbours.Count);
            neighbours.Remove(new CellCoordinate(2, 1));
            neighbours.Remove(new CellCoordinate(2, 2));
            neighbours.Remove(new CellCoordinate(0, 1));
            neighbours.Remove(new CellCoordinate(0, 2));
            neighbours.Remove(new CellCoordinate(1, 2));
            neighbours.Remove(new CellCoordinate(1, 0));
            Assert.AreEqual(0, neighbours.Count);
        }
    }
}
