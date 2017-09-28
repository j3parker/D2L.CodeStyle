using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using D2L.CodeStyle.Analyzers.Mutability.Goals;

namespace D2L.CodeStyle.Analyzers.Mutability.Tactics.Utility {
	internal static class AnnotatedImmutableTactic {
		public static IEnumerable<Goal> Apply(
			SemanticModel model,
			TypeGoal goal
		) {
			return Apply( model, goal, goal.Type );
		}

		public static IEnumerable<Goal> Apply(
			SemanticModel model,
			ConcreteTypeGoal goal
		) {
			return Apply( model, goal, goal.Type );
		}

		private static IEnumerable<Goal> Apply(
			SemanticModel model,
			Goal goal,
			ITypeSymbol type
		) {
			yield return goal; // TODO
		} 
	}
}
