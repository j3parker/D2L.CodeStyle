using System;
using System.Collections.Generic;
using D2L.CodeStyle.Analyzers.Mutability.Goals;
using Microsoft.CodeAnalysis;

namespace D2L.CodeStyle.Analyzers.Mutability.Reducers.Utility {
	internal static class CompositeReducer {
		public static IEnumerable<Goal> Apply(
			SemanticModel model,
			Goal obligation
		) {
			return ApplyInternal( model, obligation as dynamic );
		} 

		private static IEnumerable<Goal> ApplyInternal(
			SemanticModel model,
			Goal obligation
		) {
			throw new NotImplementedException( "Missing overload for ApplyInternal" );
		} 

		#region Base Types
		private static IEnumerable<Goal> ApplyInternal(
			SemanticModel model,
			TypeGoal obligation
		) {
			return BaseTypeTactics.CombinedReducer.Apply( model, obligation );
		}
		#endregion

		#region Concrete Types
		private static IEnumerable<Goal> ApplyInternal(
			SemanticModel model,
			ConcreteTypeGoal obligation
		) {
			return ConcreteTypeTactics.CombinedTactic.Apply( model, obligation );
		}
		#endregion

		#region Classes
		private static IEnumerable<Goal> ApplyInternal(
			SemanticModel model,
			ClassGoal obligation
		) {
			return ClassAndStructReducers.Apply( model, obligation );
		}
		#endregion

		#region Structs
		private static IEnumerable<Goal> ApplyInternal(
			SemanticModel model,
			StructGoal obligation
		) {
			return ClassAndStructReducers.Apply( model, obligation );
		}
		#endregion

		#region Fields
		private static IEnumerable<Goal> ApplyInternal(
			SemanticModel model,
			FieldGoal obligation
		) {
			return FieldReducer.Apply( model, obligation );
		}
		#endregion

		#region Properties
		private static IEnumerable<Goal> ApplyInternal(
			SemanticModel model,
			PropertyGoal obligation
		) {
			return DefaultPropertyTactic.Apply( model, obligation );
		}
		#endregion

		#region Initializers
		private static IEnumerable<Goal> ApplyInternal(
			SemanticModel model,
			InitializerGoal obligation
		) {
			return InitializerTactics.CombinedReducer.Apply( model, obligation );
		}
		#endregion
	}
}
