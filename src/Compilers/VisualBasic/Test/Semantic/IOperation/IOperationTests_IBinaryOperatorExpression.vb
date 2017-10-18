﻿' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CodeAnalysis.Test.Utilities
Imports Roslyn.Test.Utilities

Namespace Microsoft.CodeAnalysis.VisualBasic.UnitTests.Semantics

    Partial Public Class IOperationTests
        Inherits SemanticModelTestBase

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub VerifyLiftedBinaryOperators1()
            Dim source = <![CDATA[
Class C
    Sub F(x as Integer?, y as Integer?)
        dim z = x + y 'BIND:"x + y"
    End Sub
End Class
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBinaryOperatorExpression (BinaryOperatorKind.Add, IsLifted, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Nullable(Of System.Int32)) (Syntax: AddExpression, 'x + y') (Parent: VariableInitializer)
  Left: 
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Nullable(Of System.Int32)) (Syntax: IdentifierName, 'x')
  Right: 
    IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.Nullable(Of System.Int32)) (Syntax: IdentifierName, 'y')
]]>.Value

            VerifyOperationTreeForTest(Of BinaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub VerifyNonLiftedBinaryOperators1()
            Dim source = <![CDATA[
Class C
    Sub F(x as Integer, y as Integer)
        dim z = x + y 'BIND:"x + y"
    End Sub
End Class
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'x + y') (Parent: VariableInitializer)
  Left: 
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
  Right: 
    IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
]]>.Value

            VerifyOperationTreeForTest(Of BinaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub VerifyLiftedUserDefinedShortCircuitBinaryOperators1()
            Dim source = <![CDATA[
Structure C
    Public Shared Operator And(c1 as C, cs as C) as C
    End Operator

    Public Shared Operator IsFalse(c1 as C) as Boolean
    End Operator

    Sub F(x as C?, y as C?)
        dim z = x And y 'BIND:"x And y"
    End Sub
End Structure
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBinaryOperatorExpression (BinaryOperatorKind.And, IsLifted, Checked) (OperatorMethod: Function C.op_BitwiseAnd(c1 As C, cs As C) As C) ([0] OperationKind.BinaryOperatorExpression, Type: System.Nullable(Of C)) (Syntax: AndExpression, 'x And y') (Parent: VariableInitializer)
  Left: 
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Nullable(Of C)) (Syntax: IdentifierName, 'x')
  Right: 
    IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.Nullable(Of C)) (Syntax: IdentifierName, 'y')
]]>.Value

            VerifyOperationTreeForTest(Of BinaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub VerifyLiftedUserDefinedShortCircuitBinaryOperators2()
            Dim source = <![CDATA[
Structure C
    Public Shared Operator And(c1 as C, cs as C) as C
    End Operator

    Public Shared Operator IsFalse(c1 as C) as Boolean
    End Operator

    Sub F(x as C?, y as C?)
        dim z = x AndAlso y 'BIND:"x AndAlso y"
    End Sub
End Structure
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBinaryOperatorExpression (BinaryOperatorKind.ConditionalAnd, IsLifted) (OperatorMethod: Function C.op_BitwiseAnd(c1 As C, cs As C) As C) ([0] OperationKind.BinaryOperatorExpression, Type: System.Nullable(Of C)) (Syntax: AndAlsoExpression, 'x AndAlso y') (Parent: VariableInitializer)
  Left: 
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Nullable(Of C)) (Syntax: IdentifierName, 'x')
  Right: 
    IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.Nullable(Of C)) (Syntax: IdentifierName, 'y')
]]>.Value

            VerifyOperationTreeForTest(Of BinaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub VerifyNonLiftedUserDefinedShortCircuitBinaryOperators1()
            Dim source = <![CDATA[
Structure C
    Public Shared Operator And(c1 as C, cs as C) as C
    End Operator

    Public Shared Operator IsFalse(c1 as C) as Boolean
    End Operator

    Sub F(x as C, y as C)
        dim z = x And y 'BIND:"x And y"
    End Sub
End Structure
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBinaryOperatorExpression (BinaryOperatorKind.And, Checked) (OperatorMethod: Function C.op_BitwiseAnd(c1 As C, cs As C) As C) ([0] OperationKind.BinaryOperatorExpression, Type: C) (Syntax: AndExpression, 'x And y') (Parent: VariableInitializer)
  Left: 
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'x')
  Right: 
    IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'y')
]]>.Value

            VerifyOperationTreeForTest(Of BinaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub VerifyNonLiftedUserDefinedShortCircuitBinaryOperators2()
            Dim source = <![CDATA[
Structure C
    Public Shared Operator And(c1 as C, cs as C) as C
    End Operator

    Public Shared Operator IsFalse(c1 as C) as Boolean
    End Operator

    Sub F(x as C, y as C)
        dim z = x AndAlso y 'BIND:"x AndAlso y"
    End Sub
End Structure
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBinaryOperatorExpression (BinaryOperatorKind.ConditionalAnd) (OperatorMethod: Function C.op_BitwiseAnd(c1 As C, cs As C) As C) ([0] OperationKind.BinaryOperatorExpression, Type: C) (Syntax: AndAlsoExpression, 'x AndAlso y') (Parent: VariableInitializer)
  Left: 
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'x')
  Right: 
    IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'y')
]]>.Value

            VerifyOperationTreeForTest(Of BinaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub VerifyLiftedUserDefinedBinaryOperators1()
            Dim source = <![CDATA[
Structure C
    Public Shared Operator + (c1 as C, c2 as C) as C
    End Operator

    Sub F(x as C?, y as C?)
        dim z = x + y 'BIND:"x + y"
    End Sub
End Structure
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBinaryOperatorExpression (BinaryOperatorKind.Add, IsLifted, Checked) (OperatorMethod: Function C.op_Addition(c1 As C, c2 As C) As C) ([0] OperationKind.BinaryOperatorExpression, Type: System.Nullable(Of C)) (Syntax: AddExpression, 'x + y') (Parent: VariableInitializer)
  Left: 
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Nullable(Of C)) (Syntax: IdentifierName, 'x')
  Right: 
    IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.Nullable(Of C)) (Syntax: IdentifierName, 'y')
]]>.Value

            VerifyOperationTreeForTest(Of BinaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub VerifyNonLiftedUserDefinedBinaryOperators1()
            Dim source = <![CDATA[
Structure C
    Public Shared Operator + (c1 as C, c2 as C) as C
    End Operator

    Sub F(x as C, y as C)
        dim z = x + y 'BIND:"x + y"
    End Sub
End Structure
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) (OperatorMethod: Function C.op_Addition(c1 As C, c2 As C) As C) ([0] OperationKind.BinaryOperatorExpression, Type: C) (Syntax: AddExpression, 'x + y') (Parent: VariableInitializer)
  Left: 
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'x')
  Right: 
    IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'y')
]]>.Value

            VerifyOperationTreeForTest(Of BinaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TestBinaryOperators()
            Dim source = <![CDATA[
Module Module1

    Sub Main()'BIND:"Sub Main()"
        Dim x, y As New Integer
        Dim r As Integer
        r = x + y
        r = x - y
        r = x * y
        r = x / y
        r = x \ y
        r = x Mod y
        r = x ^ y
        r = x = y
        r = x <> y
        r = x < y
        r = x > y
        r = x <= y
        r = x >= y
        r = x Like y
        r = x & y
        r = x And y
        r = x Or y
        r = x Xor y
        r = x << 2
        r = x >> 3
        r = DirectCast(x, Object) = y
        r = DirectCast(x, Object) <> y
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBlockStatement (26 statements, 3 locals) ([Root] OperationKind.BlockStatement) (Syntax: SubBlock, 'Sub Main()' ... End Sub') (Parent: )
  Locals: Local_1: x As System.Int32
    Local_2: y As System.Int32
    Local_3: r As System.Int32
  IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim x, y As New Integer')
    IVariableDeclaration (2 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'x, y As New Integer')
      Variables: Local_1: x As System.Int32
        Local_2: y As System.Int32
      Initializer: 
        IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: AsNewClause, 'As New Integer')
          IObjectCreationExpression (Constructor: Sub System.Int32..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: System.Int32) (Syntax: ObjectCreationExpression, 'New Integer')
            Arguments(0)
            Initializer: 
              null
  IVariableDeclarationStatement (1 declarations) ([1] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim r As Integer')
    IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'r')
      Variables: Local_1: r As System.Int32
      Initializer: 
        null
  IExpressionStatement ([2] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x + y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x + y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'x + y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([3] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x - y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x - y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.Subtract, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: SubtractExpression, 'x - y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([4] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x * y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x * y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.Multiply, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: MultiplyExpression, 'x * y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([5] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x / y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x / y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: DivideExpression, 'x / y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.Divide, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Double) (Syntax: DivideExpression, 'x / y')
                Left: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Double, IsImplicit) (Syntax: IdentifierName, 'x')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                Right: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Double, IsImplicit) (Syntax: IdentifierName, 'y')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([6] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x \ y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x \ y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.IntegerDivide, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: IntegerDivideExpression, 'x \ y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([7] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x Mod y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x Mod y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.Remainder, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: ModuloExpression, 'x Mod y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([8] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x ^ y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x ^ y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: ExponentiateExpression, 'x ^ y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.Power, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Double) (Syntax: ExponentiateExpression, 'x ^ y')
                Left: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Double, IsImplicit) (Syntax: IdentifierName, 'x')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                Right: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Double, IsImplicit) (Syntax: IdentifierName, 'y')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([9] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x = y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x = y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: EqualsExpression, 'x = y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.Equals, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: EqualsExpression, 'x = y')
                Left: 
                  ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                Right: 
                  ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([10] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x <> y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x <> y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: NotEqualsExpression, 'x <> y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.NotEquals, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: NotEqualsExpression, 'x <> y')
                Left: 
                  ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                Right: 
                  ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([11] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x < y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x < y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: LessThanExpression, 'x < y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.LessThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'x < y')
                Left: 
                  ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                Right: 
                  ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([12] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x > y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x > y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: GreaterThanExpression, 'x > y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'x > y')
                Left: 
                  ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                Right: 
                  ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([13] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x <= y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x <= y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: LessThanOrEqualExpression, 'x <= y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.LessThanOrEqual, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanOrEqualExpression, 'x <= y')
                Left: 
                  ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                Right: 
                  ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([14] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x >= y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x >= y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: GreaterThanOrEqualExpression, 'x >= y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.GreaterThanOrEqual, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanOrEqualExpression, 'x >= y')
                Left: 
                  ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                Right: 
                  ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([15] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x Like y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x Like y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: LikeExpression, 'x Like y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.Like, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LikeExpression, 'x Like y')
                Left: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.String, IsImplicit) (Syntax: IdentifierName, 'x')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                Right: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.String, IsImplicit) (Syntax: IdentifierName, 'y')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([16] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x & y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x & y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: ConcatenateExpression, 'x & y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.Concatenate, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.String) (Syntax: ConcatenateExpression, 'x & y')
                Left: 
                  IConversionExpression (Explicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.String) (Syntax: IdentifierName, 'x')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                Right: 
                  IConversionExpression (Explicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.String) (Syntax: IdentifierName, 'y')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([17] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x And y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x And y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.And, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AndExpression, 'x And y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([18] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x Or y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x Or y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.Or, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: OrExpression, 'x Or y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([19] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x Xor y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x Xor y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.ExclusiveOr, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: ExclusiveOrExpression, 'x Xor y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([20] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x << 2')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x << 2')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.LeftShift, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: LeftShiftExpression, 'x << 2')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
  IExpressionStatement ([21] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x >> 3')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x >> 3')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.RightShift, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: RightShiftExpression, 'x >> 3')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3) (Syntax: NumericLiteralExpression, '3')
  IExpressionStatement ([22] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = DirectC ... Object) = y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = DirectC ... Object) = y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: EqualsExpression, 'DirectCast( ... Object) = y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.ObjectValueEquals, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Object) (Syntax: EqualsExpression, 'DirectCast( ... Object) = y')
                Left: 
                  IConversionExpression (Explicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object) (Syntax: DirectCastExpression, 'DirectCast(x, Object)')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                Right: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: IdentifierName, 'y')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([23] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = DirectC ... bject) <> y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = DirectC ... bject) <> y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: NotEqualsExpression, 'DirectCast( ... bject) <> y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.ObjectValueNotEquals, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Object) (Syntax: NotEqualsExpression, 'DirectCast( ... bject) <> y')
                Left: 
                  IConversionExpression (Explicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object) (Syntax: DirectCastExpression, 'DirectCast(x, Object)')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                Right: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: IdentifierName, 'y')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  ILabeledStatement (Label: exit) ([24] OperationKind.LabeledStatement, IsImplicit) (Syntax: EndSubStatement, 'End Sub')
    Statement: 
      null
  IReturnStatement ([25] OperationKind.ReturnStatement, IsImplicit) (Syntax: EndSubStatement, 'End Sub')
    ReturnedValue: 
      null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of MethodBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TestBinaryOperators_Unchecked()
            Dim source = <![CDATA[
Module Module1

    Sub Main()'BIND:"Sub Main()"
        Dim x, y As New Integer
        Dim r As Integer
        r = x + y
        r = x - y
        r = x * y
        r = x / y
        r = x \ y
        r = x Mod y
        r = x ^ y
        r = x = y
        r = x <> y
        r = x < y
        r = x > y
        r = x <= y
        r = x >= y
        r = x Like y
        r = x & y
        r = x And y
        r = x Or y
        r = x Xor y
        r = x << 2
        r = x >> 3
        r = DirectCast(x, Object) = y
        r = DirectCast(x, Object) <> y
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBlockStatement (26 statements, 3 locals) ([Root] OperationKind.BlockStatement) (Syntax: SubBlock, 'Sub Main()' ... End Sub') (Parent: )
  Locals: Local_1: x As System.Int32
    Local_2: y As System.Int32
    Local_3: r As System.Int32
  IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim x, y As New Integer')
    IVariableDeclaration (2 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'x, y As New Integer')
      Variables: Local_1: x As System.Int32
        Local_2: y As System.Int32
      Initializer: 
        IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: AsNewClause, 'As New Integer')
          IObjectCreationExpression (Constructor: Sub System.Int32..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: System.Int32) (Syntax: ObjectCreationExpression, 'New Integer')
            Arguments(0)
            Initializer: 
              null
  IVariableDeclarationStatement (1 declarations) ([1] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim r As Integer')
    IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'r')
      Variables: Local_1: r As System.Int32
      Initializer: 
        null
  IExpressionStatement ([2] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x + y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x + y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'x + y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([3] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x - y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x - y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.Subtract) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: SubtractExpression, 'x - y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([4] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x * y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x * y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.Multiply) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: MultiplyExpression, 'x * y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([5] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x / y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x / y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: DivideExpression, 'x / y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.Divide) ([0] OperationKind.BinaryOperatorExpression, Type: System.Double) (Syntax: DivideExpression, 'x / y')
                Left: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Double, IsImplicit) (Syntax: IdentifierName, 'x')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                Right: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Double, IsImplicit) (Syntax: IdentifierName, 'y')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([6] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x \ y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x \ y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.IntegerDivide) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: IntegerDivideExpression, 'x \ y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([7] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x Mod y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x Mod y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.Remainder) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: ModuloExpression, 'x Mod y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([8] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x ^ y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x ^ y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: ExponentiateExpression, 'x ^ y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.Power) ([0] OperationKind.BinaryOperatorExpression, Type: System.Double) (Syntax: ExponentiateExpression, 'x ^ y')
                Left: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Double, IsImplicit) (Syntax: IdentifierName, 'x')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                Right: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Double, IsImplicit) (Syntax: IdentifierName, 'y')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([9] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x = y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x = y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: EqualsExpression, 'x = y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.Equals) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: EqualsExpression, 'x = y')
                Left: 
                  ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                Right: 
                  ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([10] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x <> y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x <> y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: NotEqualsExpression, 'x <> y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.NotEquals) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: NotEqualsExpression, 'x <> y')
                Left: 
                  ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                Right: 
                  ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([11] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x < y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x < y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: LessThanExpression, 'x < y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'x < y')
                Left: 
                  ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                Right: 
                  ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([12] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x > y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x > y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: GreaterThanExpression, 'x > y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'x > y')
                Left: 
                  ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                Right: 
                  ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([13] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x <= y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x <= y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: LessThanOrEqualExpression, 'x <= y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.LessThanOrEqual) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanOrEqualExpression, 'x <= y')
                Left: 
                  ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                Right: 
                  ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([14] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x >= y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x >= y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: GreaterThanOrEqualExpression, 'x >= y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.GreaterThanOrEqual) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanOrEqualExpression, 'x >= y')
                Left: 
                  ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                Right: 
                  ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([15] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x Like y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x Like y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: LikeExpression, 'x Like y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.Like) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LikeExpression, 'x Like y')
                Left: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.String, IsImplicit) (Syntax: IdentifierName, 'x')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                Right: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.String, IsImplicit) (Syntax: IdentifierName, 'y')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([16] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x & y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x & y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: ConcatenateExpression, 'x & y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.Concatenate) ([0] OperationKind.BinaryOperatorExpression, Type: System.String) (Syntax: ConcatenateExpression, 'x & y')
                Left: 
                  IConversionExpression (Explicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.String) (Syntax: IdentifierName, 'x')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                Right: 
                  IConversionExpression (Explicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.String) (Syntax: IdentifierName, 'y')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([17] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x And y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x And y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.And) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AndExpression, 'x And y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([18] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x Or y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x Or y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.Or) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: OrExpression, 'x Or y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([19] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x Xor y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x Xor y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.ExclusiveOr) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: ExclusiveOrExpression, 'x Xor y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([20] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x << 2')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x << 2')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.LeftShift) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: LeftShiftExpression, 'x << 2')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
  IExpressionStatement ([21] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x >> 3')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x >> 3')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.RightShift) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: RightShiftExpression, 'x >> 3')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3) (Syntax: NumericLiteralExpression, '3')
  IExpressionStatement ([22] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = DirectC ... Object) = y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = DirectC ... Object) = y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: EqualsExpression, 'DirectCast( ... Object) = y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.ObjectValueEquals) ([0] OperationKind.BinaryOperatorExpression, Type: System.Object) (Syntax: EqualsExpression, 'DirectCast( ... Object) = y')
                Left: 
                  IConversionExpression (Explicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object) (Syntax: DirectCastExpression, 'DirectCast(x, Object)')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                Right: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: IdentifierName, 'y')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([23] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = DirectC ... bject) <> y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = DirectC ... bject) <> y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: NotEqualsExpression, 'DirectCast( ... bject) <> y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.ObjectValueNotEquals) ([0] OperationKind.BinaryOperatorExpression, Type: System.Object) (Syntax: NotEqualsExpression, 'DirectCast( ... bject) <> y')
                Left: 
                  IConversionExpression (Explicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object) (Syntax: DirectCastExpression, 'DirectCast(x, Object)')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                Right: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: IdentifierName, 'y')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  ILabeledStatement (Label: exit) ([24] OperationKind.LabeledStatement, IsImplicit) (Syntax: EndSubStatement, 'End Sub')
    Statement: 
      null
  IReturnStatement ([25] OperationKind.ReturnStatement, IsImplicit) (Syntax: EndSubStatement, 'End Sub')
    ReturnedValue: 
      null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            Dim fileName = "a.vb"
            Dim syntaxTree = Parse(source, fileName)
            Dim references = DefaultVbReferences.Concat({ValueTupleRef, SystemRuntimeFacadeRef})
            Dim compilation = CreateCompilationWithMscorlib45AndVBRuntime({syntaxTree}, references:=references, options:=TestOptions.ReleaseDll.WithOverflowChecks(False))

            VerifyOperationTreeAndDiagnosticsForTest(Of MethodBlockSyntax)(compilation, fileName, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub TestBinaryOperator_CompareText()
            Dim source = <![CDATA[
Option Compare Text

Class C
    Sub M(x As String, y As String, r As Integer)'BIND:"Sub M(x As String, y As String, r As Integer)"
        r = x = y
        r = x <> y
        r = x < y
        r = x > y
        r = x <= y
        r = x >= y
        r = DirectCast(x, Object) = y
        r = DirectCast(x, Object) <> y
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBlockStatement (10 statements) ([Root] OperationKind.BlockStatement) (Syntax: SubBlock, 'Sub M(x As  ... End Sub') (Parent: )
  IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x = y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x = y')
        Left: 
          IParameterReferenceExpression: r ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: EqualsExpression, 'x = y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.Equals, Checked, CompareText) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: EqualsExpression, 'x = y')
                Left: 
                  IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'x')
                Right: 
                  IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x <> y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x <> y')
        Left: 
          IParameterReferenceExpression: r ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: NotEqualsExpression, 'x <> y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.NotEquals, Checked, CompareText) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: NotEqualsExpression, 'x <> y')
                Left: 
                  IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'x')
                Right: 
                  IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([2] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x < y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x < y')
        Left: 
          IParameterReferenceExpression: r ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: LessThanExpression, 'x < y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.LessThan, Checked, CompareText) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'x < y')
                Left: 
                  IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'x')
                Right: 
                  IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([3] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x > y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x > y')
        Left: 
          IParameterReferenceExpression: r ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: GreaterThanExpression, 'x > y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked, CompareText) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'x > y')
                Left: 
                  IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'x')
                Right: 
                  IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([4] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x <= y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x <= y')
        Left: 
          IParameterReferenceExpression: r ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: LessThanOrEqualExpression, 'x <= y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.LessThanOrEqual, Checked, CompareText) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanOrEqualExpression, 'x <= y')
                Left: 
                  IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'x')
                Right: 
                  IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([5] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x >= y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = x >= y')
        Left: 
          IParameterReferenceExpression: r ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: GreaterThanOrEqualExpression, 'x >= y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.GreaterThanOrEqual, Checked, CompareText) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanOrEqualExpression, 'x >= y')
                Left: 
                  IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'x')
                Right: 
                  IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([6] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = DirectC ... Object) = y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = DirectC ... Object) = y')
        Left: 
          IParameterReferenceExpression: r ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: EqualsExpression, 'DirectCast( ... Object) = y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.ObjectValueEquals, Checked, CompareText) ([0] OperationKind.BinaryOperatorExpression, Type: System.Object) (Syntax: EqualsExpression, 'DirectCast( ... Object) = y')
                Left: 
                  IConversionExpression (Explicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object) (Syntax: DirectCastExpression, 'DirectCast(x, Object)')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'x')
                Right: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: IdentifierName, 'y')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      IParameterReferenceExpression: y ([0] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([7] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = DirectC ... bject) <> y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'r = DirectC ... bject) <> y')
        Left: 
          IParameterReferenceExpression: r ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: NotEqualsExpression, 'DirectCast( ... bject) <> y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.ObjectValueNotEquals, Checked, CompareText) ([0] OperationKind.BinaryOperatorExpression, Type: System.Object) (Syntax: NotEqualsExpression, 'DirectCast( ... bject) <> y')
                Left: 
                  IConversionExpression (Explicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object) (Syntax: DirectCastExpression, 'DirectCast(x, Object)')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'x')
                Right: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: IdentifierName, 'y')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      IParameterReferenceExpression: y ([0] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'y')
  ILabeledStatement (Label: exit) ([8] OperationKind.LabeledStatement, IsImplicit) (Syntax: EndSubStatement, 'End Sub')
    Statement: 
      null
  IReturnStatement ([9] OperationKind.ReturnStatement, IsImplicit) (Syntax: EndSubStatement, 'End Sub')
    ReturnedValue: 
      null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of MethodBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TestBinaryCompoundOperators()
            Dim source = <![CDATA[
Module Module1

    Sub Main()'BIND:"Sub Main()"
        Dim x, y As New Integer
        x += y
        x -= y
        x *= y
        x /= y
        x \= y
        x ^= y
        x &= y
        x <<= 2
        x >>= 3
    End Sub
End Module]]>.Value

            ' We don't seem to be detecting "x ^= y" and "x &= y" as compound operator expressions.
            ' See https://github.com/dotnet/roslyn/issues/21738
            Dim expectedOperationTree = <![CDATA[
IBlockStatement (12 statements, 2 locals) ([Root] OperationKind.BlockStatement) (Syntax: SubBlock, 'Sub Main()' ... End Sub') (Parent: )
  Locals: Local_1: x As System.Int32
    Local_2: y As System.Int32
  IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim x, y As New Integer')
    IVariableDeclaration (2 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'x, y As New Integer')
      Variables: Local_1: x As System.Int32
        Local_2: y As System.Int32
      Initializer: 
        IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: AsNewClause, 'As New Integer')
          IObjectCreationExpression (Constructor: Sub System.Int32..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: System.Int32) (Syntax: ObjectCreationExpression, 'New Integer')
            Arguments(0)
            Initializer: 
              null
  IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: AddAssignmentStatement, 'x += y')
    Expression: 
      ICompoundAssignmentExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: AddAssignmentStatement, 'x += y')
        Left: 
          ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
        Right: 
          ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([2] OperationKind.ExpressionStatement) (Syntax: SubtractAssignmentStatement, 'x -= y')
    Expression: 
      ICompoundAssignmentExpression (BinaryOperatorKind.Subtract, Checked) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: SubtractAssignmentStatement, 'x -= y')
        Left: 
          ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
        Right: 
          ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([3] OperationKind.ExpressionStatement) (Syntax: MultiplyAssignmentStatement, 'x *= y')
    Expression: 
      ICompoundAssignmentExpression (BinaryOperatorKind.Multiply, Checked) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: MultiplyAssignmentStatement, 'x *= y')
        Left: 
          ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
        Right: 
          ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([4] OperationKind.ExpressionStatement) (Syntax: DivideAssignmentStatement, 'x /= y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: DivideAssignmentStatement, 'x /= y')
        Left: 
          ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: DivideAssignmentStatement, 'x /= y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.Divide, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Double, IsImplicit) (Syntax: DivideAssignmentStatement, 'x /= y')
                Left: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Double, IsImplicit) (Syntax: IdentifierName, 'x')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      IOperation:  ([0] OperationKind.None, IsImplicit) (Syntax: IdentifierName, 'x')
                Right: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Double, IsImplicit) (Syntax: IdentifierName, 'y')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([5] OperationKind.ExpressionStatement) (Syntax: IntegerDivideAssignmentStatement, 'x \= y')
    Expression: 
      ICompoundAssignmentExpression (BinaryOperatorKind.IntegerDivide, Checked) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: IntegerDivideAssignmentStatement, 'x \= y')
        Left: 
          ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
        Right: 
          ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([6] OperationKind.ExpressionStatement) (Syntax: ExponentiateAssignmentStatement, 'x ^= y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: ExponentiateAssignmentStatement, 'x ^= y')
        Left: 
          ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: ExponentiateAssignmentStatement, 'x ^= y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.Power, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Double, IsImplicit) (Syntax: ExponentiateAssignmentStatement, 'x ^= y')
                Left: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Double, IsImplicit) (Syntax: IdentifierName, 'x')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      IOperation:  ([0] OperationKind.None, IsImplicit) (Syntax: IdentifierName, 'x')
                Right: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Double, IsImplicit) (Syntax: IdentifierName, 'y')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([7] OperationKind.ExpressionStatement) (Syntax: ConcatenateAssignmentStatement, 'x &= y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: ConcatenateAssignmentStatement, 'x &= y')
        Left: 
          ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: ConcatenateAssignmentStatement, 'x &= y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.Concatenate, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.String, IsImplicit) (Syntax: ConcatenateAssignmentStatement, 'x &= y')
                Left: 
                  IConversionExpression (Explicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.String) (Syntax: IdentifierName, 'x')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      IOperation:  ([0] OperationKind.None, IsImplicit) (Syntax: IdentifierName, 'x')
                Right: 
                  IConversionExpression (Explicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.String) (Syntax: IdentifierName, 'y')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([8] OperationKind.ExpressionStatement) (Syntax: LeftShiftAssignmentStatement, 'x <<= 2')
    Expression: 
      ICompoundAssignmentExpression (BinaryOperatorKind.LeftShift, Checked) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: LeftShiftAssignmentStatement, 'x <<= 2')
        Left: 
          ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
        Right: 
          ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
  IExpressionStatement ([9] OperationKind.ExpressionStatement) (Syntax: RightShiftAssignmentStatement, 'x >>= 3')
    Expression: 
      ICompoundAssignmentExpression (BinaryOperatorKind.RightShift, Checked) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: RightShiftAssignmentStatement, 'x >>= 3')
        Left: 
          ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
        Right: 
          ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3) (Syntax: NumericLiteralExpression, '3')
  ILabeledStatement (Label: exit) ([10] OperationKind.LabeledStatement, IsImplicit) (Syntax: EndSubStatement, 'End Sub')
    Statement: 
      null
  IReturnStatement ([11] OperationKind.ReturnStatement, IsImplicit) (Syntax: EndSubStatement, 'End Sub')
    ReturnedValue: 
      null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of MethodBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TestBinaryCompoundOperators_Unchecked()
            Dim source = <![CDATA[
Module Module1

    Sub Main()'BIND:"Sub Main()"
        Dim x, y As New Integer
        x += y
        x -= y
        x *= y
        x /= y
        x \= y
        x ^= y
        x &= y
        x <<= 2
        x >>= 3
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBlockStatement (12 statements, 2 locals) ([Root] OperationKind.BlockStatement) (Syntax: SubBlock, 'Sub Main()' ... End Sub') (Parent: )
  Locals: Local_1: x As System.Int32
    Local_2: y As System.Int32
  IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim x, y As New Integer')
    IVariableDeclaration (2 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'x, y As New Integer')
      Variables: Local_1: x As System.Int32
        Local_2: y As System.Int32
      Initializer: 
        IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: AsNewClause, 'As New Integer')
          IObjectCreationExpression (Constructor: Sub System.Int32..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: System.Int32) (Syntax: ObjectCreationExpression, 'New Integer')
            Arguments(0)
            Initializer: 
              null
  IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: AddAssignmentStatement, 'x += y')
    Expression: 
      ICompoundAssignmentExpression (BinaryOperatorKind.Add) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: AddAssignmentStatement, 'x += y')
        Left: 
          ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
        Right: 
          ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([2] OperationKind.ExpressionStatement) (Syntax: SubtractAssignmentStatement, 'x -= y')
    Expression: 
      ICompoundAssignmentExpression (BinaryOperatorKind.Subtract) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: SubtractAssignmentStatement, 'x -= y')
        Left: 
          ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
        Right: 
          ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([3] OperationKind.ExpressionStatement) (Syntax: MultiplyAssignmentStatement, 'x *= y')
    Expression: 
      ICompoundAssignmentExpression (BinaryOperatorKind.Multiply) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: MultiplyAssignmentStatement, 'x *= y')
        Left: 
          ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
        Right: 
          ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([4] OperationKind.ExpressionStatement) (Syntax: DivideAssignmentStatement, 'x /= y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: DivideAssignmentStatement, 'x /= y')
        Left: 
          ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: DivideAssignmentStatement, 'x /= y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.Divide) ([0] OperationKind.BinaryOperatorExpression, Type: System.Double, IsImplicit) (Syntax: DivideAssignmentStatement, 'x /= y')
                Left: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Double, IsImplicit) (Syntax: IdentifierName, 'x')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      IOperation:  ([0] OperationKind.None, IsImplicit) (Syntax: IdentifierName, 'x')
                Right: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Double, IsImplicit) (Syntax: IdentifierName, 'y')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([5] OperationKind.ExpressionStatement) (Syntax: IntegerDivideAssignmentStatement, 'x \= y')
    Expression: 
      ICompoundAssignmentExpression (BinaryOperatorKind.IntegerDivide) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: IntegerDivideAssignmentStatement, 'x \= y')
        Left: 
          ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
        Right: 
          ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([6] OperationKind.ExpressionStatement) (Syntax: ExponentiateAssignmentStatement, 'x ^= y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: ExponentiateAssignmentStatement, 'x ^= y')
        Left: 
          ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: ExponentiateAssignmentStatement, 'x ^= y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.Power) ([0] OperationKind.BinaryOperatorExpression, Type: System.Double, IsImplicit) (Syntax: ExponentiateAssignmentStatement, 'x ^= y')
                Left: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Double, IsImplicit) (Syntax: IdentifierName, 'x')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      IOperation:  ([0] OperationKind.None, IsImplicit) (Syntax: IdentifierName, 'x')
                Right: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Double, IsImplicit) (Syntax: IdentifierName, 'y')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([7] OperationKind.ExpressionStatement) (Syntax: ConcatenateAssignmentStatement, 'x &= y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: ConcatenateAssignmentStatement, 'x &= y')
        Left: 
          ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: ConcatenateAssignmentStatement, 'x &= y')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.Concatenate) ([0] OperationKind.BinaryOperatorExpression, Type: System.String, IsImplicit) (Syntax: ConcatenateAssignmentStatement, 'x &= y')
                Left: 
                  IConversionExpression (Explicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.String) (Syntax: IdentifierName, 'x')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      IOperation:  ([0] OperationKind.None, IsImplicit) (Syntax: IdentifierName, 'x')
                Right: 
                  IConversionExpression (Explicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.String) (Syntax: IdentifierName, 'y')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([8] OperationKind.ExpressionStatement) (Syntax: LeftShiftAssignmentStatement, 'x <<= 2')
    Expression: 
      ICompoundAssignmentExpression (BinaryOperatorKind.LeftShift) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: LeftShiftAssignmentStatement, 'x <<= 2')
        Left: 
          ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
        Right: 
          ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
  IExpressionStatement ([9] OperationKind.ExpressionStatement) (Syntax: RightShiftAssignmentStatement, 'x >>= 3')
    Expression: 
      ICompoundAssignmentExpression (BinaryOperatorKind.RightShift) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: RightShiftAssignmentStatement, 'x >>= 3')
        Left: 
          ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
        Right: 
          ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3) (Syntax: NumericLiteralExpression, '3')
  ILabeledStatement (Label: exit) ([10] OperationKind.LabeledStatement, IsImplicit) (Syntax: EndSubStatement, 'End Sub')
    Statement: 
      null
  IReturnStatement ([11] OperationKind.ReturnStatement, IsImplicit) (Syntax: EndSubStatement, 'End Sub')
    ReturnedValue: 
      null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            Dim fileName = "a.vb"
            Dim syntaxTree = Parse(source, fileName)
            Dim references = DefaultVbReferences.Concat({ValueTupleRef, SystemRuntimeFacadeRef})
            Dim compilation = CreateCompilationWithMscorlib45AndVBRuntime({syntaxTree}, references:=references, options:=TestOptions.ReleaseDll.WithOverflowChecks(False))

            VerifyOperationTreeAndDiagnosticsForTest(Of MethodBlockSyntax)(compilation, fileName, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TestUserDefinedBinaryOperators()
            Dim source = <![CDATA[
Public Class B2

    Public Shared Operator +(x As B2, y As B2) As B2
        System.Console.WriteLine("+")
        Return x
    End Operator

    Public Shared Operator -(x As B2, y As B2) As B2
        System.Console.WriteLine("-")
        Return x
    End Operator

    Public Shared Operator *(x As B2, y As B2) As B2
        System.Console.WriteLine("*")
        Return x
    End Operator

    Public Shared Operator /(x As B2, y As B2) As B2
        System.Console.WriteLine("/")
        Return x
    End Operator

    Public Shared Operator \(x As B2, y As B2) As B2
        System.Console.WriteLine("\")
        Return x
    End Operator

    Public Shared Operator Mod(x As B2, y As B2) As B2
        System.Console.WriteLine("Mod")
        Return x
    End Operator

    Public Shared Operator ^(x As B2, y As B2) As B2
        System.Console.WriteLine("^")
        Return x
    End Operator

    Public Shared Operator =(x As B2, y As B2) As B2
        System.Console.WriteLine("=")
        Return x
    End Operator

    Public Shared Operator <>(x As B2, y As B2) As B2
        System.Console.WriteLine("<>")
        Return x
    End Operator

    Public Shared Operator <(x As B2, y As B2) As B2
        System.Console.WriteLine("<")
        Return x
    End Operator

    Public Shared Operator >(x As B2, y As B2) As B2
        System.Console.WriteLine(">")
        Return x
    End Operator

    Public Shared Operator <=(x As B2, y As B2) As B2
        System.Console.WriteLine("<=")
        Return x
    End Operator

    Public Shared Operator >=(x As B2, y As B2) As B2
        System.Console.WriteLine(">=")
        Return x
    End Operator

    Public Shared Operator Like(x As B2, y As B2) As B2
        System.Console.WriteLine("Like")
        Return x
    End Operator

    Public Shared Operator &(x As B2, y As B2) As B2
        System.Console.WriteLine("&")
        Return x
    End Operator

    Public Shared Operator And(x As B2, y As B2) As B2
        System.Console.WriteLine("And")
        Return x
    End Operator

    Public Shared Operator Or(x As B2, y As B2) As B2
        System.Console.WriteLine("Or")
        Return x
    End Operator

    Public Shared Operator Xor(x As B2, y As B2) As B2
        System.Console.WriteLine("Xor")
        Return x
    End Operator

    Public Shared Operator <<(x As B2, y As Integer) As B2
        System.Console.WriteLine("<<")
        Return x
    End Operator

    Public Shared Operator >>(x As B2, y As Integer) As B2
        System.Console.WriteLine(">>")
        Return x
    End Operator
End Class

Module Module1

    Sub Main()'BIND:"Sub Main()"
        Dim x, y As New B2()
        Dim r As B2
        r = x + y
        r = x - y
        r = x * y
        r = x / y
        r = x \ y
        r = x Mod y
        r = x ^ y
        r = x = y
        r = x <> y
        r = x < y
        r = x > y
        r = x <= y
        r = x >= y
        r = x Like y
        r = x & y
        r = x And y
        r = x Or y
        r = x Xor y
        r = x << 2
        r = x >> 3
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBlockStatement (24 statements, 3 locals) ([Root] OperationKind.BlockStatement) (Syntax: SubBlock, 'Sub Main()' ... End Sub') (Parent: )
  Locals: Local_1: x As B2
    Local_2: y As B2
    Local_3: r As B2
  IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim x, y As New B2()')
    IVariableDeclaration (2 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'x, y As New B2()')
      Variables: Local_1: x As B2
        Local_2: y As B2
      Initializer: 
        IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: AsNewClause, 'As New B2()')
          IObjectCreationExpression (Constructor: Sub B2..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: B2) (Syntax: ObjectCreationExpression, 'New B2()')
            Arguments(0)
            Initializer: 
              null
  IVariableDeclarationStatement (1 declarations) ([1] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim r As B2')
    IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'r')
      Variables: Local_1: r As B2
      Initializer: 
        null
  IExpressionStatement ([2] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x + y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: B2) (Syntax: SimpleAssignmentStatement, 'r = x + y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) (OperatorMethod: Function B2.op_Addition(x As B2, y As B2) As B2) ([1] OperationKind.BinaryOperatorExpression, Type: B2) (Syntax: AddExpression, 'x + y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([3] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x - y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: B2) (Syntax: SimpleAssignmentStatement, 'r = x - y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.Subtract, Checked) (OperatorMethod: Function B2.op_Subtraction(x As B2, y As B2) As B2) ([1] OperationKind.BinaryOperatorExpression, Type: B2) (Syntax: SubtractExpression, 'x - y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([4] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x * y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: B2) (Syntax: SimpleAssignmentStatement, 'r = x * y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.Multiply, Checked) (OperatorMethod: Function B2.op_Multiply(x As B2, y As B2) As B2) ([1] OperationKind.BinaryOperatorExpression, Type: B2) (Syntax: MultiplyExpression, 'x * y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([5] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x / y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: B2) (Syntax: SimpleAssignmentStatement, 'r = x / y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.Divide, Checked) (OperatorMethod: Function B2.op_Division(x As B2, y As B2) As B2) ([1] OperationKind.BinaryOperatorExpression, Type: B2) (Syntax: DivideExpression, 'x / y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([6] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x \ y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: B2) (Syntax: SimpleAssignmentStatement, 'r = x \ y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.IntegerDivide, Checked) (OperatorMethod: Function B2.op_IntegerDivision(x As B2, y As B2) As B2) ([1] OperationKind.BinaryOperatorExpression, Type: B2) (Syntax: IntegerDivideExpression, 'x \ y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([7] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x Mod y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: B2) (Syntax: SimpleAssignmentStatement, 'r = x Mod y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.Remainder, Checked) (OperatorMethod: Function B2.op_Modulus(x As B2, y As B2) As B2) ([1] OperationKind.BinaryOperatorExpression, Type: B2) (Syntax: ModuloExpression, 'x Mod y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([8] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x ^ y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: B2) (Syntax: SimpleAssignmentStatement, 'r = x ^ y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.Power, Checked) (OperatorMethod: Function B2.op_Exponent(x As B2, y As B2) As B2) ([1] OperationKind.BinaryOperatorExpression, Type: B2) (Syntax: ExponentiateExpression, 'x ^ y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([9] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x = y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: B2) (Syntax: SimpleAssignmentStatement, 'r = x = y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.Equals, Checked) (OperatorMethod: Function B2.op_Equality(x As B2, y As B2) As B2) ([1] OperationKind.BinaryOperatorExpression, Type: B2) (Syntax: EqualsExpression, 'x = y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([10] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x <> y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: B2) (Syntax: SimpleAssignmentStatement, 'r = x <> y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.NotEquals, Checked) (OperatorMethod: Function B2.op_Inequality(x As B2, y As B2) As B2) ([1] OperationKind.BinaryOperatorExpression, Type: B2) (Syntax: NotEqualsExpression, 'x <> y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([11] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x < y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: B2) (Syntax: SimpleAssignmentStatement, 'r = x < y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.LessThan, Checked) (OperatorMethod: Function B2.op_LessThan(x As B2, y As B2) As B2) ([1] OperationKind.BinaryOperatorExpression, Type: B2) (Syntax: LessThanExpression, 'x < y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([12] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x > y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: B2) (Syntax: SimpleAssignmentStatement, 'r = x > y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) (OperatorMethod: Function B2.op_GreaterThan(x As B2, y As B2) As B2) ([1] OperationKind.BinaryOperatorExpression, Type: B2) (Syntax: GreaterThanExpression, 'x > y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([13] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x <= y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: B2) (Syntax: SimpleAssignmentStatement, 'r = x <= y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.LessThanOrEqual, Checked) (OperatorMethod: Function B2.op_LessThanOrEqual(x As B2, y As B2) As B2) ([1] OperationKind.BinaryOperatorExpression, Type: B2) (Syntax: LessThanOrEqualExpression, 'x <= y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([14] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x >= y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: B2) (Syntax: SimpleAssignmentStatement, 'r = x >= y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.GreaterThanOrEqual, Checked) (OperatorMethod: Function B2.op_GreaterThanOrEqual(x As B2, y As B2) As B2) ([1] OperationKind.BinaryOperatorExpression, Type: B2) (Syntax: GreaterThanOrEqualExpression, 'x >= y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([15] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x Like y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: B2) (Syntax: SimpleAssignmentStatement, 'r = x Like y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.Like, Checked) (OperatorMethod: Function B2.op_Like(x As B2, y As B2) As B2) ([1] OperationKind.BinaryOperatorExpression, Type: B2) (Syntax: LikeExpression, 'x Like y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([16] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x & y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: B2) (Syntax: SimpleAssignmentStatement, 'r = x & y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.Concatenate, Checked) (OperatorMethod: Function B2.op_Concatenate(x As B2, y As B2) As B2) ([1] OperationKind.BinaryOperatorExpression, Type: B2) (Syntax: ConcatenateExpression, 'x & y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([17] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x And y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: B2) (Syntax: SimpleAssignmentStatement, 'r = x And y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.And, Checked) (OperatorMethod: Function B2.op_BitwiseAnd(x As B2, y As B2) As B2) ([1] OperationKind.BinaryOperatorExpression, Type: B2) (Syntax: AndExpression, 'x And y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([18] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x Or y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: B2) (Syntax: SimpleAssignmentStatement, 'r = x Or y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.Or, Checked) (OperatorMethod: Function B2.op_BitwiseOr(x As B2, y As B2) As B2) ([1] OperationKind.BinaryOperatorExpression, Type: B2) (Syntax: OrExpression, 'x Or y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([19] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x Xor y')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: B2) (Syntax: SimpleAssignmentStatement, 'r = x Xor y')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.ExclusiveOr, Checked) (OperatorMethod: Function B2.op_ExclusiveOr(x As B2, y As B2) As B2) ([1] OperationKind.BinaryOperatorExpression, Type: B2) (Syntax: ExclusiveOrExpression, 'x Xor y')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'x')
            Right: 
              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'y')
  IExpressionStatement ([20] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x << 2')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: B2) (Syntax: SimpleAssignmentStatement, 'r = x << 2')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.LeftShift, Checked) (OperatorMethod: Function B2.op_LeftShift(x As B2, y As System.Int32) As B2) ([1] OperationKind.BinaryOperatorExpression, Type: B2) (Syntax: LeftShiftExpression, 'x << 2')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'x')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
  IExpressionStatement ([21] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = x >> 3')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: B2) (Syntax: SimpleAssignmentStatement, 'r = x >> 3')
        Left: 
          ILocalReferenceExpression: r ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.RightShift, Checked) (OperatorMethod: Function B2.op_RightShift(x As B2, y As System.Int32) As B2) ([1] OperationKind.BinaryOperatorExpression, Type: B2) (Syntax: RightShiftExpression, 'x >> 3')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'x')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3) (Syntax: NumericLiteralExpression, '3')
  ILabeledStatement (Label: exit) ([22] OperationKind.LabeledStatement, IsImplicit) (Syntax: EndSubStatement, 'End Sub')
    Statement: 
      null
  IReturnStatement ([23] OperationKind.ReturnStatement, IsImplicit) (Syntax: EndSubStatement, 'End Sub')
    ReturnedValue: 
      null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of MethodBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TestEqualityBinaryOperators()
            Dim source = <![CDATA[
Class C
    Sub M(c1 As C, c2 As C, r As Boolean)'BIND:"Sub M(c1 As C, c2 As C, r As Boolean)"
        r = c1 Is c2
        r = c1 IsNot c2
        r = DirectCast(c1, Object) = c2
        r = DirectCast(c1, Object) <> c2
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBlockStatement (6 statements) ([Root] OperationKind.BlockStatement) (Syntax: SubBlock, 'Sub M(c1 As ... End Sub') (Parent: )
  IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = c1 Is c2')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Boolean) (Syntax: SimpleAssignmentStatement, 'r = c1 Is c2')
        Left: 
          IParameterReferenceExpression: r ([0] OperationKind.ParameterReferenceExpression, Type: System.Boolean) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.Equals) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: IsExpression, 'c1 Is c2')
            Left: 
              IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: IdentifierName, 'c1')
                Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
                Operand: 
                  IParameterReferenceExpression: c1 ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c1')
            Right: 
              IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: IdentifierName, 'c2')
                Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
                Operand: 
                  IParameterReferenceExpression: c2 ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c2')
  IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = c1 IsNot c2')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Boolean) (Syntax: SimpleAssignmentStatement, 'r = c1 IsNot c2')
        Left: 
          IParameterReferenceExpression: r ([0] OperationKind.ParameterReferenceExpression, Type: System.Boolean) (Syntax: IdentifierName, 'r')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.NotEquals) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: IsNotExpression, 'c1 IsNot c2')
            Left: 
              IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: IdentifierName, 'c1')
                Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
                Operand: 
                  IParameterReferenceExpression: c1 ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c1')
            Right: 
              IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: IdentifierName, 'c2')
                Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
                Operand: 
                  IParameterReferenceExpression: c2 ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c2')
  IExpressionStatement ([2] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = DirectC ... bject) = c2')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Boolean) (Syntax: SimpleAssignmentStatement, 'r = DirectC ... bject) = c2')
        Left: 
          IParameterReferenceExpression: r ([0] OperationKind.ParameterReferenceExpression, Type: System.Boolean) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Boolean, IsImplicit) (Syntax: EqualsExpression, 'DirectCast( ... bject) = c2')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.ObjectValueEquals, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Object) (Syntax: EqualsExpression, 'DirectCast( ... bject) = c2')
                Left: 
                  IConversionExpression (Explicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object) (Syntax: DirectCastExpression, 'DirectCast(c1, Object)')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      IParameterReferenceExpression: c1 ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c1')
                Right: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: IdentifierName, 'c2')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      IParameterReferenceExpression: c2 ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c2')
  IExpressionStatement ([3] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'r = DirectC ... ject) <> c2')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Boolean) (Syntax: SimpleAssignmentStatement, 'r = DirectC ... ject) <> c2')
        Left: 
          IParameterReferenceExpression: r ([0] OperationKind.ParameterReferenceExpression, Type: System.Boolean) (Syntax: IdentifierName, 'r')
        Right: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Boolean, IsImplicit) (Syntax: NotEqualsExpression, 'DirectCast( ... ject) <> c2')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.ObjectValueNotEquals, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Object) (Syntax: NotEqualsExpression, 'DirectCast( ... ject) <> c2')
                Left: 
                  IConversionExpression (Explicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object) (Syntax: DirectCastExpression, 'DirectCast(c1, Object)')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      IParameterReferenceExpression: c1 ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c1')
                Right: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: IdentifierName, 'c2')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      IParameterReferenceExpression: c2 ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c2')
  ILabeledStatement (Label: exit) ([4] OperationKind.LabeledStatement, IsImplicit) (Syntax: EndSubStatement, 'End Sub')
    Statement: 
      null
  IReturnStatement ([5] OperationKind.ReturnStatement, IsImplicit) (Syntax: EndSubStatement, 'End Sub')
    ReturnedValue: 
      null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of MethodBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub
    End Class
End Namespace
