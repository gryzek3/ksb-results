using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Fluent;

namespace KSB.Results
{
    public static class ConfigureTypings
    {
        public static void Configure(Reinforced.Typings.Fluent.ConfigurationBuilder builder)
        {
            var global = builder.Context.Global;
            global.UnresolvedToUnknown = true;
            global.DiscardNamespacesWhenUsingModules = true;
            global.RootNamespace = "";
            builder.Global(c => c.UseModules(true, true));
            builder.SubstituteGeneric(typeof(Nullable<>), (t, r) => new RtSimpleTypeName($"{r.ResolveTypeName(t.GenericTypeArguments.First())} | null"));

        }
    }
}
