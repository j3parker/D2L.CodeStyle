using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace D2L.CodeStyle.Analyzers.Mutability.Goals {
	internal sealed class InitializerGoal : Goal {
		public InitializerGoal( ExpressionSyntax expr ) {
			Expression = expr;
		}

		public ExpressionSyntax Expression { get; }
	}
}
