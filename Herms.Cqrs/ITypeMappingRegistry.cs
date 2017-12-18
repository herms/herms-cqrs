using System;
using System.Collections.Generic;
using Herms.Cqrs.Registration;

namespace Herms.Cqrs {
    public interface ITypeMappingRegistry
    {
        void Register(TypeMapping typeMapping);
        void Register(IEnumerable<TypeMapping> typeMappings);
        Type ResolveType(string name);
        string ResolveName(Type type);
    }
}