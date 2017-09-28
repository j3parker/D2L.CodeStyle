using Microsoft.CodeAnalysis;

namespace D2L.CodeStyle.Analyzers.Mutability.Goals {
	internal sealed class ConcreteTypeGoal : Goal {
		public ConcreteTypeGoal( ITypeSymbol type ) {
			Type = type;
		}

		public ITypeSymbol Type { get; }
	}
}
