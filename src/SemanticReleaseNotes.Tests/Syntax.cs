using System;
using System.Linq;
using ApprovalTests;
using NUnit.Framework;

namespace SemanticReleaseNotes.Tests
{
    /// <summary>
    /// Syntax examples sourced from: http://semanticreleasenotes.org/#Syntax [Sat.Feb.8.2014]
    /// </summary>
    [TestFixture]
    public class Syntax
    {
        /// <summary>
        /// A summary is one or more paragraphs of text.
        /// </summary>
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

        /// <summary>
        /// Items are indicated via a standard Markdown 
        /// <i>ordered</i> or <i>unordered</i> list.
        /// </summary>
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

        /// <summary>
        /// Priority is indicated via a standard Markdown 
        /// <i>ordered list</i>.
        /// </summary>
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

        /// <summary>
        /// A Section can be arbitrary in nature and specific
        /// to the release notes of the application.  Sections
        /// are indicated via a standard Markdown <i>header</i>.
        /// </summary>
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

        /// <summary>
        /// <para>
        /// In some cases we want to have the one document that 
        /// describes many releases. In this case, the syntax 
        /// simply allows you to define a heading which is the 
        /// Version Number. If you use this feature in conjunction 
        /// with sections, Section headers are also altered.
        /// </para>
        /// <para>
        /// For the purposes of interpretation of the version number, 
        /// it is assumed that you are using Semantic Visioning - 
        /// http://semver.org/. Normal Markdown headers are used 
        /// to describe the version number for each release and 
        /// the scope of reach release (which item, etc makes up 
        /// the release).
        /// </para>
        /// </summary>
        [Test]
        public void Release()
        {
            Assert.Inconclusive("Examples not yet specified.");
        }
    }
}