using Microsoft.CodeAnalysis;

namespace D2L.CodeStyle.Analyzers.Mutability.Goals {
	internal sealed class StructGoal : Goal {
		public StructGoal( ITypeSymbol type ) {
			Type = type;
		}

		public ITypeSymbol Type { get; }
	}
}