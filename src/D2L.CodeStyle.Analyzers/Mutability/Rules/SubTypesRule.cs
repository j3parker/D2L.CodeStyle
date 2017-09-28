using System.Collections.Generic;
using D2L.CodeStyle.Analyzers.Mutability.Goals;
using Microsoft.CodeAnalysis;

namespace D2L.CodeStyle.Analyzers.Mutability.Rules {
	internal static class SubTypesRule {
		public static IEnumerable<Goal> Apply(
			SemanticModel model,
			SubTypesGoal goal
		) {
			if ( goal.Type.IsSealed ) {
				yield break;
			}

			// TODO: if it's private or internal we can find all subtypes and
			// emit a TypeGoal
		} 
	}
}
