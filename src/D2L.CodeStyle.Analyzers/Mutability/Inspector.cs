using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace D2L.CodeStyle.Analyzers.Mutability {
	/*
	public sealed class Inspector {
		private readonly SemanticModel m_model;
		private readonly CachingMetaTactic m_cachingTactic;

		public Inspector( SemanticModel model ) {
			m_model = model;
			m_cachingTactic = new CachingMetaTactic( CompositeTactic.Apply );
		}

		public IEnumerable<SourceOfMutability> Inspect( IFieldSymbol field ) {
			return Inspect( new FieldObligation( field ) );
		}

		public IEnumerable<SourceOfMutability> Inspect( IPropertySymbol property ) {
			return Inspect( new PropertyObligation( property ) );
		}

		public IEnumerable<SourceOfMutability> InspectConcreteType( ITypeSymbol type ) {
			return Inspect( new ConcreteTypeObligation( type ) );
		}

		public IEnumerable<SourceOfMutability> InspectBaseType( ITypeSymbol type ) {
			return Inspect( new BaseTypeObligation( type ) );
		}

		private IEnumerable<SourceOfMutability> Inspect( Goal obligation ) {
			var obligations = new Stack<Goal>();
			obligations.Push( obligation );
			return Inspect( obligations );
		}

		private IEnumerable<SourceOfMutability> Inspect( Stack<Goal> obligations ) {
			var cycleDetector = new CycleDetectingMetaTactic();

			var tactic = new SequencedMetaTactic<Goal>(
				cycleDetector.Apply,
				m_cachingTactic.Apply
			);

			while ( obligations.Count != 0 ) {
				var obligation = obligations.Pop();

				// should try/catch?
				var result = tactic
					.Apply( m_model, obligation )
					.ToImmutableArray();

				if ( result.Length == 1 && result[0] == obligation ) {
					throw new Exception( "couldn't resolve obligation" );
				}

				foreach( var subObligation in obligations ) {
					obligations.Push( subObligation );

				}
			}

			// Not mutable!
			yield break;
		}
	}
	*/
}
