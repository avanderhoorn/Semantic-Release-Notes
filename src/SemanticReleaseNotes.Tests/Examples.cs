using System.Linq;
using ApprovalTests;
using NUnit.Framework;

namespace SemanticReleaseNotes.Tests
{
    [TestFixture]
    public class Examples
    {
        [Test]
        public void Example_A()
        {
            var input = @"
Incremental release designed to provide an update to some of the core plugins.

 - Release Checker: Now gives you a breakdown of exactly what you are missing. +New
 - Structured Layout: An alternative layout engine that allows developers to control layout. +New
 - Timeline: Comes with an additional grid view to show the same data. +Changed
 - Ajax: Fix that crashed poll in Chrome and IE due to log/trace statement. +Fix
";
            var result = Parser.ParseAST(input);
            Approvals.Verify(Parser.PrettyPrint(result));
        }

        [Test]
        public void Example_B()
        {
            var input = @"
Incremental release designed to provide an update to some of the core plugins.

# System
 - *Release Checker*: Now gives you a breakdown of exactly what you are missing. +New
 - *Structured Layout*: An alternative layout engine that allows developers to control layout. +New
 
# Plugin
 - *Timeline*: Comes with an additional grid view to show the same data. +Changed
 - *Ajax*: Fix that crashed poll in Chrome and IE due to log/trace statement. +Fix
";
            var result = Parser.ParseAST(input);
            Approvals.Verify(Parser.PrettyPrint(result));
        }

        [Test]
        public void Example_C()
        {
            var input = @"
Incremental release designed to provide an update to some of the core plugins.
 - *Example*: You can have global issues that aren't grouped to a section

# System
This description is specific to system section.
 - *Release Checker*: Now gives you a +new breakdown of exactly what you are missing.
 - *Structured Layout*: A +new alternative layout engine that allows developers to control layout.

# Plugin
This description is specific to plugin section.
 - *Timeline*: Comes with an additional grid view to show the same data. +Changed
 - *Ajax*: +Fix that crashed poll in Chrome and IE due to log/trace statement. [[i1234][http://getglimpse.com]]
";
            var result = Parser.ParseAST(input);
            Approvals.Verify(Parser.PrettyPrint(result));
        }

        [Test]
        public void Example_D()
        {
            var input = @"
Incremental release designed to provide an update to some of the core plugins.
 1. *Example*: You can have global issues that aren't grouped to a section

# System [[icon][http://getglimpse.com/release/icon/core.png]]
This description is specific to system section.
 3. *Release Checker*: Now gives you a breakdown of exactly what you are missing. +New
 2. *Structured Layout*: An alternative layout engine that allows developers to control layout. +New

# Plugin [[icon][http://getglimpse.com/release/icon/mvc.png]]
This description is specific to plugin section.
 1. *Timeline*: Comes with an additional grid view to show the same data. +Changed
 1. *Ajax*: Fix that crashed poll in Chrome and IE due to log/trace statement. +Fix [[i1234][http://getglimpse.com]]
";
            var result = Parser.ParseAST(input);
            Approvals.Verify(Parser.PrettyPrint(result));
        }
        
    }
}