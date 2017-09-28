using System.Collections.Generic;
using D2L.CodeStyle.Analyzers.Mutability.Goals;
using Microsoft.CodeAnalysis;

namespace D2L.CodeStyle.Analyzers.Mutability.Reducers.Utility {
	/// <summary>
	/// Handles things like ImmutableArray&lt;T&gt; by emitting a
	/// BaseTypeObligation of type T.
	/// </summary>
	internal sealed class ContainerTypeReducer {
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
			Goal originalGoal,
			ITypeSymbol type
		) {
			yield return originalGoal;
		} 
	}
}
