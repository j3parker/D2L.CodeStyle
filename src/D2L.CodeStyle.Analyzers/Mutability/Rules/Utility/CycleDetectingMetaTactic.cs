using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using D2L.CodeStyle.Analyzers.Mutability.Goals;

namespace D2L.CodeStyle.Analyzers.Mutability.Tactics.Utility {
	/// <summary>
	/// Catches cycles including ConcreteTypeObligation or BaseTypeObligation.
	/// Types can contain themselves, but this never results in mutability that
	/// wouldn't otherwise be there.
	/// </summary>
	internal sealed class CycleDetectingMetaTactic {
		private readonly HashSet<Goal> m_seen = new HashSet<Goal>(); 

		public IEnumerable<Goal> Apply(
			SemanticModel model,
			Goal goal
		) {
			if ( goal is ConcreteTypeGoal || goal is TypeGoal ) {
				if( m_seen.Contains( goal )) {
					// Discard this obligation if we've seen it before
					yield break;
				}

				// Remember that we've seen it
				m_seen.Add( goal );
			}

			yield return goal;
		}
	}
}
