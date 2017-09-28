using Microsoft.CodeAnalysis;

namespace D2L.CodeStyle.Analyzers.Mutability.Goals {
	internal sealed class ClassGoal : Goal {
		public ClassGoal( ITypeSymbol type ) {
			Type = type;
		}

		public ITypeSymbol Type { get; }
	}
}
