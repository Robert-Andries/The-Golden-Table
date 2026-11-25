using System.Reflection;

namespace GoldenTable.Modules.Order.Infrastructure;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}
