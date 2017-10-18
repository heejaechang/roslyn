// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Test.Utilities;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.CSharp.UnitTests
{
    public partial class IOperationTests : SemanticModelTestBase
    {
        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(19927, "https://github.com/dotnet/roslyn/issues/19927")]
        public void TestPatternCaseClause_VarPatternDeclaration()
        {
            string source = @"
using System;
class X
{
    void M()
    {
        int? x = 12;
        switch (x)
        {
            /*<bind>*/case var y:/*</bind>*/
                break;
        }
    }
}
";
            string expectedOperationTree = @"
IPatternCaseClause (Label Symbol: case var y:) (CaseKind.Pattern) ([0] OperationKind.CaseClause) (Syntax: CasePatternSwitchLabel, 'case var y:') (Parent: SwitchCase)
  Pattern: 
    IDeclarationPattern (Declared Symbol: System.Int32? y) ([0] OperationKind.DeclarationPattern) (Syntax: DeclarationPattern, 'var y')
  Guard Expression: 
    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<CasePatternSwitchLabelSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(19927, "https://github.com/dotnet/roslyn/issues/19927")]
        public void TestPatternCaseClause_PrimitiveTypePatternDeclaration()
        {
            string source = @"
using System;
class X
{
    void M()
    {
        int? x = 12;
        switch (x)
        {
            /*<bind>*/case int y:/*</bind>*/
                break;
        }
    }
}
";
            string expectedOperationTree = @"
IPatternCaseClause (Label Symbol: case int y:) (CaseKind.Pattern) ([0] OperationKind.CaseClause) (Syntax: CasePatternSwitchLabel, 'case int y:') (Parent: SwitchCase)
  Pattern: 
    IDeclarationPattern (Declared Symbol: System.Int32 y) ([0] OperationKind.DeclarationPattern) (Syntax: DeclarationPattern, 'int y')
  Guard Expression: 
    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<CasePatternSwitchLabelSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(19927, "https://github.com/dotnet/roslyn/issues/19927")]
        public void TestPatternCaseClause_ReferenceTypePatternDeclaration()
        {
            string source = @"
using System;
class X
{
    void M(object x)
    {
        switch (x)
        {
            /*<bind>*/case X y:/*</bind>*/
                break;
        }
    }
}
";
            string expectedOperationTree = @"
IPatternCaseClause (Label Symbol: case X y:) (CaseKind.Pattern) ([0] OperationKind.CaseClause) (Syntax: CasePatternSwitchLabel, 'case X y:') (Parent: SwitchCase)
  Pattern: 
    IDeclarationPattern (Declared Symbol: X y) ([0] OperationKind.DeclarationPattern) (Syntax: DeclarationPattern, 'X y')
  Guard Expression: 
    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<CasePatternSwitchLabelSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(19927, "https://github.com/dotnet/roslyn/issues/19927")]
        public void TestPatternCaseClause_TypeParameterTypePatternDeclaration()
        {
            string source = @"
using System;
class X
{
    void M<T>(object x) where T : class
    {
        switch (x)
        {
            /*<bind>*/case T y:/*</bind>*/
                break;
        }
    }
}
";
            string expectedOperationTree = @"
IPatternCaseClause (Label Symbol: case T y:) (CaseKind.Pattern) ([0] OperationKind.CaseClause) (Syntax: CasePatternSwitchLabel, 'case T y:') (Parent: SwitchCase)
  Pattern: 
    IDeclarationPattern (Declared Symbol: T y) ([0] OperationKind.DeclarationPattern) (Syntax: DeclarationPattern, 'T y')
  Guard Expression: 
    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<CasePatternSwitchLabelSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(19927, "https://github.com/dotnet/roslyn/issues/19927")]
        public void TestPatternCaseClause_DynamicTypePatternDeclaration()
        {
            string source = @"
using System;
class X
{
    void M<T>(object x) where T : class
    {
        switch (x)
        {
            /*<bind>*/case dynamic y:/*</bind>*/
                break;
        }
    }
}
";
            string expectedOperationTree = @"
IPatternCaseClause (Label Symbol: case dynamic y:) (CaseKind.Pattern) ([0] OperationKind.CaseClause, IsInvalid) (Syntax: CasePatternSwitchLabel, 'case dynamic y:') (Parent: SwitchCase)
  Pattern: 
    IDeclarationPattern (Declared Symbol: dynamic y) ([0] OperationKind.DeclarationPattern, IsInvalid) (Syntax: DeclarationPattern, 'dynamic y')
  Guard Expression: 
    null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS8208: It is not legal to use the type 'dynamic' in a pattern.
                //             /*<bind>*/case dynamic y:/*</bind>*/
                Diagnostic(ErrorCode.ERR_PatternDynamicType, "dynamic").WithLocation(9, 28)
            };

            VerifyOperationTreeAndDiagnosticsForTest<CasePatternSwitchLabelSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(19927, "https://github.com/dotnet/roslyn/issues/19927")]
        public void TestPatternCaseClause_MixedDeclarationPatternAndConstantPatternClauses()
        {
            string source = @"
using System;
class X
{
    void M(object x)
    {
        switch (x)
        {
            case null:
                break;
            /*<bind>*/case X y:/*</bind>*/
                break;
        }
    }
}
";
            string expectedOperationTree = @"
IPatternCaseClause (Label Symbol: case X y:) (CaseKind.Pattern) ([0] OperationKind.CaseClause) (Syntax: CasePatternSwitchLabel, 'case X y:') (Parent: SwitchCase)
  Pattern: 
    IDeclarationPattern (Declared Symbol: X y) ([0] OperationKind.DeclarationPattern) (Syntax: DeclarationPattern, 'X y')
  Guard Expression: 
    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<CasePatternSwitchLabelSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(19927, "https://github.com/dotnet/roslyn/issues/19927")]
        public void TestPatternCaseClause_MixedDeclarationPatternAndConstantPatternClausesInSameSwitchSection()
        {
            string source = @"
using System;
class X
{
    void M(object x)
    {
        switch (x)
        {
            case null:
            /*<bind>*/case X y:/*</bind>*/
                break;
        }
    }
}
";
            string expectedOperationTree = @"
IPatternCaseClause (Label Symbol: case X y:) (CaseKind.Pattern) ([1] OperationKind.CaseClause) (Syntax: CasePatternSwitchLabel, 'case X y:') (Parent: SwitchCase)
  Pattern: 
    IDeclarationPattern (Declared Symbol: X y) ([0] OperationKind.DeclarationPattern) (Syntax: DeclarationPattern, 'X y')
  Guard Expression: 
    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<CasePatternSwitchLabelSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(19927, "https://github.com/dotnet/roslyn/issues/19927")]
        public void TestPatternCaseClause_MixedDeclarationPatternAndConstantPatternWithDefaultLabel()
        {
            string source = @"
using System;
class X
{
    void M(object x)
    {
        switch (x)
        {
            case null:
            /*<bind>*/case X y:/*</bind>*/
            default:
                break;
        }
    }
}
";
            string expectedOperationTree = @"
IPatternCaseClause (Label Symbol: case X y:) (CaseKind.Pattern) ([1] OperationKind.CaseClause) (Syntax: CasePatternSwitchLabel, 'case X y:') (Parent: SwitchCase)
  Pattern: 
    IDeclarationPattern (Declared Symbol: X y) ([0] OperationKind.DeclarationPattern) (Syntax: DeclarationPattern, 'X y')
  Guard Expression: 
    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<CasePatternSwitchLabelSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(19927, "https://github.com/dotnet/roslyn/issues/19927")]
        public void TestPatternCaseClause_GuardExpressionInPattern()
        {
            string source = @"
using System;
class X
{
    void M(object x)
    {
        switch (x)
        {
            /*<bind>*/case X y when x != null:/*</bind>*/
                break;
        }
    }
}
";
            string expectedOperationTree = @"
IPatternCaseClause (Label Symbol: case X y when x != null:) (CaseKind.Pattern) ([0] OperationKind.CaseClause) (Syntax: CasePatternSwitchLabel, 'case X y when x != null:') (Parent: SwitchCase)
  Pattern: 
    IDeclarationPattern (Declared Symbol: X y) ([0] OperationKind.DeclarationPattern) (Syntax: DeclarationPattern, 'X y')
  Guard Expression: 
    IBinaryOperatorExpression (BinaryOperatorKind.NotEquals) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: NotEqualsExpression, 'x != null')
      Left: 
        IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'x')
      Right: 
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Object, Constant: null, IsImplicit) (Syntax: NullLiteralExpression, 'null')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NullLiteralExpression, 'null')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<CasePatternSwitchLabelSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(19927, "https://github.com/dotnet/roslyn/issues/19927")]
        public void TestPatternCaseClause_PatternInGuardExpressionInPattern()
        {
            string source = @"
using System;
class X
{
    void M(object x)
    {
        switch (x)
        {
            /*<bind>*/case X y when x is X z :/*</bind>*/
                break;
        }
    }
}
";
            string expectedOperationTree = @"
IPatternCaseClause (Label Symbol: case X y when x is X z :) (CaseKind.Pattern) ([0] OperationKind.CaseClause) (Syntax: CasePatternSwitchLabel, 'case X y when x is X z :') (Parent: SwitchCase)
  Pattern: 
    IDeclarationPattern (Declared Symbol: X y) ([0] OperationKind.DeclarationPattern) (Syntax: DeclarationPattern, 'X y')
  Guard Expression: 
    IIsPatternExpression ([1] OperationKind.IsPatternExpression, Type: System.Boolean) (Syntax: IsPatternExpression, 'x is X z')
      Expression: 
        IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'x')
      Pattern: 
        IDeclarationPattern (Declared Symbol: X z) ([1] OperationKind.DeclarationPattern) (Syntax: DeclarationPattern, 'X z')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<CasePatternSwitchLabelSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(19927, "https://github.com/dotnet/roslyn/issues/19927")]
        public void TestPatternCaseClause_SyntaxErrorInGuardExpressionInPattern()
        {
            string source = @"
using System;
class X
{
    void M(object x)
    {
        switch (x)
        {
            /*<bind>*/case X y when :/*</bind>*/
                break;
        }
    }
}
";
            string expectedOperationTree = @"
IPatternCaseClause (Label Symbol: case X y when :) (CaseKind.Pattern) ([0] OperationKind.CaseClause, IsInvalid) (Syntax: CasePatternSwitchLabel, 'case X y when :') (Parent: SwitchCase)
  Pattern: 
    IDeclarationPattern (Declared Symbol: X y) ([0] OperationKind.DeclarationPattern) (Syntax: DeclarationPattern, 'X y')
  Guard Expression: 
    IInvalidExpression ([1] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
      Children(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1525: Invalid expression term ':'
                //             /*<bind>*/case X y when :/*</bind>*/
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, ":").WithArguments(":").WithLocation(9, 37)
            };

            VerifyOperationTreeAndDiagnosticsForTest<CasePatternSwitchLabelSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(19927, "https://github.com/dotnet/roslyn/issues/19927")]
        public void TestPatternCaseClause_SemanticErrorInGuardExpressionInPattern()
        {
            string source = @"
using System;
class X
{
    void M(object x)
    {
        switch (x)
        {
            /*<bind>*/case X y when x:/*</bind>*/
                break;
        }
    }
}
";
            string expectedOperationTree = @"
IPatternCaseClause (Label Symbol: case X y when x:) (CaseKind.Pattern) ([0] OperationKind.CaseClause, IsInvalid) (Syntax: CasePatternSwitchLabel, 'case X y when x:') (Parent: SwitchCase)
  Pattern: 
    IDeclarationPattern (Declared Symbol: X y) ([0] OperationKind.DeclarationPattern) (Syntax: DeclarationPattern, 'X y')
  Guard Expression: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Boolean, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'x')
      Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Object, IsInvalid) (Syntax: IdentifierName, 'x')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0266: Cannot implicitly convert type 'object' to 'bool'. An explicit conversion exists (are you missing a cast?)
                //             /*<bind>*/case X y when x:/*</bind>*/
                Diagnostic(ErrorCode.ERR_NoImplicitConvCast, "x").WithArguments("object", "bool").WithLocation(9, 37)
            };

            VerifyOperationTreeAndDiagnosticsForTest<CasePatternSwitchLabelSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(19927, "https://github.com/dotnet/roslyn/issues/19927")]
        public void TestPatternCaseClause_ConstantPattern()
        {
            string source = @"
using System;
class X
{
    void M(bool x)
    {
        switch (x)
        {
            case /*<bind>*/x is true/*</bind>*/:
                break;
        }
    }
}
";
            string expectedOperationTree = @"
IIsPatternExpression ([0] OperationKind.IsPatternExpression, Type: System.Boolean, IsInvalid) (Syntax: IsPatternExpression, 'x is true') (Parent: CaseClause)
  Expression: 
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Boolean, IsInvalid) (Syntax: IdentifierName, 'x')
  Pattern: 
    IConstantPattern ([1] OperationKind.ConstantPattern, IsInvalid) (Syntax: ConstantPattern, 'true')
      Value: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Boolean, Constant: True, IsInvalid) (Syntax: TrueLiteralExpression, 'true')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0150: A constant value is expected
                //             case /*<bind>*/x is true/*</bind>*/:
                Diagnostic(ErrorCode.ERR_ConstantExpected, "x is true").WithLocation(9, 28)
            };

            VerifyOperationTreeAndDiagnosticsForTest<IsPatternExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(19927, "https://github.com/dotnet/roslyn/issues/19927")]
        public void TestPatternCaseClause_DefaultLabel()
        {
            string source = @"
using System;
class X
{
    void M(object x)
    {
        switch (x)
        {
            case X y:
            /*<bind>*/default:/*</bind>*/
                break;
        }
    }
}
";
            string expectedOperationTree = @"
IDefaultCaseClause (CaseKind.Default) ([1] OperationKind.CaseClause) (Syntax: DefaultSwitchLabel, 'default:') (Parent: SwitchCase)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<DefaultSwitchLabelSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(19927, "https://github.com/dotnet/roslyn/issues/19927")]
        public void TestPatternCaseClause_InvalidTypeSwitch()
        {
            string source = @"
using System;
class X
{
    void M(object x)
    {
        switch (x.GetType())
        {
            /*<bind>*/case typeof(X):/*</bind>*/
                break;
        }
    }
}
";
            string expectedOperationTree = @"
IPatternCaseClause (Label Symbol: case typeof(X):) (CaseKind.Pattern) ([0] OperationKind.CaseClause, IsInvalid) (Syntax: CaseSwitchLabel, 'case typeof(X):') (Parent: SwitchCase)
  Pattern: 
    IConstantPattern ([0] OperationKind.ConstantPattern, IsInvalid) (Syntax: CaseSwitchLabel, 'case typeof(X):')
      Value: 
        ITypeOfExpression ([0] OperationKind.TypeOfExpression, Type: System.Type, IsInvalid) (Syntax: TypeOfExpression, 'typeof(X)')
          TypeOperand: X
  Guard Expression: 
    null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0150: A constant value is expected
                //             /*<bind>*/case typeof(X):/*</bind>*/
                Diagnostic(ErrorCode.ERR_ConstantExpected, "typeof(X)").WithLocation(9, 28),
                // CS0162: Unreachable code detected
                //                 break;
                Diagnostic(ErrorCode.WRN_UnreachableCode, "break").WithLocation(10, 17)
            };

            VerifyOperationTreeAndDiagnosticsForTest<CaseSwitchLabelSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(19927, "https://github.com/dotnet/roslyn/issues/19927")]
        public void TestPatternCaseClause_UndefinedTypeInPatternDeclaration()
        {
            string source = @"
using System;
class X
{
    void M(object x)
    {
        switch (x)
        {
            /*<bind>*/case UndefinedType y:/*</bind>*/
                break;
        }
    }
}
";
            string expectedOperationTree = @"
IPatternCaseClause (Label Symbol: case UndefinedType y:) (CaseKind.Pattern) ([0] OperationKind.CaseClause, IsInvalid) (Syntax: CasePatternSwitchLabel, 'case UndefinedType y:') (Parent: SwitchCase)
  Pattern: 
    IDeclarationPattern (Declared Symbol: UndefinedType y) ([0] OperationKind.DeclarationPattern, IsInvalid) (Syntax: DeclarationPattern, 'UndefinedType y')
  Guard Expression: 
    null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0246: The type or namespace name 'UndefinedType' could not be found (are you missing a using directive or an assembly reference?)
                //             /*<bind>*/case UndefinedType y:/*</bind>*/
                Diagnostic(ErrorCode.ERR_SingleTypeNameNotFound, "UndefinedType").WithArguments("UndefinedType").WithLocation(9, 28)
            };

            VerifyOperationTreeAndDiagnosticsForTest<CasePatternSwitchLabelSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(19927, "https://github.com/dotnet/roslyn/issues/19927")]
        public void TestPatternCaseClause_InvalidTypeInPatternDeclaration()
        {
            string source = @"
using System;
class X
{
    void M(int? x)
    {
        switch (x)
        {
            /*<bind>*/case X y:/*</bind>*/
                break;
        }
    }
}
";
            string expectedOperationTree = @"
IPatternCaseClause (Label Symbol: case X y:) (CaseKind.Pattern) ([0] OperationKind.CaseClause, IsInvalid) (Syntax: CasePatternSwitchLabel, 'case X y:') (Parent: SwitchCase)
  Pattern: 
    IDeclarationPattern (Declared Symbol: X y) ([0] OperationKind.DeclarationPattern, IsInvalid) (Syntax: DeclarationPattern, 'X y')
  Guard Expression: 
    null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // file.cs(9,28): error CS8121: An expression of type 'int?' cannot be handled by a pattern of type 'X'.
                //             /*<bind>*/case X y:/*</bind>*/
                Diagnostic(ErrorCode.ERR_PatternWrongType, "X").WithArguments("int?", "X").WithLocation(9, 28),
                // file.cs(10,17): warning CS0162: Unreachable code detected
                //                 break;
                Diagnostic(ErrorCode.WRN_UnreachableCode, "break").WithLocation(10, 17)
            };

            VerifyOperationTreeAndDiagnosticsForTest<CasePatternSwitchLabelSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(19927, "https://github.com/dotnet/roslyn/issues/19927")]
        public void TestPatternCaseClause_DuplicateLocalInPatternDeclaration()
        {
            string source = @"
using System;
class X
{
    void M(int? x)
    {
        int? y = 0;
        switch (x)
        {
            /*<bind>*/case int y:/*</bind>*/
                break;
        }
    }
}
";
            string expectedOperationTree = @"
IPatternCaseClause (Label Symbol: case int y:) (CaseKind.Pattern) ([0] OperationKind.CaseClause, IsInvalid) (Syntax: CasePatternSwitchLabel, 'case int y:') (Parent: SwitchCase)
  Pattern: 
    IDeclarationPattern (Declared Symbol: System.Int32 y) ([0] OperationKind.DeclarationPattern, IsInvalid) (Syntax: DeclarationPattern, 'int y')
  Guard Expression: 
    null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0136: A local or parameter named 'y' cannot be declared in this scope because that name is used in an enclosing local scope to define a local or parameter
                //             /*<bind>*/case int y:/*</bind>*/
                Diagnostic(ErrorCode.ERR_LocalIllegallyOverrides, "y").WithArguments("y").WithLocation(10, 32),
                // CS0219: The variable 'y' is assigned but its value is never used
                //         int? y = 0;
                Diagnostic(ErrorCode.WRN_UnreferencedVarAssg, "y").WithArguments("y").WithLocation(7, 14)
            };

            VerifyOperationTreeAndDiagnosticsForTest<CasePatternSwitchLabelSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(19927, "https://github.com/dotnet/roslyn/issues/19927")]
        public void TestPatternCaseClause_InvalidConstDeclarationInPatternDeclaration()
        {
            string source = @"
using System;
class X
{
    void M(int? x)
    {
        switch (x)
        {
            /*<bind>*/case /*</bind>*/const int y:
                break;
        }
    }
}
";
            string expectedOperationTree = @"
ISingleValueCaseClause (CaseKind.SingleValue) ([0] OperationKind.CaseClause, IsInvalid) (Syntax: CaseSwitchLabel, 'case /*</bind>*/') (Parent: SwitchCase)
  Value: 
    IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
      Children(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1525: Invalid expression term 'const'
                //             /*<bind>*/case /*</bind>*/const int y:
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, "const").WithArguments("const").WithLocation(9, 39),
                // CS1003: Syntax error, ':' expected
                //             /*<bind>*/case /*</bind>*/const int y:
                Diagnostic(ErrorCode.ERR_SyntaxError, "const").WithArguments(":", "const").WithLocation(9, 39),
                // CS0145: A const field requires a value to be provided
                //             /*<bind>*/case /*</bind>*/const int y:
                Diagnostic(ErrorCode.ERR_ConstValueRequired, "y").WithLocation(9, 49),
                // CS1002: ; expected
                //             /*<bind>*/case /*</bind>*/const int y:
                Diagnostic(ErrorCode.ERR_SemicolonExpected, ":").WithLocation(9, 50),
                // CS1513: } expected
                //             /*<bind>*/case /*</bind>*/const int y:
                Diagnostic(ErrorCode.ERR_RbraceExpected, ":").WithLocation(9, 50),
                // CS0168: The variable 'y' is declared but never used
                //             /*<bind>*/case /*</bind>*/const int y:
                Diagnostic(ErrorCode.WRN_UnreferencedVar, "y").WithArguments("y").WithLocation(9, 49)
            };

            VerifyOperationTreeAndDiagnosticsForTest<CaseSwitchLabelSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(19927, "https://github.com/dotnet/roslyn/issues/19927")]
        public void TestPatternCaseClause_RedundantPatternDeclarationClauses()
        {
            string source = @"
using System;
class X
{
    void M(object p)
    {
        /*<bind>*/switch (p)
        {
            case int x:
                break;
            case int y:
                break;
            case X z:
                break;
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
ISwitchStatement (3 cases) ([0] OperationKind.SwitchStatement, IsInvalid) (Syntax: SwitchStatement, 'switch (p) ... }') (Parent: BlockStatement)
  Switch expression: 
    IParameterReferenceExpression: p ([0] OperationKind.ParameterReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'p')
  Sections:
    ISwitchCase (1 case clauses, 1 statements) ([1] OperationKind.SwitchCase) (Syntax: SwitchSection, 'case int x: ... break;')
        Clauses:
          IPatternCaseClause (Label Symbol: case int x:) (CaseKind.Pattern) ([0] OperationKind.CaseClause) (Syntax: CasePatternSwitchLabel, 'case int x:')
            Pattern: 
              IDeclarationPattern (Declared Symbol: System.Int32 x) ([0] OperationKind.DeclarationPattern) (Syntax: DeclarationPattern, 'int x')
            Guard Expression: 
              null
        Body:
          IBranchStatement (BranchKind.Break) ([1] OperationKind.BranchStatement) (Syntax: BreakStatement, 'break;')
    ISwitchCase (1 case clauses, 1 statements) ([2] OperationKind.SwitchCase, IsInvalid) (Syntax: SwitchSection, 'case int y: ... break;')
        Clauses:
          IPatternCaseClause (Label Symbol: case int y:) (CaseKind.Pattern) ([0] OperationKind.CaseClause, IsInvalid) (Syntax: CasePatternSwitchLabel, 'case int y:')
            Pattern: 
              IDeclarationPattern (Declared Symbol: System.Int32 y) ([0] OperationKind.DeclarationPattern, IsInvalid) (Syntax: DeclarationPattern, 'int y')
            Guard Expression: 
              null
        Body:
          IBranchStatement (BranchKind.Break) ([1] OperationKind.BranchStatement) (Syntax: BreakStatement, 'break;')
    ISwitchCase (1 case clauses, 1 statements) ([3] OperationKind.SwitchCase) (Syntax: SwitchSection, 'case X z: ... break;')
        Clauses:
          IPatternCaseClause (Label Symbol: case X z:) (CaseKind.Pattern) ([0] OperationKind.CaseClause) (Syntax: CasePatternSwitchLabel, 'case X z:')
            Pattern: 
              IDeclarationPattern (Declared Symbol: X z) ([0] OperationKind.DeclarationPattern) (Syntax: DeclarationPattern, 'X z')
            Guard Expression: 
              null
        Body:
          IBranchStatement (BranchKind.Break) ([1] OperationKind.BranchStatement) (Syntax: BreakStatement, 'break;')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS8120: The switch case has already been handled by a previous case.
                //             case int y:
                Diagnostic(ErrorCode.ERR_PatternIsSubsumed, "int y").WithLocation(11, 18),
                // CS0162: Unreachable code detected
                //                 break;
                Diagnostic(ErrorCode.WRN_UnreachableCode, "break").WithLocation(12, 17)
            };

            VerifyOperationTreeAndDiagnosticsForTest<SwitchStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }
    }
}
