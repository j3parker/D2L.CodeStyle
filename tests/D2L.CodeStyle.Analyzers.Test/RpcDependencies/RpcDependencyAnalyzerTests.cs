﻿using D2L.CodeStyle.Analyzers.Test.Verifiers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;

namespace D2L.CodeStyle.Analyzers.RpcDependencies {
	[TestFixture]
	internal sealed class RpcDependencyAnalyzerTests : DiagnosticVerifier {
		private const string PREAMBLE = @"
namespace D2L.Web {
	interface IRpcContext {}
	class RpcAttribute : System.Attribute {}
}

namespace D2L.LP.Extensibility.Activation.Domain {
	class DependencyAttribute : System.Attribute {}
}";

		[Test]
		public void NormalMethod_NoDiag() {
			const string test = PREAMBLE + @"
namespace Test {
	class Test {
		public void Test( int x ) {}
	}
}";
			AssertNoDiagnostic( test );
		}

		[Test]
		public void MethodWithUnrelatedButAnnoyinglyNamedRpcAttribute_NoDiag() {
			const string test = PREAMBLE + @"
namespace Test {
	class RpcAttribute : System.Attribute {} // shadow the parent one
	class Test {
		[Rpc]
		public void Test() {}
	}
}";
			AssertNoDiagnostic( test );
		}

		[Test]
		public void MethodWithRpcAttributeButNoArguments_RpcContextDiag() {
			const string test = PREAMBLE + @"
namespace Test {
	using D2L.Web;

	class Test {
		[Rpc]
		public void Test() {}
	}
}";
			AssertSingleDiagnostic( RpcDependencyAnalyzer.RpcContextRule, test, 15, 19 );
		}

		[Test]
		public void MethodWithRpcAttributeAndIntFirstArgument_RpcContextDiag() {
			const string test = PREAMBLE + @"
namespace Test {
	using D2L.Web;

	class Test {
		[Rpc]
		public void Test( int x ) {}
	}
}";
			AssertSingleDiagnostic( RpcDependencyAnalyzer.RpcContextRule, test, 15, 21 );
		}

		[Test]
		public void MethodWithRpcAttributeAndIRpcContextFirstArg_NoDiag() {
			const string test = PREAMBLE + @"
namespace Test {
	using D2L.Web;

	class Test {
		[Rpc]
		public void Test( IRpcContext x ) {}
	}
}";
			AssertNoDiagnostic( test );
		}

		[Test]
		public void RpcWithoutDependencyArgs_NoDiag() {
			const string test = PREAMBLE + @"
namespace Test {
	using D2L.Web;
	using D2L.LP.Extensibility.Activation.Domain;

	class Test {
		[Rpc]
		public void Test( IRpcContext x, int x, int y ) {}
	}
}";
			AssertNoDiagnostic( test );
		}

		[Test]
		public void RpcWithSingleDependencyNoParams_NoDiag() {
			const string test = PREAMBLE + @"
namespace Test {
	using D2L.Web;
	using D2L.LP.Extensibility.Activation.Domain;

	class Test {
		[Rpc]
		public void Test( IRpcContext x, [Dependency] int x ) {}
	}
}";
			AssertNoDiagnostic( test );
		}

		[Test]
		public void RpcWithTwoDependencyNoParams_NoDiag() {
			const string test = PREAMBLE + @"
namespace Test {
	using D2L.Web;
	using D2L.LP.Extensibility.Activation.Domain;

	class Test {
		[Rpc]
		public void Test( IRpcContext x, [Dependency] int x, [Dependency] int y ) {}
	}
}";
			AssertNoDiagnostic( test );
		}

		[Test]
		public void RpcWithTwoDependencySingleParams_NoDiag() {
			const string test = PREAMBLE + @"
namespace Test {
	using D2L.Web;
	using D2L.LP.Extensibility.Activation.Domain;

	class Test {
		[Rpc]
		public void Test( IRpcContext x, [Dependency] int x, [Dependency] int y, int z ) {}
	}
}";
			AssertNoDiagnostic( test );
		}

		[Test]
		public void RpcWithTwoDependencySingleParamInTheMiddle_SortDiag() {
			const string test = PREAMBLE + @"
namespace Test {
	using D2L.Web;
	using D2L.LP.Extensibility.Activation.Domain;

	class Test {
		[Rpc]
		public void Test( IRpcContext x, [Dependency] int x, int y, [Dependency] int z ) {}
	}
}";
			AssertSingleDiagnostic( RpcDependencyAnalyzer.SortRule, test, 16, 63 );
		}

		private void AssertNoDiagnostic( string file ) {
			VerifyCSharpDiagnostic( file );
		}

		private void AssertSingleDiagnostic( DiagnosticDescriptor diag, string file, int line, int column ) {
			DiagnosticResult result = new DiagnosticResult {
				Id = diag.Id,
				Message = diag.MessageFormat.ToString(),
				Severity =  DiagnosticSeverity.Error,
				Locations = new [] {
					new DiagnosticResultLocation( "Test0.cs", line, column )
				}
			};

			VerifyCSharpDiagnostic( file, result );
		}

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() {
			return new RpcDependencyAnalyzer();
		}
	}
}
