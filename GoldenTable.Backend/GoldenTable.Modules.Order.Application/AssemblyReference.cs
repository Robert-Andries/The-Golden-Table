using System.Reflection;

namespace GoldenTable.Modules.Order.Application;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}
