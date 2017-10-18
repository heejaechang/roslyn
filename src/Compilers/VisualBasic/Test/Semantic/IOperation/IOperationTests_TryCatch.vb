' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CodeAnalysis.Test.Utilities

Namespace Microsoft.CodeAnalysis.VisualBasic.UnitTests.Semantics

    Partial Public Class IOperationTests
        Inherits SemanticModelTestBase

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatchFinally_Basic()
            Dim source = <![CDATA[
Imports System

Class C
    Private Sub M(i As Integer)
        Try'BIND:"Try"
            i = 0
        Catch ex As Exception When i > 0
            Throw ex
        Finally
            i = 1
        End Try
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITryStatement ([0] OperationKind.TryStatement) (Syntax: TryBlock, 'Try'BIND:"T ... End Try') (Parent: BlockStatement)
  Body: 
    IBlockStatement (1 statements) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: TryBlock, 'Try'BIND:"T ... End Try')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'i = 0')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'i = 0')
            Left: 
              IParameterReferenceExpression: i ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  Catch clauses(1):
    ICatchClause (Exception type: System.Exception) ([1] OperationKind.CatchClause) (Syntax: CatchBlock, 'Catch ex As ... Throw ex')
      Locals: Local_1: ex As System.Exception
      ExceptionDeclarationOrExpression: 
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: IdentifierName, 'ex')
          Variables: Local_1: ex As System.Exception
          Initializer: 
            null
      Filter: 
        IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'i > 0')
          Left: 
            IParameterReferenceExpression: i ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
          Right: 
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
      Handler: 
        IBlockStatement (1 statements) ([2] OperationKind.BlockStatement, IsImplicit) (Syntax: CatchBlock, 'Catch ex As ... Throw ex')
          IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ThrowStatement, 'Throw ex')
            Expression: 
              IThrowExpression ([0] OperationKind.ThrowExpression, Type: System.Exception) (Syntax: ThrowStatement, 'Throw ex')
                ILocalReferenceExpression: ex ([0] OperationKind.LocalReferenceExpression, Type: System.Exception) (Syntax: IdentifierName, 'ex')
  Finally: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: FinallyBlock, 'Finally ... i = 1')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'i = 1')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'i = 1')
            Left: 
              IParameterReferenceExpression: i ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of TryBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatchFinally_Parent()
            Dim source = <![CDATA[
Imports System

Class C
    Private Sub M(i As Integer)'BIND:"Private Sub M(i As Integer)"
        Try
            i = 0
        Catch ex As Exception When i > 0
            Throw ex
        Finally
            i = 1
        End Try
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBlockStatement (3 statements) ([Root] OperationKind.BlockStatement) (Syntax: SubBlock, 'Private Sub ... End Sub') (Parent: )
  ITryStatement ([0] OperationKind.TryStatement) (Syntax: TryBlock, 'Try ... End Try')
    Body: 
      IBlockStatement (1 statements) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: TryBlock, 'Try ... End Try')
        IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'i = 0')
          Expression: 
            ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'i = 0')
              Left: 
                IParameterReferenceExpression: i ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
    Catch clauses(1):
      ICatchClause (Exception type: System.Exception) ([1] OperationKind.CatchClause) (Syntax: CatchBlock, 'Catch ex As ... Throw ex')
        Locals: Local_1: ex As System.Exception
        ExceptionDeclarationOrExpression: 
          IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: IdentifierName, 'ex')
            Variables: Local_1: ex As System.Exception
            Initializer: 
              null
        Filter: 
          IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'i > 0')
            Left: 
              IParameterReferenceExpression: i ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
        Handler: 
          IBlockStatement (1 statements) ([2] OperationKind.BlockStatement, IsImplicit) (Syntax: CatchBlock, 'Catch ex As ... Throw ex')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ThrowStatement, 'Throw ex')
              Expression: 
                IThrowExpression ([0] OperationKind.ThrowExpression, Type: System.Exception) (Syntax: ThrowStatement, 'Throw ex')
                  ILocalReferenceExpression: ex ([0] OperationKind.LocalReferenceExpression, Type: System.Exception) (Syntax: IdentifierName, 'ex')
    Finally: 
      IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: FinallyBlock, 'Finally ... i = 1')
        IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'i = 1')
          Expression: 
            ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'i = 1')
              Left: 
                IParameterReferenceExpression: i ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
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

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatch_SingleCatchClause()
            Dim source = <![CDATA[

Class C
    Private Shared Sub M()
        Try'BIND:"Try"
        Catch e As System.IO.IOException
        End Try
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITryStatement ([0] OperationKind.TryStatement) (Syntax: TryBlock, 'Try'BIND:"T ... End Try') (Parent: BlockStatement)
  Body: 
    IBlockStatement (0 statements) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: TryBlock, 'Try'BIND:"T ... End Try')
  Catch clauses(1):
    ICatchClause (Exception type: System.IO.IOException) ([1] OperationKind.CatchClause) (Syntax: CatchBlock, 'Catch e As  ... IOException')
      Locals: Local_1: e As System.IO.IOException
      ExceptionDeclarationOrExpression: 
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: IdentifierName, 'e')
          Variables: Local_1: e As System.IO.IOException
          Initializer: 
            null
      Filter: 
        null
      Handler: 
        IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: CatchBlock, 'Catch e As  ... IOException')
  Finally: 
    null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of TryBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatch_SingleCatchClauseAndFilter()
            Dim source = <![CDATA[

Class C
    Private Shared Sub M()
        Try'BIND:"Try"
        Catch e As System.IO.IOException When e.Message IsNot Nothing
        End Try
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITryStatement ([0] OperationKind.TryStatement) (Syntax: TryBlock, 'Try'BIND:"T ... End Try') (Parent: BlockStatement)
  Body: 
    IBlockStatement (0 statements) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: TryBlock, 'Try'BIND:"T ... End Try')
  Catch clauses(1):
    ICatchClause (Exception type: System.IO.IOException) ([1] OperationKind.CatchClause) (Syntax: CatchBlock, 'Catch e As  ... Not Nothing')
      Locals: Local_1: e As System.IO.IOException
      ExceptionDeclarationOrExpression: 
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: IdentifierName, 'e')
          Variables: Local_1: e As System.IO.IOException
          Initializer: 
            null
      Filter: 
        IBinaryOperatorExpression (BinaryOperatorKind.NotEquals) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: IsNotExpression, 'e.Message IsNot Nothing')
          Left: 
            IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: SimpleMemberAccessExpression, 'e.Message')
              Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
              Operand: 
                IPropertyReferenceExpression: ReadOnly Property System.Exception.Message As System.String ([0] OperationKind.PropertyReferenceExpression, Type: System.String) (Syntax: SimpleMemberAccessExpression, 'e.Message')
                  Instance Receiver: 
                    ILocalReferenceExpression: e ([0] OperationKind.LocalReferenceExpression, Type: System.IO.IOException) (Syntax: IdentifierName, 'e')
          Right: 
            IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Object, Constant: null, IsImplicit) (Syntax: NothingLiteralExpression, 'Nothing')
              Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              Operand: 
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NothingLiteralExpression, 'Nothing')
      Handler: 
        IBlockStatement (0 statements) ([2] OperationKind.BlockStatement, IsImplicit) (Syntax: CatchBlock, 'Catch e As  ... Not Nothing')
  Finally: 
    null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of TryBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatch_MultipleCatchClausesWithDifferentCaughtTypes()
            Dim source = <![CDATA[

Class C
    Private Shared Sub M()
        Try'BIND:"Try"
        Catch e As System.IO.IOException
        Catch e As System.Exception When e.Message IsNot Nothing
        End Try
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITryStatement ([0] OperationKind.TryStatement) (Syntax: TryBlock, 'Try'BIND:"T ... End Try') (Parent: BlockStatement)
  Body: 
    IBlockStatement (0 statements) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: TryBlock, 'Try'BIND:"T ... End Try')
  Catch clauses(2):
    ICatchClause (Exception type: System.IO.IOException) ([1] OperationKind.CatchClause) (Syntax: CatchBlock, 'Catch e As  ... IOException')
      Locals: Local_1: e As System.IO.IOException
      ExceptionDeclarationOrExpression: 
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: IdentifierName, 'e')
          Variables: Local_1: e As System.IO.IOException
          Initializer: 
            null
      Filter: 
        null
      Handler: 
        IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: CatchBlock, 'Catch e As  ... IOException')
    ICatchClause (Exception type: System.Exception) ([2] OperationKind.CatchClause) (Syntax: CatchBlock, 'Catch e As  ... Not Nothing')
      Locals: Local_1: e As System.Exception
      ExceptionDeclarationOrExpression: 
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: IdentifierName, 'e')
          Variables: Local_1: e As System.Exception
          Initializer: 
            null
      Filter: 
        IBinaryOperatorExpression (BinaryOperatorKind.NotEquals) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: IsNotExpression, 'e.Message IsNot Nothing')
          Left: 
            IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: SimpleMemberAccessExpression, 'e.Message')
              Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
              Operand: 
                IPropertyReferenceExpression: ReadOnly Property System.Exception.Message As System.String ([0] OperationKind.PropertyReferenceExpression, Type: System.String) (Syntax: SimpleMemberAccessExpression, 'e.Message')
                  Instance Receiver: 
                    ILocalReferenceExpression: e ([0] OperationKind.LocalReferenceExpression, Type: System.Exception) (Syntax: IdentifierName, 'e')
          Right: 
            IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Object, Constant: null, IsImplicit) (Syntax: NothingLiteralExpression, 'Nothing')
              Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              Operand: 
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NothingLiteralExpression, 'Nothing')
      Handler: 
        IBlockStatement (0 statements) ([2] OperationKind.BlockStatement, IsImplicit) (Syntax: CatchBlock, 'Catch e As  ... Not Nothing')
  Finally: 
    null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of TryBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatch_MultipleCatchClausesWithDuplicateCaughtTypes()
            Dim source = <![CDATA[

Class C
    Private Shared Sub M()
        Try'BIND:"Try"
        Catch e As System.IO.IOException
        Catch e As System.IO.IOException When e.Message IsNot Nothing
        End Try
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITryStatement ([0] OperationKind.TryStatement) (Syntax: TryBlock, 'Try'BIND:"T ... End Try') (Parent: BlockStatement)
  Body: 
    IBlockStatement (0 statements) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: TryBlock, 'Try'BIND:"T ... End Try')
  Catch clauses(2):
    ICatchClause (Exception type: System.IO.IOException) ([1] OperationKind.CatchClause) (Syntax: CatchBlock, 'Catch e As  ... IOException')
      Locals: Local_1: e As System.IO.IOException
      ExceptionDeclarationOrExpression: 
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: IdentifierName, 'e')
          Variables: Local_1: e As System.IO.IOException
          Initializer: 
            null
      Filter: 
        null
      Handler: 
        IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: CatchBlock, 'Catch e As  ... IOException')
    ICatchClause (Exception type: System.IO.IOException) ([2] OperationKind.CatchClause) (Syntax: CatchBlock, 'Catch e As  ... Not Nothing')
      Locals: Local_1: e As System.IO.IOException
      ExceptionDeclarationOrExpression: 
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: IdentifierName, 'e')
          Variables: Local_1: e As System.IO.IOException
          Initializer: 
            null
      Filter: 
        IBinaryOperatorExpression (BinaryOperatorKind.NotEquals) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: IsNotExpression, 'e.Message IsNot Nothing')
          Left: 
            IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: SimpleMemberAccessExpression, 'e.Message')
              Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
              Operand: 
                IPropertyReferenceExpression: ReadOnly Property System.Exception.Message As System.String ([0] OperationKind.PropertyReferenceExpression, Type: System.String) (Syntax: SimpleMemberAccessExpression, 'e.Message')
                  Instance Receiver: 
                    ILocalReferenceExpression: e ([0] OperationKind.LocalReferenceExpression, Type: System.IO.IOException) (Syntax: IdentifierName, 'e')
          Right: 
            IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Object, Constant: null, IsImplicit) (Syntax: NothingLiteralExpression, 'Nothing')
              Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              Operand: 
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NothingLiteralExpression, 'Nothing')
      Handler: 
        IBlockStatement (0 statements) ([2] OperationKind.BlockStatement, IsImplicit) (Syntax: CatchBlock, 'Catch e As  ... Not Nothing')
  Finally: 
    null
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC42031: 'Catch' block never reached; 'IOException' handled above in the same Try statement.
        Catch e As System.IO.IOException When e.Message IsNot Nothing
        ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of TryBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatch_CatchClauseWithTypeExpression()
            Dim source = <![CDATA[

Class C
    Private Shared Sub M()
        Try'BIND:"Try"
        Catch System.Exception
        End Try
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITryStatement ([0] OperationKind.TryStatement, IsInvalid) (Syntax: TryBlock, 'Try'BIND:"T ... End Try') (Parent: BlockStatement)
  Body: 
    IBlockStatement (0 statements) ([0] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: TryBlock, 'Try'BIND:"T ... End Try')
  Catch clauses(1):
    ICatchClause (Exception type: ?) ([1] OperationKind.CatchClause, IsInvalid) (Syntax: CatchBlock, 'Catch System')
      ExceptionDeclarationOrExpression: 
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'System')
          Children(1):
            IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: IdentifierName, 'System')
      Filter: 
        null
      Handler: 
        IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: CatchBlock, 'Catch System')
  Finally: 
    null
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC31082: 'System' is not a local variable or parameter, and so cannot be used as a 'Catch' variable.
        Catch System.Exception
              ~~~~~~
BC30205: End of statement expected.
        Catch System.Exception
                    ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of TryBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatch_CatchClauseWithLocalReferenceExpression()
            Dim source = <![CDATA[
Imports System
Class C
    Private Shared Sub M()
        Dim e As IO.IOException = Nothing
        Try'BIND:"Try"
        Catch e
        End Try
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITryStatement ([1] OperationKind.TryStatement) (Syntax: TryBlock, 'Try'BIND:"T ... End Try') (Parent: BlockStatement)
  Body: 
    IBlockStatement (0 statements) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: TryBlock, 'Try'BIND:"T ... End Try')
  Catch clauses(1):
    ICatchClause (Exception type: System.IO.IOException) ([1] OperationKind.CatchClause) (Syntax: CatchBlock, 'Catch e')
      ExceptionDeclarationOrExpression: 
        ILocalReferenceExpression: e ([0] OperationKind.LocalReferenceExpression, Type: System.IO.IOException) (Syntax: IdentifierName, 'e')
      Filter: 
        null
      Handler: 
        IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: CatchBlock, 'Catch e')
  Finally: 
    null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of TryBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatch_CatchClauseWithParameterReferenceExpression()
            Dim source = <![CDATA[
Imports System
Class C
    Private Shared Sub M(e As IO.IOException)
        Try'BIND:"Try"
        Catch e
        End Try
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITryStatement ([0] OperationKind.TryStatement) (Syntax: TryBlock, 'Try'BIND:"T ... End Try') (Parent: BlockStatement)
  Body: 
    IBlockStatement (0 statements) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: TryBlock, 'Try'BIND:"T ... End Try')
  Catch clauses(1):
    ICatchClause (Exception type: System.IO.IOException) ([1] OperationKind.CatchClause) (Syntax: CatchBlock, 'Catch e')
      ExceptionDeclarationOrExpression: 
        IParameterReferenceExpression: e ([0] OperationKind.ParameterReferenceExpression, Type: System.IO.IOException) (Syntax: IdentifierName, 'e')
      Filter: 
        null
      Handler: 
        IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: CatchBlock, 'Catch e')
  Finally: 
    null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of TryBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatch_CatchClauseWithFieldReferenceExpression()
            Dim source = <![CDATA[
Imports System
Class C
    Private e As IO.IOException = Nothing

    Private Sub M()
        Try 'BIND:"Try"'BIND:"Try 'BIND:"Try""
        Catch e
        End Try
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITryStatement ([0] OperationKind.TryStatement, IsInvalid) (Syntax: TryBlock, 'Try 'BIND:" ... End Try') (Parent: BlockStatement)
  Body: 
    IBlockStatement (0 statements) ([0] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: TryBlock, 'Try 'BIND:" ... End Try')
  Catch clauses(1):
    ICatchClause (Exception type: System.IO.IOException) ([1] OperationKind.CatchClause, IsInvalid) (Syntax: CatchBlock, 'Catch e')
      ExceptionDeclarationOrExpression: 
        IFieldReferenceExpression: C.e As System.IO.IOException ([0] OperationKind.FieldReferenceExpression, Type: System.IO.IOException, IsInvalid) (Syntax: IdentifierName, 'e')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'e')
      Filter: 
        null
      Handler: 
        IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: CatchBlock, 'Catch e')
  Finally: 
    null
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC31082: 'e' is not a local variable or parameter, and so cannot be used as a 'Catch' variable.
        Catch e
              ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of TryBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatch_CatchClauseWithErrorExpression()
            Dim source = <![CDATA[
Imports System
Class C
    Private Shared Sub M()
        Try'BIND:"Try"
        Catch e
        End Try
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITryStatement ([0] OperationKind.TryStatement, IsInvalid) (Syntax: TryBlock, 'Try'BIND:"T ... End Try') (Parent: BlockStatement)
  Body: 
    IBlockStatement (0 statements) ([0] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: TryBlock, 'Try'BIND:"T ... End Try')
  Catch clauses(1):
    ICatchClause (Exception type: ?) ([1] OperationKind.CatchClause, IsInvalid) (Syntax: CatchBlock, 'Catch e')
      ExceptionDeclarationOrExpression: 
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: IdentifierName, 'e')
          Children(0)
      Filter: 
        null
      Handler: 
        IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: CatchBlock, 'Catch e')
  Finally: 
    null
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30451: 'e' is not declared. It may be inaccessible due to its protection level.
        Catch e
              ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of TryBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatch_CatchClauseWithInvalidExpression()
            Dim source = <![CDATA[
Imports System
Class C
    Private Shared Sub M()
        Try'BIND:"Try"
        Catch M2(e)
        End Try
    End Sub

    Private Shared Function M2(e As Exception) As Exception
        Return e
    End Function
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITryStatement ([0] OperationKind.TryStatement, IsInvalid) (Syntax: TryBlock, 'Try'BIND:"T ... End Try') (Parent: BlockStatement)
  Body: 
    IBlockStatement (0 statements) ([0] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: TryBlock, 'Try'BIND:"T ... End Try')
  Catch clauses(1):
    ICatchClause (Exception type: ?) ([1] OperationKind.CatchClause, IsInvalid) (Syntax: CatchBlock, 'Catch M2')
      ExceptionDeclarationOrExpression: 
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M2')
          Children(1):
            IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: IdentifierName, 'M2')
              Children(1):
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M2')
      Filter: 
        null
      Handler: 
        IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: CatchBlock, 'Catch M2')
  Finally: 
    null
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC31082: 'M2' is not a local variable or parameter, and so cannot be used as a 'Catch' variable.
        Catch M2(e)
              ~~
BC30205: End of statement expected.
        Catch M2(e)
                ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of TryBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatch_CatchClauseWithoutCaughtTypeOrExceptionLocal()
            Dim source = <![CDATA[

Class C
    Private Shared Sub M()
        Try'BIND:"Try"
        Catch
        End Try
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITryStatement ([0] OperationKind.TryStatement) (Syntax: TryBlock, 'Try'BIND:"T ... End Try') (Parent: BlockStatement)
  Body: 
    IBlockStatement (0 statements) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: TryBlock, 'Try'BIND:"T ... End Try')
  Catch clauses(1):
    ICatchClause (Exception type: System.Exception) ([1] OperationKind.CatchClause) (Syntax: CatchBlock, 'Catch')
      ExceptionDeclarationOrExpression: 
        null
      Filter: 
        null
      Handler: 
        IBlockStatement (0 statements) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: CatchBlock, 'Catch')
  Finally: 
    null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of TryBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatch_FinallyWithoutCatchClause()
            Dim source = <![CDATA[
Imports System
Class C
    Private Shared Sub M(s As String)
        Try'BIND:"Try"
        Finally
            Console.WriteLine(s)
        End Try
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITryStatement ([0] OperationKind.TryStatement) (Syntax: TryBlock, 'Try'BIND:"T ... End Try') (Parent: BlockStatement)
  Body: 
    IBlockStatement (0 statements) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: TryBlock, 'Try'BIND:"T ... End Try')
  Catch clauses(0)
  Finally: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: FinallyBlock, 'Finally ... riteLine(s)')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.WriteLine(s)')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.WriteLine(s)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 's')
                IParameterReferenceExpression: s ([0] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 's')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of TryBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatch_TryBlockWithLocalDeclaration()
            Dim source = <![CDATA[
Imports System
Class C
    Private Shared Sub M(s As String)
        Try'BIND:"Try"
            Dim i As Integer = 0
        Finally
        End Try
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITryStatement ([0] OperationKind.TryStatement) (Syntax: TryBlock, 'Try'BIND:"T ... End Try') (Parent: BlockStatement)
  Body: 
    IBlockStatement (1 statements, 1 locals) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: TryBlock, 'Try'BIND:"T ... End Try')
      Locals: Local_1: i As System.Int32
      IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim i As Integer = 0')
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i')
          Variables: Local_1: i As System.Int32
          Initializer: 
            IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= 0')
              ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  Catch clauses(0)
  Finally: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement) (Syntax: FinallyBlock, 'Finally')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of TryBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatch_CatchBlockWithLocalDeclaration()
            Dim source = <![CDATA[
Imports System
Class C
    Private Shared Sub M(s As String)
        Try'BIND:"Try"
        Catch ex As Exception
            Dim i As Integer = 0
        End Try
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITryStatement ([0] OperationKind.TryStatement) (Syntax: TryBlock, 'Try'BIND:"T ... End Try') (Parent: BlockStatement)
  Body: 
    IBlockStatement (0 statements) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: TryBlock, 'Try'BIND:"T ... End Try')
  Catch clauses(1):
    ICatchClause (Exception type: System.Exception) ([1] OperationKind.CatchClause) (Syntax: CatchBlock, 'Catch ex As ... Integer = 0')
      Locals: Local_1: ex As System.Exception
      ExceptionDeclarationOrExpression: 
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: IdentifierName, 'ex')
          Variables: Local_1: ex As System.Exception
          Initializer: 
            null
      Filter: 
        null
      Handler: 
        IBlockStatement (1 statements, 1 locals) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: CatchBlock, 'Catch ex As ... Integer = 0')
          Locals: Local_1: i As System.Int32
          IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim i As Integer = 0')
            IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i')
              Variables: Local_1: i As System.Int32
              Initializer: 
                IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= 0')
                  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  Finally: 
    null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of TryBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatch_FinallyWithLocalDeclaration()
            Dim source = <![CDATA[
Imports System
Class C
    Private Shared Sub M(s As String)
        Try'BIND:"Try"
        Finally
            Dim i As Integer = 0
        End Try
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITryStatement ([0] OperationKind.TryStatement) (Syntax: TryBlock, 'Try'BIND:"T ... End Try') (Parent: BlockStatement)
  Body: 
    IBlockStatement (0 statements) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: TryBlock, 'Try'BIND:"T ... End Try')
  Catch clauses(0)
  Finally: 
    IBlockStatement (1 statements, 1 locals) ([1] OperationKind.BlockStatement) (Syntax: FinallyBlock, 'Finally ... Integer = 0')
      Locals: Local_1: i As System.Int32
      IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim i As Integer = 0')
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i')
          Variables: Local_1: i As System.Int32
          Initializer: 
            IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= 0')
              ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of TryBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatch_InvalidCaughtType()
            Dim source = <![CDATA[
Imports System
Class C
    Private Shared Sub M(s As String)
        Try'BIND:"Try"
        Catch i As Integer
        End Try
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITryStatement ([0] OperationKind.TryStatement, IsInvalid) (Syntax: TryBlock, 'Try'BIND:"T ... End Try') (Parent: BlockStatement)
  Body: 
    IBlockStatement (0 statements) ([0] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: TryBlock, 'Try'BIND:"T ... End Try')
  Catch clauses(1):
    ICatchClause (Exception type: System.Int32) ([1] OperationKind.CatchClause, IsInvalid) (Syntax: CatchBlock, 'Catch i As Integer')
      Locals: Local_1: i As System.Int32
      ExceptionDeclarationOrExpression: 
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: IdentifierName, 'i')
          Variables: Local_1: i As System.Int32
          Initializer: 
            null
      Filter: 
        null
      Handler: 
        IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: CatchBlock, 'Catch i As Integer')
  Finally: 
    null
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30392: 'Catch' cannot catch type 'Integer' because it is not 'System.Exception' or a class that inherits from 'System.Exception'.
        Catch i As Integer
                   ~~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of TryBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatch_GetOperationForCatchBlock()
            Dim source = <![CDATA[
Imports System
Class C
    Private Shared Sub M()
        Try
        Catch e As IO.IOException When e.Message IsNot Nothing'BIND:"Catch e As IO.IOException When e.Message IsNot Nothing"
        End Try
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ICatchClause (Exception type: System.IO.IOException) ([1] OperationKind.CatchClause) (Syntax: CatchBlock, 'Catch e As  ... Not Nothing') (Parent: TryStatement)
  Locals: Local_1: e As System.IO.IOException
  ExceptionDeclarationOrExpression: 
    IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: IdentifierName, 'e')
      Variables: Local_1: e As System.IO.IOException
      Initializer: 
        null
  Filter: 
    IBinaryOperatorExpression (BinaryOperatorKind.NotEquals) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: IsNotExpression, 'e.Message IsNot Nothing')
      Left: 
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: SimpleMemberAccessExpression, 'e.Message')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            IPropertyReferenceExpression: ReadOnly Property System.Exception.Message As System.String ([0] OperationKind.PropertyReferenceExpression, Type: System.String) (Syntax: SimpleMemberAccessExpression, 'e.Message')
              Instance Receiver: 
                ILocalReferenceExpression: e ([0] OperationKind.LocalReferenceExpression, Type: System.IO.IOException) (Syntax: IdentifierName, 'e')
      Right: 
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Object, Constant: null, IsImplicit) (Syntax: NothingLiteralExpression, 'Nothing')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NothingLiteralExpression, 'Nothing')
  Handler: 
    IBlockStatement (0 statements) ([2] OperationKind.BlockStatement, IsImplicit) (Syntax: CatchBlock, 'Catch e As  ... Not Nothing')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of CatchBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatch_GetOperationForFinallyBlock()
            Dim source = <![CDATA[
Imports System
Class C
    Private Shared Sub M(s As String)
        Try
        Finally'BIND:"Finally"
            Console.WriteLine(s)
        End Try
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: FinallyBlock, 'Finally'BIN ... riteLine(s)') (Parent: TryStatement)
  IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.WriteLine(s)')
    Expression: 
      IInvocationExpression (Sub System.Console.WriteLine(value As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.WriteLine(s)')
        Instance Receiver: 
          null
        Arguments(1):
          IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 's')
            IParameterReferenceExpression: s ([0] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 's')
            InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of FinallyBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatch_GetOperationForCatchExceptionIdentifier()
            Dim source = <![CDATA[
Imports System
Class C
    Private Sub M(e As Exception)
        Try
        Catch e'BIND:"e"
        End Try
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IParameterReferenceExpression: e ([0] OperationKind.ParameterReferenceExpression, Type: System.Exception) (Syntax: IdentifierName, 'e') (Parent: CatchClause)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of IdentifierNameSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(Skip:="https://github.com/dotnet/roslyn/issues/22299")>
        Public Sub TryCatch_GetOperationForCatchExceptionDeclaration()
            Dim source = <![CDATA[
Imports System
Class C
    Private Sub M()
        Try
        Catch e As Exception'BIND:"e"
        End Try
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclaration (1 variables) (OperationKind.VariableDeclaration) (Syntax: 'e')
  Variables: Local_1: e As System.Exception
  Initializer: null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of IdentifierNameSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatch_GetOperationForCatchFilterClause()
            Dim source = <![CDATA[
Imports System
Class C
    Private Shared Sub M()
        Try
        Catch e As IO.IOException When e.Message IsNot Nothing'BIND:"When e.Message IsNot Nothing"
        End Try
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
]]>.Value

            ' GetOperation return Nothing for CatchFilterClauseSyntax
            Assert.Null(GetOperationTreeForTest(Of CatchFilterClauseSyntax)(source).operation)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatch_GetOperationForCatchFilterClauseExpression()
            Dim source = <![CDATA[
Imports System
Class C
    Private Shared Sub M()
        Try
        Catch e As IO.IOException When e.Message IsNot Nothing'BIND:"e.Message IsNot Nothing"
        End Try
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBinaryOperatorExpression (BinaryOperatorKind.NotEquals) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: IsNotExpression, 'e.Message IsNot Nothing') (Parent: CatchClause)
  Left: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: SimpleMemberAccessExpression, 'e.Message')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IPropertyReferenceExpression: ReadOnly Property System.Exception.Message As System.String ([0] OperationKind.PropertyReferenceExpression, Type: System.String) (Syntax: SimpleMemberAccessExpression, 'e.Message')
          Instance Receiver: 
            ILocalReferenceExpression: e ([0] OperationKind.LocalReferenceExpression, Type: System.IO.IOException) (Syntax: IdentifierName, 'e')
  Right: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Object, Constant: null, IsImplicit) (Syntax: NothingLiteralExpression, 'Nothing')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NothingLiteralExpression, 'Nothing')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of BinaryExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatch_GetOperationForCatchStatement()
            Dim source = <![CDATA[
Imports System
Class C
    Private Shared Sub M()
        Try
        Catch e As IO.IOException When e.Message IsNot Nothing'BIND:"Catch e As IO.IOException When e.Message IsNot Nothing"
        End Try
    End Sub
End Class]]>.Value

            ' GetOperation returns Nothing for CatchStatementSyntax
            Assert.Null(GetOperationTreeForTest(Of CatchStatementSyntax)(source).operation)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatch_GetOperationForTryStatement()
            Dim source = <![CDATA[
Imports System
Class C
    Private Shared Sub M()
        Try'BIND:"Try"
        Catch e As IO.IOException When e.Message IsNot Nothing
        End Try
    End Sub
End Class]]>.Value

            ' GetOperation returns Nothing for TryStatementSyntax
            Assert.Null(GetOperationTreeForTest(Of TryStatementSyntax)(source).operation)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatch_GetOperationForEndTryStatement()
            Dim source = <![CDATA[
Imports System
Class C
    Private Shared Sub M()
        Try
        Catch e As IO.IOException When e.Message IsNot Nothing
        End Try'BIND:"End Try"
    End Sub
End Class]]>.Value

            ' GetOperation returns Nothing for End Try statement
            Assert.Null(GetOperationTreeForTest(Of EndBlockStatementSyntax)(source).operation)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatch_GetOperationForFinallyStatement()
            Dim source = <![CDATA[
Imports System
Class C
    Private Shared Sub M(s As String)
        Try
        Finally'BIND:"Finally"
            Console.WriteLine(s)
        End Try
    End Sub
End Class]]>.Value

            ' GetOperation returns Nothing for FinallyStatementSyntax
            Assert.Null(GetOperationTreeForTest(Of FinallyStatementSyntax)(source).operation)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatch_GetOperationForStatementInTryBlock()
            Dim source = <![CDATA[
Imports System
Class C
    Private Shared Sub M(s As String)
        Try
            Console.WriteLine(s)'BIND:"Console.WriteLine(s)"
        Catch e As IO.IOException
        End Try
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.WriteLine(s)') (Parent: BlockStatement)
  Expression: 
    IInvocationExpression (Sub System.Console.WriteLine(value As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.WriteLine(s)')
      Instance Receiver: 
        null
      Arguments(1):
        IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 's')
          IParameterReferenceExpression: s ([0] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 's')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ExpressionStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatch_GetOperationForStatementInCatchBlock()
            Dim source = <![CDATA[
Imports System
Class C
    Private Shared Sub M()
        Try
        Catch e As IO.IOException
            Console.WriteLine(e)'BIND:"Console.WriteLine(e)"
        End Try
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.WriteLine(e)') (Parent: BlockStatement)
  Expression: 
    IInvocationExpression (Sub System.Console.WriteLine(value As System.Object)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.WriteLine(e)')
      Instance Receiver: 
        null
      Arguments(1):
        IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'e')
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: IdentifierName, 'e')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              ILocalReferenceExpression: e ([0] OperationKind.LocalReferenceExpression, Type: System.IO.IOException) (Syntax: IdentifierName, 'e')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ExpressionStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TryCatch_GetOperationForStatementInFinallyBlock()
            Dim source = <![CDATA[
Imports System
Class C
    Private Shared Sub M(s As String)
        Try
        Finally
            Console.WriteLine(s)'BIND:"Console.WriteLine(s)"
        End Try
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.WriteLine(s)') (Parent: BlockStatement)
  Expression: 
    IInvocationExpression (Sub System.Console.WriteLine(value As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.WriteLine(s)')
      Instance Receiver: 
        null
      Arguments(1):
        IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 's')
          IParameterReferenceExpression: s ([0] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 's')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ExpressionStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

    End Class
End Namespace
