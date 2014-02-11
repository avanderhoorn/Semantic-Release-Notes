using System;
using System.Linq;
using ApprovalTests;
using NUnit.Framework;

namespace SemanticReleaseNotes.Tests
{
    [TestFixture]
    public class TestCases
    {
        [Test]
        public void Summary()
        {
            var input = 
@"This is a _project_ summary with two paragraphs. 
Lorem ipsum dolor sit amet consectetuer **adipiscing** elit. 
Aliquam hendreritmi posuere lectus.

Vestibulum `enim wisi` viverra nec fringilla in laoreet
vitae risus. Donec sit amet nisl. Aliquam [semper](?) ipsum
sit amet velit.";

            var result = Parser.Parse(input);

            //Assert.AreEqual(input.NormalizeLineEndings(),result);
            Approvals.Verify(Parser.PrettyPrint(result));
        }

        [Test]
        public void ItemsAST()
        {
            var input = @"
- This is the _first_ *list* item.
- This is the **second** __list__ item.
- This is the `third` list item.
- This is the [forth](?) list item.";

            var result = Parser.ParseAST(input);

            Approvals.Verify(Parser.PrettyPrint(result));
        }

        [Test]
        public void PriorityAST()
        {
            var input = @"
1. This is a High priority list item.
1. This is a High priority list item. 
2. This is a Normal priority list item. 
1. This is a High priority list item. 
2. This is a Normal priority list item.
3. This is a Minor priority list item.
3. This is a Minor priority list item. ";

            var result = Parser.ParseAST(input);

            Approvals.Verify(Parser.PrettyPrint(result));
        }

        [Test]
        public void SectionsAST()
        {
            var input = @"
# Section
This is the summary for Section.
 - This is a Section scoped first list item.
 - This is a Section scoped second list item.

# Other Section
This is the summary for Other Section.
 - This is a Other Section scoped first list item.
 - This is a Other Section scoped second list item.
";

            var result = Parser.ParseAST(input);

            Approvals.Verify(Parser.PrettyPrint(result));
        }
    }
}