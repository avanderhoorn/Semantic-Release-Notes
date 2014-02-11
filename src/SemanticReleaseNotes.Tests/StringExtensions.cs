using System.Linq;
using ApprovalTests;

namespace SemanticReleaseNotes.Tests
{
    public static class StringExtensions
    {
        public static string NormalizeLineEndings(this string src)
        {
            return (src ?? string.Empty).Replace("\r\n", "\n");
        }
    }
}
