' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.Test.Utilities
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax

Namespace Microsoft.CodeAnalysis.VisualBasic.UnitTests.Semantics
    Partial Public Class IOperationTests
        Inherits SemanticModelTestBase

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub ILockStatement_ObjectLock_FieldReference()
            Dim source = <![CDATA[
Option Strict On

Public Class C1
    Dim o As New Object
    Public Sub M1()
        SyncLock o'BIND:"SyncLock o"

        End SyncLock
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ILockStatement ([0] OperationKind.LockStatement) (Syntax: SyncLockBlock, 'SyncLock o' ... nd SyncLock') (Parent: BlockStatement)
  Expression: 
    IFieldReferenceExpression: C1.o As System.Object ([0] OperationKind.FieldReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'o')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C1, IsImplicit) (Syntax: IdentifierName, 'o')
  Body: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: SyncLockBlock, 'SyncLock o' ... nd SyncLock')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of SyncLockBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub ILockStatement_ObjectLock_LocalReference()
            Dim source = <![CDATA[
Option Strict On

Public Class C1
    Public Sub M1()
        Dim o As New Object
        SyncLock o'BIND:"SyncLock o"

        End SyncLock
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ILockStatement ([1] OperationKind.LockStatement) (Syntax: SyncLockBlock, 'SyncLock o' ... nd SyncLock') (Parent: BlockStatement)
  Expression: 
    ILocalReferenceExpression: o ([0] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'o')
  Body: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: SyncLockBlock, 'SyncLock o' ... nd SyncLock')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of SyncLockBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub ILockStatement_ObjectLock_Nothing()
            Dim source = <![CDATA[
Option Strict On

Public Class C1
    Public Sub M1()
        Dim o As New Object
        SyncLock o'BIND:"SyncLock o"

        End SyncLock
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ILockStatement ([1] OperationKind.LockStatement) (Syntax: SyncLockBlock, 'SyncLock o' ... nd SyncLock') (Parent: BlockStatement)
  Expression: 
    ILocalReferenceExpression: o ([0] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'o')
  Body: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: SyncLockBlock, 'SyncLock o' ... nd SyncLock')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of SyncLockBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub ILockStatement_ObjectLock_NonReferenceType()
            Dim source = <![CDATA[
Option Strict On

Public Class C1
    Public Sub M1()
        Dim i As Integer = 1
        SyncLock i'BIND:"SyncLock i"

        End SyncLock
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ILockStatement ([1] OperationKind.LockStatement, IsInvalid) (Syntax: SyncLockBlock, 'SyncLock i' ... nd SyncLock') (Parent: BlockStatement)
  Expression: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32, IsInvalid) (Syntax: IdentifierName, 'i')
  Body: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: SyncLockBlock, 'SyncLock i' ... nd SyncLock')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30582: 'SyncLock' operand cannot be of type 'Integer' because 'Integer' is not a reference type.
        SyncLock i'BIND:"SyncLock i"
                 ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of SyncLockBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub ILockStatement_MissingLockExpression()
            Dim source = <![CDATA[
Option Strict On

Public Class C1
    Public Sub M1()
        SyncLock'BIND:"SyncLock"

        End SyncLock
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ILockStatement ([0] OperationKind.LockStatement, IsInvalid) (Syntax: SyncLockBlock, 'SyncLock'BI ... nd SyncLock') (Parent: BlockStatement)
  Expression: 
    IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
      Children(0)
  Body: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: SyncLockBlock, 'SyncLock'BI ... nd SyncLock')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30201: Expression expected.
        SyncLock'BIND:"SyncLock"
                ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of SyncLockBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub ILockStatement_InvalidLockExpression()
            Dim source = <![CDATA[
Option Strict On

Public Class C1
    Public Sub M1()
        SyncLock InvalidReference'BIND:"SyncLock InvalidReference"

        End SyncLock
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ILockStatement ([0] OperationKind.LockStatement, IsInvalid) (Syntax: SyncLockBlock, 'SyncLock In ... nd SyncLock') (Parent: BlockStatement)
  Expression: 
    IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: IdentifierName, 'InvalidReference')
      Children(0)
  Body: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: SyncLockBlock, 'SyncLock In ... nd SyncLock')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30451: 'InvalidReference' is not declared. It may be inaccessible due to its protection level.
        SyncLock InvalidReference'BIND:"SyncLock InvalidReference"
                 ~~~~~~~~~~~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of SyncLockBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub ILockStatement_MissingEndLock()
            Dim source = <![CDATA[
Option Strict On

Public Class C1
    Public Sub M1()
        Dim o As New Object
        SyncLock o'BIND:"SyncLock o"

    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ILockStatement ([1] OperationKind.LockStatement, IsInvalid) (Syntax: SyncLockBlock, 'SyncLock o' ... SyncLock o"') (Parent: BlockStatement)
  Expression: 
    ILocalReferenceExpression: o ([0] OperationKind.LocalReferenceExpression, Type: System.Object, IsInvalid) (Syntax: IdentifierName, 'o')
  Body: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: SyncLockBlock, 'SyncLock o' ... SyncLock o"')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30675: 'SyncLock' statement must end with a matching 'End SyncLock'.
        SyncLock o'BIND:"SyncLock o"
        ~~~~~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of SyncLockBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub ILockStatement_ExpressionBody_ObjectCall()
            Dim source = <![CDATA[
Option Strict On

Public Class C1
    Public Sub M1()
        Dim o As New Object
        SyncLock o.ToString()'BIND:"SyncLock o.ToString()"
        End SyncLock
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ILockStatement ([1] OperationKind.LockStatement) (Syntax: SyncLockBlock, 'SyncLock o. ... nd SyncLock') (Parent: BlockStatement)
  Expression: 
    IInvocationExpression (virtual Function System.Object.ToString() As System.String) ([0] OperationKind.InvocationExpression, Type: System.String) (Syntax: InvocationExpression, 'o.ToString()')
      Instance Receiver: 
        ILocalReferenceExpression: o ([0] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'o')
      Arguments(0)
  Body: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: SyncLockBlock, 'SyncLock o. ... nd SyncLock')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of SyncLockBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub ILockStatement_ExpressionLock_ClassMethodCall()
            Dim source = <![CDATA[
Option Strict On

Public Class C1
    Public Sub M1()
        SyncLock M2()'BIND:"SyncLock M2()"
        End SyncLock
    End Sub

    Public Function M2() As Object
        Return New Object
    End Function
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ILockStatement ([0] OperationKind.LockStatement) (Syntax: SyncLockBlock, 'SyncLock M2 ... nd SyncLock') (Parent: BlockStatement)
  Expression: 
    IInvocationExpression ( Function C1.M2() As System.Object) ([0] OperationKind.InvocationExpression, Type: System.Object) (Syntax: InvocationExpression, 'M2()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C1, IsImplicit) (Syntax: IdentifierName, 'M2')
      Arguments(0)
  Body: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: SyncLockBlock, 'SyncLock M2 ... nd SyncLock')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of SyncLockBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub ILockStatement_ExpressionBody_SubMethod()
            Dim source = <![CDATA[
Option Strict On

Public Class C1
    Public Sub M1()
        SyncLock M2()'BIND:"SyncLock M2()"
        End SyncLock
    End Sub

    Public Sub M2()
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ILockStatement ([0] OperationKind.LockStatement, IsInvalid) (Syntax: SyncLockBlock, 'SyncLock M2 ... nd SyncLock') (Parent: BlockStatement)
  Expression: 
    IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'M2()')
      Children(1):
        IInvocationExpression ( Sub C1.M2()) ([0] OperationKind.InvocationExpression, Type: System.Void, IsInvalid) (Syntax: InvocationExpression, 'M2()')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C1, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M2')
          Arguments(0)
  Body: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: SyncLockBlock, 'SyncLock M2 ... nd SyncLock')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30491: Expression does not produce a value.
        SyncLock M2()'BIND:"SyncLock M2()"
                 ~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of SyncLockBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub ILockStatement_NonEmptyBody()
            Dim source = <![CDATA[
Option Strict On
Imports System

Public Class C1
    Public Sub M1()
        Dim o As New Object
        SyncLock o'BIND:"SyncLock o"
            Console.WriteLine("Hello World!")
        End SyncLock
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ILockStatement ([1] OperationKind.LockStatement) (Syntax: SyncLockBlock, 'SyncLock o' ... nd SyncLock') (Parent: BlockStatement)
  Expression: 
    ILocalReferenceExpression: o ([0] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'o')
  Body: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: SyncLockBlock, 'SyncLock o' ... nd SyncLock')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... lo World!")')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... lo World!")')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '"Hello World!"')
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "Hello World!") (Syntax: StringLiteralExpression, '"Hello World!"')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of SyncLockBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub
    End Class
End Namespace
