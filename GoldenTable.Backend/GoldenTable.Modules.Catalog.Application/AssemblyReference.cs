using System.Reflection;

namespace GoldenTable.Modules.Catalog.Application;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}
