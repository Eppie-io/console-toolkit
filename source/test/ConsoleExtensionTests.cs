using NUnit.Framework;

namespace Tuvi.Toolkit.Cli.CommandLine.Test
{
    [TestFixture]
    public class ConsoleExtensionTests
    {
        [Test]
        public void ReadMultiLineReturnsNullWhenNoInput()
        {
            var input = new StringReader("EOF" + Environment.NewLine);
            Console.SetIn(input);
            var result = ConsoleExtension.ReadMultiLine("Enter text:");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void ReadMultiLineReturnsLinesJoinedWhenInputProvided()
        {
            var lines = new[] { "line1", "line2", "EOF" };
            var input = new StringReader(string.Join(Environment.NewLine, lines) + Environment.NewLine);
            Console.SetIn(input);
            var result = ConsoleExtension.ReadMultiLine("Enter text:");
            Assert.That(result, Is.EqualTo("line1" + Environment.NewLine + "line2"));
        }

        [Test]
        public void ReadMultiLineUsesCustomEndMarker()
        {
            var lines = new[] { "foo", "bar", "END" };
            var input = new StringReader(string.Join(Environment.NewLine, lines) + Environment.NewLine);
            Console.SetIn(input);
            var result = ConsoleExtension.ReadMultiLine("Enter text:", endMarker: "END");
            Assert.That(result, Is.EqualTo("foo" + Environment.NewLine + "bar"));
        }

        [Test]
        public void ReadMultiLineReturnsNullWhenFirstLineIsEndMarker()
        {
            var input = new StringReader("END" + Environment.NewLine);
            Console.SetIn(input);
            var result = ConsoleExtension.ReadMultiLine("Enter text:", endMarker: "END");
            Assert.That(result, Is.Null);
        }

        [TestCase("")]
        [TestCase("   ")]
        [TestCase("          ")]
        public void ReadMultiLineThrowsArgumentNullExceptionWhenEndMarkerIsWhitespace(string invalidEndMarker)
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                ConsoleExtension.ReadMultiLine("Enter text:", invalidEndMarker)
            );

            Assert.That(ex.ParamName, Is.EqualTo("endMarker"));
        }

        [Test]
        public void ReadMultiLineReturnsNullWhenReadLineReturnsNull()
        {
            var input = new StringReader("");
            Console.SetIn(input);

            var result = ConsoleExtension.ReadMultiLine("Enter text:");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void ReadMultiLineReturnsEmptyStringWhenUserInputsEmptyLineThenEndMarker()
        {
            var input = new StringReader(Environment.NewLine + "EOF" + Environment.NewLine);
            Console.SetIn(input);

            var result = ConsoleExtension.ReadMultiLine("Enter text:");
            Assert.That(result, Is.EqualTo("")); 
        }
    }
}
