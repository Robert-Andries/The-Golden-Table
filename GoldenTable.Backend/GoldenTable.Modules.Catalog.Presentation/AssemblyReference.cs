using System.Reflection;

namespace GoldenTable.Modules.Catalog.Presentation;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}
