using System;

namespace DynaCore
{
    public interface IDynaCoreAppBuilder
    {
        void BeforeBuild(Action action);

        DynaCoreApp Build();

        void AfterBuild(Action action);
    }
}