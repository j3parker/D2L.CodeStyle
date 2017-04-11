using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace D2L.CodeStyle.Analyzers.RpcDependencies {
	[DiagnosticAnalyzer( LanguageNames.CSharp )]
	internal sealed class RpcDependencyAnalyzer : DiagnosticAnalyzer {
		internal static readonly DiagnosticDescriptor RpcContextRule = new DiagnosticDescriptor(
			id: "D2L0004",
			title: "RPCs must take an IRpcContext as their first argument",
			messageFormat: "RPCs must take an IRpcContext as their first argument",
			category: "Correctness",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "RPCs must take an IRpcContext as their first argument"
		);

		internal static readonly DiagnosticDescriptor SortRule = new DiagnosticDescriptor(
			id: "D2L0005",
			title: "Dependency-injected arguments must be properly sorted",
			messageFormat: "Dependency-injected must come before RPC parameters but after IRpcContext",
			category: "Correctness",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "Dependency-injected must come before RPC parameters but after IRpcContext"
		);

		internal static readonly DiagnosticDescriptor DependencyAttributeRule = new DiagnosticDescriptor(
			id: "D2L0006",
			title: "Argument cannot come from RPC parameters. Are you missing a [Dependency] attribute?",
			messageFormat: "Argument cannot come from RPC parameters. Are you missing a [Dependency] attribute?",
			category: "Correctness",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "Argument cannot come from RPC parameters. Are you missing a [Dependency] attribute?"
		);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create( RpcContextRule, SortRule );

		public override void Initialize( AnalysisContext context ) {
			context.RegisterSyntaxNodeAction( AnalyzeMethod, SyntaxKind.MethodDeclaration );
		}

		private static void AnalyzeMethod( SyntaxNodeAnalysisContext context ) {
			var method = context.Node as MethodDeclarationSyntax;

			if ( method == null ) {
				return;
			}

			var ps = method.ParameterList.Parameters;

			bool done = CheckThatFirstArgumentIsIRpcContext( context, method, ps );

			if ( done ) {
				return;
			}

			CheckThatDependencyArgumentsAreSortedCorrectly( context, ps );

			// other things to check:
			// - appropriate use of [Dependency]
		}

		private static bool CheckThatFirstArgumentIsIRpcContext(
			SyntaxNodeAnalysisContext context,
			MethodDeclarationSyntax method,
			SeparatedSyntaxList<ParameterSyntax> ps
		) {
			// Could this be cached per-compilation?
			var rpcAttributeType = context.Compilation.GetTypeByMetadataName( "D2L.Web.RpcAttribute" );

			bool isRpc = method
				.AttributeLists
				.SelectMany( al => al.Attributes )
				.Any( attr => IsAttribute( rpcAttributeType, attr, context.SemanticModel ) );

			if( !isRpc ) {
				return true;
			}

			if ( ps.Count == 0 ) {
				context.ReportDiagnostic( Diagnostic.Create( RpcContextRule, method.ParameterList.GetLocation() ) );
				return true;
			}

			var firstParam = method.ParameterList.Parameters[0];

			var rpcContextType = context.Compilation.GetTypeByMetadataName( "D2L.Web.IRpcContext" );

			if ( !ParameterIsOfTypeIRpcContext( rpcContextType, firstParam, context.SemanticModel ) ) {
				context.ReportDiagnostic( Diagnostic.Create( RpcContextRule, firstParam.GetLocation() ) );
			}

			return false;
		}

		private static void CheckThatDependencyArgumentsAreSortedCorrectly(
			SyntaxNodeAnalysisContext context,
			SeparatedSyntaxList<ParameterSyntax> ps
		) {
			var dependencyAttributeType = context.Compilation.GetTypeByMetadataName( "D2L.LP.Extensibility.Activation.Domain.DependencyAttribute" );

			bool doneDependencies = false;
			foreach( var param in ps.Skip( 1 ) ){
				var isDep = param
					.AttributeLists
					.SelectMany( al => al.Attributes )
					.Any( attr => IsAttribute( dependencyAttributeType, attr, context.SemanticModel ) );

				if( !isDep && !doneDependencies ) {
					doneDependencies = true;
				} else if ( isDep && doneDependencies ) {
					context.ReportDiagnostic( Diagnostic.Create( SortRule, param.GetLocation() ) );
				}
			}
		}

		private static bool IsAttribute( INamedTypeSymbol expectedType, AttributeSyntax attr, SemanticModel model ) {
			var symbol = model.GetSymbolInfo( attr ).Symbol;

			if ( symbol == null || symbol.Kind == SymbolKind.ErrorType ) {
				return false;
			}

			return symbol.ContainingType.Equals( expectedType );
		}

		private static bool ParameterIsOfTypeIRpcContext( INamedTypeSymbol expectedType, ParameterSyntax param, SemanticModel model ) {
			var symbol = model.GetSymbolInfo( param.Type ).Symbol;

			if ( symbol == null || symbol.Kind == SymbolKind.ErrorType ) {
				return false;
			}

			return symbol.Equals( expectedType );
		}
	}
}