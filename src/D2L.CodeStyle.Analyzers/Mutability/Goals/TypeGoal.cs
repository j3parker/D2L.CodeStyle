using Microsoft.CodeAnalysis;

namespace D2L.CodeStyle.Analyzers.Mutability.Goals {
	internal sealed class TypeGoal : Goal {
		public TypeGoal( ITypeSymbol type ) {
			Type = type;
		}

		public ITypeSymbol Type { get; }
	}
}
