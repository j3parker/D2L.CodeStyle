using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using D2L.CodeStyle.Analyzers.Mutability.Goals;
using Microsoft.CodeAnalysis;

namespace D2L.CodeStyle.Analyzers.Mutability.Reducers.Utility {
	internal class SequencedReducer<TGoal>
		where TGoal : Goal
	{
		private readonly ImmutableArray<Func<SemanticModel, TGoal, IEnumerable<Goal>>> m_reducers;

		public SequencedReducer(
			params Func<SemanticModel, TGoal, IEnumerable<Goal>>[] reducers
		) {
			m_reducers = reducers.ToImmutableArray();
		}
		
		public IEnumerable<Goal> Apply(
			SemanticModel model,
			TGoal obligation
		) {
			foreach( var reducer in m_reducers ) {
				var result = reducer( model, obligation ).ToImmutableArray();

				if ( result.Length == 1 && result[0] == obligation ) {
					continue;
				}

				return result;
			}

			// goal couldn't be reduced
			return ImmutableArray.Create( obligation );
		}
	}
}
