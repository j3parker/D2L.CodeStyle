using Microsoft.CodeAnalysis;

namespace D2L.CodeStyle.Analyzers.Mutability.Goals {
	internal sealed class FieldGoal : Goal {
		public FieldGoal( IFieldSymbol field ) {
			Field = field;
		}

		public IFieldSymbol Field { get; }
	}
}