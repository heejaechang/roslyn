' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CodeAnalysis.Test.Utilities

Namespace Microsoft.CodeAnalysis.VisualBasic.UnitTests.Semantics

    Partial Public Class IOperationTests
        Inherits SemanticModelTestBase

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub WithStatement_Basic()
            Dim source = <![CDATA[
Class C
    Public I, J As Integer
End Class

Class D
    Private Sub M(c As C)
        With c'BIND:"With c"
            .I = 0
            .J = 0
        End With

    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IWithStatement ([0] OperationKind.None) (Syntax: WithBlock, 'With c'BIND ... End With') (Parent: BlockStatement)
  Value: 
    IParameterReferenceExpression: c ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c')
  Body: 
    IBlockStatement (2 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: WithBlock, 'With c'BIND ... End With')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, '.I = 0')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, '.I = 0')
            Left: 
              IFieldReferenceExpression: C.I As System.Int32 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: SimpleMemberAccessExpression, '.I')
                Instance Receiver: 
                  IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'c')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
      IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, '.J = 0')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, '.J = 0')
            Left: 
              IFieldReferenceExpression: C.J As System.Int32 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: SimpleMemberAccessExpression, '.J')
                Instance Receiver: 
                  IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'c')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of WithBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub WithStatement_Parent()
            Dim source = <![CDATA[
Class C
    Public I, J As Integer
End Class

Class D
    Private Sub M(c As C)'BIND:"Private Sub M(c As C)"
        With c
            .I = 0
            .J = 0
        End With

    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBlockStatement (3 statements) ([Root] OperationKind.BlockStatement) (Syntax: SubBlock, 'Private Sub ... End Sub') (Parent: )
  IWithStatement ([0] OperationKind.None) (Syntax: WithBlock, 'With c ... End With')
    Value: 
      IParameterReferenceExpression: c ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c')
    Body: 
      IBlockStatement (2 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: WithBlock, 'With c ... End With')
        IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, '.I = 0')
          Expression: 
            ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, '.I = 0')
              Left: 
                IFieldReferenceExpression: C.I As System.Int32 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: SimpleMemberAccessExpression, '.I')
                  Instance Receiver: 
                    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'c')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
        IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, '.J = 0')
          Expression: 
            ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, '.J = 0')
              Left: 
                IFieldReferenceExpression: C.J As System.Int32 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: SimpleMemberAccessExpression, '.J')
                  Instance Receiver: 
                    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'c')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  ILabeledStatement (Label: exit) ([1] OperationKind.LabeledStatement, IsImplicit) (Syntax: EndSubStatement, 'End Sub')
    Statement: 
      null
  IReturnStatement ([2] OperationKind.ReturnStatement, IsImplicit) (Syntax: EndSubStatement, 'End Sub')
    ReturnedValue: 
      null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of MethodBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub
    End Class
End Namespace
