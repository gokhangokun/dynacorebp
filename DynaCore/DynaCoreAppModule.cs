using System;

namespace DynaCore
{
    public class DynaCoreAppModule : IDynaCoreAppBuilder
    {
        private readonly DynaCoreAppBuilder _builder;

        protected DynaCoreAppModule(DynaCoreAppBuilder builder1)
        {
            _builder = builder1;
        }

        public void BeforeBuild(Action action)
        {
            _builder.BeforeBuild(action);
        }

        public DynaCoreApp Build()
        {
            return _builder.Build();
        }

        public void AfterBuild(Action action)
        {
            _builder.AfterBuild(action);
        }

        public DynaCoreAppBuilder Then()
        {
            return _builder;
        }
    }
}