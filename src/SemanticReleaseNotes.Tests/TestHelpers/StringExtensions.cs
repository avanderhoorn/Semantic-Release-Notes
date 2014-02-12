using System.Linq;

namespace SemanticReleaseNotes.Tests.TestHelpers
{
    public static class StringExtensions
    {
        public static string NormalizeLineEndings(this string src)
        {
            return (src ?? string.Empty).Replace("\r\n", "\n");
        }
    }
}
