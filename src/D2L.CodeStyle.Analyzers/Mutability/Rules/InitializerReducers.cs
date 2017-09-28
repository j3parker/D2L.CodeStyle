using System.Collections.Generic;
using D2L.CodeStyle.Analyzers.Mutability.Goals;
using D2L.CodeStyle.Analyzers.Mutability.Reducers.Utility;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace D2L.CodeStyle.Analyzers.Mutability.Reducers {
	internal static class InitializerTactics {
		public static SequencedReducer<InitializerGoal> CombinedReducer = new SequencedReducer<InitializerGoal>(
			NewObjectReduce,
			DefaultReduce
		);

		public static IEnumerable<Goal> NewObjectReduce(
			SemanticModel model,
			InitializerGoal obligation
		) {
			if( !( obligation.Expression is ObjectCreationExpressionSyntax ) ) {
				yield return obligation;
			}

			yield return new ConcreteTypeGoal(
				GetType( model, obligation.Expression )
			);
		}

		public static IEnumerable<Goal> DefaultReduce(
			SemanticModel model,
			InitializerGoal obligation
		) {
			yield return new TypeGoal(
				GetType( model, obligation.Expression )
			);
		}

		private static ITypeSymbol GetType(
			SemanticModel model,
			ExpressionSyntax expr
		) {
			return model.GetTypeInfo( expr ).Type;
		}
	}
}
