using System.Reflection;

namespace GoldenTable.Modules.Orders.Infrastructure;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}
