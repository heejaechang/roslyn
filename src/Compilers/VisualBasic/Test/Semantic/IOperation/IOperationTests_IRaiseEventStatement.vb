' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.Test.Utilities
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax

Namespace Microsoft.CodeAnalysis.VisualBasic.UnitTests.Semantics

    Partial Public Class IOperationTests
        Inherits SemanticModelTestBase        

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub RaiseInstanceEvent()
            Dim source = <![CDATA[
Imports System

Class TestClass
    
    Event TestEvent As Action

    Sub M()
        RaiseEvent TestEvent()'BIND:"RaiseEvent TestEvent()"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IRaiseEventStatement ([0] OperationKind.RaiseEventStatement) (Syntax: RaiseEventStatement, 'RaiseEvent TestEvent()') (Parent: BlockStatement)
  Event Reference: 
    IEventReferenceExpression: Event TestClass.TestEvent As System.Action ([0] OperationKind.EventReferenceExpression, Type: System.Action) (Syntax: IdentifierName, 'TestEvent')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: TestClass, IsImplicit) (Syntax: IdentifierName, 'TestEvent')
  Arguments(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty
            VerifyOperationTreeAndDiagnosticsForTest(Of RaiseEventStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub RaiseInstanceEventWithArguments()
            Dim source = <![CDATA[
Imports System

Class TestClass
    
    Public Event MyEvent(x As String, y As Integer)

    Sub M()
        RaiseEvent MyEvent(y:=1, x:=String.Empty)'BIND:"RaiseEvent MyEvent(y:=1, x:=String.Empty)"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IRaiseEventStatement ([0] OperationKind.RaiseEventStatement) (Syntax: RaiseEventStatement, 'RaiseEvent  ... ring.Empty)') (Parent: BlockStatement)
  Event Reference: 
    IEventReferenceExpression: Event TestClass.MyEvent(x As System.String, y As System.Int32) ([0] OperationKind.EventReferenceExpression, Type: TestClass.MyEventEventHandler) (Syntax: IdentifierName, 'MyEvent')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: TestClass, IsImplicit) (Syntax: IdentifierName, 'MyEvent')
  Arguments(2):
    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([1] OperationKind.Argument) (Syntax: SimpleArgument, 'x:=String.Empty')
      IFieldReferenceExpression: System.String.Empty As System.String (Static) ([0] OperationKind.FieldReferenceExpression, Type: System.String) (Syntax: SimpleMemberAccessExpression, 'String.Empty')
        Instance Receiver: 
          null
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.Explicit, Matching Parameter: y) ([2] OperationKind.Argument) (Syntax: SimpleArgument, 'y:=1')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
]]>.Value

            Dim expectedDiagnostics = String.Empty
            VerifyOperationTreeAndDiagnosticsForTest(Of RaiseEventStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub RaiseSharedEvent()
            Dim source = <![CDATA[
Imports System

Class TestClass
    
    Shared Event TestEvent As Action

    Sub M()
        RaiseEvent TestEvent()'BIND:"RaiseEvent TestEvent()"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IRaiseEventStatement ([0] OperationKind.RaiseEventStatement) (Syntax: RaiseEventStatement, 'RaiseEvent TestEvent()') (Parent: BlockStatement)
  Event Reference: 
    IEventReferenceExpression: Event TestClass.TestEvent As System.Action (Static) ([0] OperationKind.EventReferenceExpression, Type: System.Action) (Syntax: IdentifierName, 'TestEvent')
      Instance Receiver: 
        null
  Arguments(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty
            VerifyOperationTreeAndDiagnosticsForTest(Of RaiseEventStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub RaiseInstanceEventWithExtraArgument()
            Dim source = <![CDATA[
Imports System

Class TestClass
    
    Event TestEvent As Action

    Sub M()
        RaiseEvent TestEvent(1)'BIND:"RaiseEvent TestEvent(1)"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IInvalidStatement ([0] OperationKind.InvalidStatement, IsInvalid) (Syntax: RaiseEventStatement, 'RaiseEvent TestEvent(1)') (Parent: BlockStatement)
  Children(1):
    IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Void, IsInvalid, IsImplicit) (Syntax: RaiseEventStatement, 'RaiseEvent TestEvent(1)')
      Children(2):
        IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: RaiseEventStatement, 'RaiseEvent TestEvent(1)')
          Children(1):
            IFieldReferenceExpression: TestClass.TestEventEvent As System.Action ([0] OperationKind.FieldReferenceExpression, Type: System.Action, IsImplicit) (Syntax: IdentifierName, 'TestEvent')
              Instance Receiver: 
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: TestClass, IsImplicit) (Syntax: IdentifierName, 'TestEvent')
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid) (Syntax: NumericLiteralExpression, '1')
]]>.Value
            
            Dim expectedDiagnostics = <![CDATA[
BC30057: Too many arguments to 'Public Event TestEvent As Action'.
        RaiseEvent TestEvent(1)'BIND:"RaiseEvent TestEvent(1)"
                             ~
]]>.Value
            VerifyOperationTreeAndDiagnosticsForTest(Of RaiseEventStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub RaiseUndefinedEvent()
            Dim source = <![CDATA[
Imports System

Class TestClass
    
    Event TestEvent As Action

    Sub M()
        RaiseEvent TestEvent2()'BIND:"RaiseEvent TestEvent2()"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IInvalidStatement ([0] OperationKind.InvalidStatement, IsInvalid) (Syntax: RaiseEventStatement, 'RaiseEvent TestEvent2()') (Parent: BlockStatement)
  Children(1):
    IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: IdentifierName, 'TestEvent2')
      Children(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30451: 'TestEvent2' is not declared. It may be inaccessible due to its protection level.
        RaiseEvent TestEvent2()'BIND:"RaiseEvent TestEvent2()"
                   ~~~~~~~~~~
]]>.Value
            VerifyOperationTreeAndDiagnosticsForTest(Of RaiseEventStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub RaiseEventThroughField1()
            Dim source = <![CDATA[
Imports System

Class TestClass
    
    Event TestEvent As Action

    Sub M()
        TestEventEvent()'BIND:"TestEventEvent()"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IInvocationExpression (virtual Sub System.Action.Invoke()) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'TestEventEvent()') (Parent: ExpressionStatement)
  Instance Receiver: 
    IFieldReferenceExpression: TestClass.TestEventEvent As System.Action ([0] OperationKind.FieldReferenceExpression, Type: System.Action) (Syntax: IdentifierName, 'TestEventEvent')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: TestClass, IsImplicit) (Syntax: IdentifierName, 'TestEventEvent')
  Arguments(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty
            VerifyOperationTreeAndDiagnosticsForTest(Of InvocationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub RaiseEventThroughField2()
            Dim source = <![CDATA[
Imports System

Class TestClass
    
    Event TestEvent As Action

    Sub M()
        TestEventEvent.Invoke()'BIND:"TestEventEvent.Invoke()"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IInvocationExpression (virtual Sub System.Action.Invoke()) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'TestEventEvent.Invoke()') (Parent: ExpressionStatement)
  Instance Receiver: 
    IFieldReferenceExpression: TestClass.TestEventEvent As System.Action ([0] OperationKind.FieldReferenceExpression, Type: System.Action) (Syntax: IdentifierName, 'TestEventEvent')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: TestClass, IsImplicit) (Syntax: IdentifierName, 'TestEventEvent')
  Arguments(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty
            VerifyOperationTreeAndDiagnosticsForTest(Of InvocationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub RaiseCustomEvent()
            Dim source = <![CDATA[
Imports System

Class TestClass   
    Public Custom Event TestEvent As Action
        AddHandler(value As Action)
        End AddHandler

        RemoveHandler(value As Action)
        End RemoveHandler

        RaiseEvent()
        End RaiseEvent
	End Event

    Sub M()
        RaiseEvent TestEvent()'BIND:"RaiseEvent TestEvent()"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IRaiseEventStatement ([0] OperationKind.RaiseEventStatement) (Syntax: RaiseEventStatement, 'RaiseEvent TestEvent()') (Parent: BlockStatement)
  Event Reference: 
    IEventReferenceExpression: Event TestClass.TestEvent As System.Action ([0] OperationKind.EventReferenceExpression, Type: System.Action) (Syntax: IdentifierName, 'TestEvent')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: TestClass, IsImplicit) (Syntax: IdentifierName, 'TestEvent')
  Arguments(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty
            VerifyOperationTreeAndDiagnosticsForTest(Of RaiseEventStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub RaiseCustomEventWithArguments()
            Dim source = <![CDATA[
Imports System

Class TestClass   
    Public Custom Event TestEvent As Eventhandler
        AddHandler(value As Eventhandler)
        End AddHandler

        RemoveHandler(value As Eventhandler)
        End RemoveHandler

        RaiseEvent(sender As Object, e As EventArgs)
        End RaiseEvent
	End Event

    Sub M()
        RaiseEvent TestEvent(Nothing, Nothing)'BIND:"RaiseEvent TestEvent(Nothing, Nothing)"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IRaiseEventStatement ([0] OperationKind.RaiseEventStatement) (Syntax: RaiseEventStatement, 'RaiseEvent  ... g, Nothing)') (Parent: BlockStatement)
  Event Reference: 
    IEventReferenceExpression: Event TestClass.TestEvent As System.EventHandler ([0] OperationKind.EventReferenceExpression, Type: System.EventHandler) (Syntax: IdentifierName, 'TestEvent')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: TestClass, IsImplicit) (Syntax: IdentifierName, 'TestEvent')
  Arguments(2):
    IArgument (ArgumentKind.Explicit, Matching Parameter: sender) ([1] OperationKind.Argument) (Syntax: SimpleArgument, 'Nothing')
      IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, Constant: null, IsImplicit) (Syntax: NothingLiteralExpression, 'Nothing')
        Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        Operand: 
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NothingLiteralExpression, 'Nothing')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.Explicit, Matching Parameter: e) ([2] OperationKind.Argument) (Syntax: SimpleArgument, 'Nothing')
      IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.EventArgs, Constant: null, IsImplicit) (Syntax: NothingLiteralExpression, 'Nothing')
        Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        Operand: 
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NothingLiteralExpression, 'Nothing')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
]]>.Value

            Dim expectedDiagnostics = String.Empty
            VerifyOperationTreeAndDiagnosticsForTest(Of RaiseEventStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub
        
        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub EventAccessFromRaiseEventShouldReturnEventReference()
            Dim source = <![CDATA[
Imports System

Class TestClass   
   Event TestEvent As Action

    Sub M()
        RaiseEvent TestEvent()'BIND:"TestEvent"
    End Sub
End Class]]>.Value
            
            Dim expectedOperationTree = <![CDATA[
IEventReferenceExpression: Event TestClass.TestEvent As System.Action ([0] OperationKind.EventReferenceExpression, Type: System.Action) (Syntax: IdentifierName, 'TestEvent') (Parent: RaiseEventStatement)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: TestClass, IsImplicit) (Syntax: IdentifierName, 'TestEvent')
]]>.Value
            Dim expectedDiagnostics = String.Empty
            VerifyOperationTreeAndDiagnosticsForTest(Of IdentifierNameSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub EventAccessFromRaiseCustomEventShouldReturnEventReference()
            Dim source = <![CDATA[
Imports System

Class TestClass   
    Public Custom Event TestEvent As Action
        AddHandler(value As Action)
        End AddHandler

        RemoveHandler(value As Action)
        End RemoveHandler

        RaiseEvent()
        End RaiseEvent
	End Event

    Sub M()
        RaiseEvent TestEvent()'BIND:"TestEvent"
    End Sub
End Class]]>.Value
            
            Dim expectedOperationTree = <![CDATA[
IEventReferenceExpression: Event TestClass.TestEvent As System.Action ([0] OperationKind.EventReferenceExpression, Type: System.Action) (Syntax: IdentifierName, 'TestEvent') (Parent: RaiseEventStatement)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: TestClass, IsImplicit) (Syntax: IdentifierName, 'TestEvent')
]]>.Value
            Dim expectedDiagnostics = String.Empty
            VerifyOperationTreeAndDiagnosticsForTest(Of IdentifierNameSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub
    End Class
End Namespace
