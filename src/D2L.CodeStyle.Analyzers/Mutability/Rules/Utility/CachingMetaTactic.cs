using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using D2L.CodeStyle.Analyzers.Mutability.Goals;

namespace D2L.CodeStyle.Analyzers.Mutability.Tactics.Utility {
	/// <summary>
	/// A thread-safe cache of a tactic. Behaves the same as the inner tactic
	/// (assuming its deterministic) but faster.
	/// </summary>
	internal sealed class CachingMetaTactic {
		// the value should be either IEnumerable or an exception
		private readonly ConcurrentDictionary<Goal, object> m_cache;

		private readonly Func<SemanticModel, Goal, IEnumerable<Goal>> m_inner; 

		public CachingMetaTactic(
			Func<SemanticModel, Goal, IEnumerable<Goal>> inner
		) {
			m_inner = inner;
		}

		public IEnumerable<Goal> Apply(
			SemanticModel model,
			Goal goal
		) {
			// TODO
			return m_inner( model, goal );
		}
	}
}
