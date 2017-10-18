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
        [Fact]
        public void VerifyLiftedBinaryOperators1()
        {
            var source = @"
class C
{
    void F(int? x, int? y)
    {
        var z = /*<bind>*/x + y/*</bind>*/;
    }
}";

            string expectedOperationTree =
@"
IBinaryOperatorExpression (BinaryOperatorKind.Add, IsLifted) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32?) (Syntax: AddExpression, 'x + y') (Parent: VariableInitializer)
  Left: 
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32?) (Syntax: IdentifierName, 'x')
  Right: 
    IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32?) (Syntax: IdentifierName, 'y')
";

            VerifyOperationTreeForTest<BinaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void VerifyNonLiftedBinaryOperators1()
        {
            var source = @"
class C
{
    void F(int x, int y)
    {
        var z = /*<bind>*/x + y/*</bind>*/;
    }
}";

            string expectedOperationTree =
@"
IBinaryOperatorExpression (BinaryOperatorKind.Add) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'x + y') (Parent: VariableInitializer)
  Left: 
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
  Right: 
    IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
";

            VerifyOperationTreeForTest<BinaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void VerifyLiftedCheckedBinaryOperators1()
        {
            string source = @"
class C
{
    void F(int? x, int? y)
    {
        checked
        {
            var z = /*<bind>*/x + y/*</bind>*/;
        }
    }
}
";
            string expectedOperationTree = @"
IBinaryOperatorExpression (BinaryOperatorKind.Add, IsLifted, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32?) (Syntax: AddExpression, 'x + y') (Parent: VariableInitializer)
  Left: 
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32?) (Syntax: IdentifierName, 'x')
  Right: 
    IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32?) (Syntax: IdentifierName, 'y')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<BinaryExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void VerifyNonLiftedCheckedBinaryOperators1()
        {
            string source = @"
class C
{
    void F(int x, int y)
    {
        checked
        {
            var z = /*<bind>*/x + y/*</bind>*/;
        }
    }
}
";
            string expectedOperationTree = @"
IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'x + y') (Parent: VariableInitializer)
  Left: 
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
  Right: 
    IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<BinaryExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void VerifyLiftedUserDefinedBinaryOperators1()
        {
            var source = @"
struct C
{
    public static C operator +(C c1, C c2) { }
    void F(C? x, C? y)
    {
        var z = /*<bind>*/x + y/*</bind>*/;
    }
}";

            string expectedOperationTree =
@"
IBinaryOperatorExpression (BinaryOperatorKind.Add, IsLifted) (OperatorMethod: C C.op_Addition(C c1, C c2)) ([0] OperationKind.BinaryOperatorExpression, Type: C?) (Syntax: AddExpression, 'x + y') (Parent: VariableInitializer)
  Left: 
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: C?) (Syntax: IdentifierName, 'x')
  Right: 
    IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: C?) (Syntax: IdentifierName, 'y')
";

            VerifyOperationTreeForTest<BinaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void VerifyNonLiftedUserDefinedBinaryOperators1()
        {
            var source = @"
struct C
{
    public static C operator +(C c1, C c2) { }
    void F(C x, C y)
    {
        var z = /*<bind>*/x + y/*</bind>*/;
    }
}";

            string expectedOperationTree =
@"
IBinaryOperatorExpression (BinaryOperatorKind.Add) (OperatorMethod: C C.op_Addition(C c1, C c2)) ([0] OperationKind.BinaryOperatorExpression, Type: C) (Syntax: AddExpression, 'x + y') (Parent: VariableInitializer)
  Left: 
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'x')
  Right: 
    IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'y')
";

            VerifyOperationTreeForTest<BinaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void TestBinaryOperators()
        {
            string source = @"
using System;
class C
{
    void M(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p)
    {
        /*<bind>*/Console.WriteLine(
            (a >> 10) + (b << 20) - c * d / e % f & g |
            h ^ (i == (j != ((((k < l ? 1 : 0) > m ? 1 : 0) <= o ? 1 : 0) >= p ? 1 : 0) ? 1 : 0) ? 1 : 0))/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IInvocationExpression (void System.Console.WriteLine(System.Int32 value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... ) ? 1 : 0))') (Parent: ExpressionStatement)
  Instance Receiver: 
    null
  Arguments(1):
    IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, '(a >> 10) + ... 0) ? 1 : 0)')
      IBinaryOperatorExpression (BinaryOperatorKind.Or) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: BitwiseOrExpression, '(a >> 10) + ... 0) ? 1 : 0)')
        Left: 
          IBinaryOperatorExpression (BinaryOperatorKind.And) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: BitwiseAndExpression, '(a >> 10) + ... / e % f & g')
            Left: 
              IBinaryOperatorExpression (BinaryOperatorKind.Subtract) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: SubtractExpression, '(a >> 10) + ... * d / e % f')
                Left: 
                  IBinaryOperatorExpression (BinaryOperatorKind.Add) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, '(a >> 10) + (b << 20)')
                    Left: 
                      IBinaryOperatorExpression (BinaryOperatorKind.RightShift) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: RightShiftExpression, 'a >> 10')
                        Left: 
                          IParameterReferenceExpression: a ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'a')
                        Right: 
                          ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
                    Right: 
                      IBinaryOperatorExpression (BinaryOperatorKind.LeftShift) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: LeftShiftExpression, 'b << 20')
                        Left: 
                          IParameterReferenceExpression: b ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'b')
                        Right: 
                          ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 20) (Syntax: NumericLiteralExpression, '20')
                Right: 
                  IBinaryOperatorExpression (BinaryOperatorKind.Remainder) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: ModuloExpression, 'c * d / e % f')
                    Left: 
                      IBinaryOperatorExpression (BinaryOperatorKind.Divide) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: DivideExpression, 'c * d / e')
                        Left: 
                          IBinaryOperatorExpression (BinaryOperatorKind.Multiply) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: MultiplyExpression, 'c * d')
                            Left: 
                              IParameterReferenceExpression: c ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'c')
                            Right: 
                              IParameterReferenceExpression: d ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'd')
                        Right: 
                          IParameterReferenceExpression: e ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'e')
                    Right: 
                      IParameterReferenceExpression: f ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'f')
            Right: 
              IParameterReferenceExpression: g ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'g')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.ExclusiveOr) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: ExclusiveOrExpression, 'h ^ (i == ( ... 0) ? 1 : 0)')
            Left: 
              IParameterReferenceExpression: h ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'h')
            Right: 
              IConditionalExpression ([1] OperationKind.ConditionalExpression, Type: System.Int32) (Syntax: ConditionalExpression, 'i == (j !=  ...  0) ? 1 : 0')
                Condition: 
                  IBinaryOperatorExpression (BinaryOperatorKind.Equals) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: EqualsExpression, 'i == (j !=  ... 0) ? 1 : 0)')
                    Left: 
                      IParameterReferenceExpression: i ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                    Right: 
                      IConditionalExpression ([1] OperationKind.ConditionalExpression, Type: System.Int32) (Syntax: ConditionalExpression, 'j != ((((k  ...  0) ? 1 : 0')
                        Condition: 
                          IBinaryOperatorExpression (BinaryOperatorKind.NotEquals) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: NotEqualsExpression, 'j != ((((k  ...  p ? 1 : 0)')
                            Left: 
                              IParameterReferenceExpression: j ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
                            Right: 
                              IConditionalExpression ([1] OperationKind.ConditionalExpression, Type: System.Int32) (Syntax: ConditionalExpression, '(((k < l ?  ... = p ? 1 : 0')
                                Condition: 
                                  IBinaryOperatorExpression (BinaryOperatorKind.GreaterThanOrEqual) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanOrEqualExpression, '(((k < l ?  ... 1 : 0) >= p')
                                    Left: 
                                      IConditionalExpression ([0] OperationKind.ConditionalExpression, Type: System.Int32) (Syntax: ConditionalExpression, '((k < l ? 1 ... = o ? 1 : 0')
                                        Condition: 
                                          IBinaryOperatorExpression (BinaryOperatorKind.LessThanOrEqual) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanOrEqualExpression, '((k < l ? 1 ... 1 : 0) <= o')
                                            Left: 
                                              IConditionalExpression ([0] OperationKind.ConditionalExpression, Type: System.Int32) (Syntax: ConditionalExpression, '(k < l ? 1  ... > m ? 1 : 0')
                                                Condition: 
                                                  IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, '(k < l ? 1 : 0) > m')
                                                    Left: 
                                                      IConditionalExpression ([0] OperationKind.ConditionalExpression, Type: System.Int32) (Syntax: ConditionalExpression, 'k < l ? 1 : 0')
                                                        Condition: 
                                                          IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'k < l')
                                                            Left: 
                                                              IParameterReferenceExpression: k ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'k')
                                                            Right: 
                                                              IParameterReferenceExpression: l ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'l')
                                                        WhenTrue: 
                                                          ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                                                        WhenFalse: 
                                                          ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
                                                    Right: 
                                                      IParameterReferenceExpression: m ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'm')
                                                WhenTrue: 
                                                  ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                                                WhenFalse: 
                                                  ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
                                            Right: 
                                              IParameterReferenceExpression: o ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'o')
                                        WhenTrue: 
                                          ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                                        WhenFalse: 
                                          ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
                                    Right: 
                                      IParameterReferenceExpression: p ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'p')
                                WhenTrue: 
                                  ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                                WhenFalse: 
                                  ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
                        WhenTrue: 
                          ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                        WhenFalse: 
                          ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
                WhenTrue: 
                  ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                WhenFalse: 
                  ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void TestBinaryOperators_Checked()
        {
            string source = @"
using System;
class C
{
    void M(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p)
    {
        checked
        {
            /*<bind>*/Console.WriteLine(
                (a >> 10) + (b << 20) - c * d / e % f & g |
                h ^ (i == (j != ((((k < l ? 1 : 0) > m ? 1 : 0) <= o ? 1 : 0) >= p ? 1 : 0) ? 1 : 0) ? 1 : 0))/*</bind>*/;
        }
    }
}
";
            string expectedOperationTree = @"
IInvocationExpression (void System.Console.WriteLine(System.Int32 value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... ) ? 1 : 0))') (Parent: ExpressionStatement)
  Instance Receiver: 
    null
  Arguments(1):
    IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, '(a >> 10) + ... 0) ? 1 : 0)')
      IBinaryOperatorExpression (BinaryOperatorKind.Or) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: BitwiseOrExpression, '(a >> 10) + ... 0) ? 1 : 0)')
        Left: 
          IBinaryOperatorExpression (BinaryOperatorKind.And) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: BitwiseAndExpression, '(a >> 10) + ... / e % f & g')
            Left: 
              IBinaryOperatorExpression (BinaryOperatorKind.Subtract, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: SubtractExpression, '(a >> 10) + ... * d / e % f')
                Left: 
                  IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, '(a >> 10) + (b << 20)')
                    Left: 
                      IBinaryOperatorExpression (BinaryOperatorKind.RightShift) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: RightShiftExpression, 'a >> 10')
                        Left: 
                          IParameterReferenceExpression: a ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'a')
                        Right: 
                          ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
                    Right: 
                      IBinaryOperatorExpression (BinaryOperatorKind.LeftShift) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: LeftShiftExpression, 'b << 20')
                        Left: 
                          IParameterReferenceExpression: b ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'b')
                        Right: 
                          ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 20) (Syntax: NumericLiteralExpression, '20')
                Right: 
                  IBinaryOperatorExpression (BinaryOperatorKind.Remainder) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: ModuloExpression, 'c * d / e % f')
                    Left: 
                      IBinaryOperatorExpression (BinaryOperatorKind.Divide, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: DivideExpression, 'c * d / e')
                        Left: 
                          IBinaryOperatorExpression (BinaryOperatorKind.Multiply, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: MultiplyExpression, 'c * d')
                            Left: 
                              IParameterReferenceExpression: c ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'c')
                            Right: 
                              IParameterReferenceExpression: d ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'd')
                        Right: 
                          IParameterReferenceExpression: e ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'e')
                    Right: 
                      IParameterReferenceExpression: f ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'f')
            Right: 
              IParameterReferenceExpression: g ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'g')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.ExclusiveOr) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: ExclusiveOrExpression, 'h ^ (i == ( ... 0) ? 1 : 0)')
            Left: 
              IParameterReferenceExpression: h ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'h')
            Right: 
              IConditionalExpression ([1] OperationKind.ConditionalExpression, Type: System.Int32) (Syntax: ConditionalExpression, 'i == (j !=  ...  0) ? 1 : 0')
                Condition: 
                  IBinaryOperatorExpression (BinaryOperatorKind.Equals) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: EqualsExpression, 'i == (j !=  ... 0) ? 1 : 0)')
                    Left: 
                      IParameterReferenceExpression: i ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                    Right: 
                      IConditionalExpression ([1] OperationKind.ConditionalExpression, Type: System.Int32) (Syntax: ConditionalExpression, 'j != ((((k  ...  0) ? 1 : 0')
                        Condition: 
                          IBinaryOperatorExpression (BinaryOperatorKind.NotEquals) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: NotEqualsExpression, 'j != ((((k  ...  p ? 1 : 0)')
                            Left: 
                              IParameterReferenceExpression: j ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
                            Right: 
                              IConditionalExpression ([1] OperationKind.ConditionalExpression, Type: System.Int32) (Syntax: ConditionalExpression, '(((k < l ?  ... = p ? 1 : 0')
                                Condition: 
                                  IBinaryOperatorExpression (BinaryOperatorKind.GreaterThanOrEqual) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanOrEqualExpression, '(((k < l ?  ... 1 : 0) >= p')
                                    Left: 
                                      IConditionalExpression ([0] OperationKind.ConditionalExpression, Type: System.Int32) (Syntax: ConditionalExpression, '((k < l ? 1 ... = o ? 1 : 0')
                                        Condition: 
                                          IBinaryOperatorExpression (BinaryOperatorKind.LessThanOrEqual) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanOrEqualExpression, '((k < l ? 1 ... 1 : 0) <= o')
                                            Left: 
                                              IConditionalExpression ([0] OperationKind.ConditionalExpression, Type: System.Int32) (Syntax: ConditionalExpression, '(k < l ? 1  ... > m ? 1 : 0')
                                                Condition: 
                                                  IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, '(k < l ? 1 : 0) > m')
                                                    Left: 
                                                      IConditionalExpression ([0] OperationKind.ConditionalExpression, Type: System.Int32) (Syntax: ConditionalExpression, 'k < l ? 1 : 0')
                                                        Condition: 
                                                          IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'k < l')
                                                            Left: 
                                                              IParameterReferenceExpression: k ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'k')
                                                            Right: 
                                                              IParameterReferenceExpression: l ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'l')
                                                        WhenTrue: 
                                                          ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                                                        WhenFalse: 
                                                          ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
                                                    Right: 
                                                      IParameterReferenceExpression: m ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'm')
                                                WhenTrue: 
                                                  ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                                                WhenFalse: 
                                                  ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
                                            Right: 
                                              IParameterReferenceExpression: o ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'o')
                                        WhenTrue: 
                                          ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                                        WhenFalse: 
                                          ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
                                    Right: 
                                      IParameterReferenceExpression: p ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'p')
                                WhenTrue: 
                                  ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                                WhenFalse: 
                                  ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
                        WhenTrue: 
                          ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                        WhenFalse: 
                          ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
                WhenTrue: 
                  ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                WhenFalse: 
                  ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }
    }
}
