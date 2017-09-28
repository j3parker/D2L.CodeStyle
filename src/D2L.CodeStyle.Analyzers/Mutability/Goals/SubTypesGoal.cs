using Microsoft.CodeAnalysis;

namespace D2L.CodeStyle.Analyzers.Mutability.Goals {
	internal sealed class SubTypesGoal : Goal {
		public SubTypesGoal( ITypeSymbol type ) {
			Type = type;
		}

		public ITypeSymbol Type { get; }
	}
}
