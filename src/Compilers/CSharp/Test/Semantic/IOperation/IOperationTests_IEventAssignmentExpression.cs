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
        public void AddEventHandler()
        {
            string source = @"
using System;

class Test
{
    public event EventHandler MyEvent;   
}

class C
{
    void Handler(object sender, EventArgs e)
    {
    } 

    void M()
    {
        var t = new Test();
        /*<bind>*/t.MyEvent += Handler/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IEventAssignmentExpression (EventAdd) ([0] OperationKind.EventAssignmentExpression, Type: System.Void) (Syntax: AddAssignmentExpression, 't.MyEvent += Handler') (Parent: ExpressionStatement)
  Event Reference: 
    IEventReferenceExpression: event System.EventHandler Test.MyEvent ([0] OperationKind.EventReferenceExpression, Type: System.EventHandler) (Syntax: SimpleMemberAccessExpression, 't.MyEvent')
      Instance Receiver: 
        ILocalReferenceExpression: t ([0] OperationKind.LocalReferenceExpression, Type: Test) (Syntax: IdentifierName, 't')
  Handler: 
    IDelegateCreationExpression ([1] OperationKind.DelegateCreationExpression, Type: System.EventHandler, IsImplicit) (Syntax: IdentifierName, 'Handler')
      Target: 
        IMethodReferenceExpression: void C.Handler(System.Object sender, System.EventArgs e) ([0] OperationKind.MethodReferenceExpression, Type: null) (Syntax: IdentifierName, 'Handler')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'Handler')
";
            var expectedDiagnostics = new[] {
                // file.cs(6,31): warning CS0067: The event 'Test.MyEvent' is never used
                //     public event EventHandler MyEvent;
                Diagnostic(ErrorCode.WRN_UnreferencedEvent, "MyEvent").WithArguments("Test.MyEvent").WithLocation(6, 31)
            };

            VerifyOperationTreeAndDiagnosticsForTest<AssignmentExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void AddEventHandler_JustHandlerReturnsMethodReference()
        {
            string source = @"
using System;

class Test
{
    public event EventHandler MyEvent;
}

class C
{
    void Handler(object sender, EventArgs e)
    {
    }

    void M()
    {
        var t = new Test();
        t.MyEvent += /*<bind>*/Handler/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IMethodReferenceExpression: void C.Handler(System.Object sender, System.EventArgs e) ([0] OperationKind.MethodReferenceExpression, Type: null) (Syntax: IdentifierName, 'Handler') (Parent: DelegateCreationExpression)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'Handler')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0067: The event 'Test.MyEvent' is never used
                //     public event EventHandler MyEvent;
                Diagnostic(ErrorCode.WRN_UnreferencedEvent, "MyEvent").WithArguments("Test.MyEvent").WithLocation(6, 31)
            };

            VerifyOperationTreeAndDiagnosticsForTest<IdentifierNameSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void RemoveEventHandler()
        {
            string source = @"
using System;

class Test
{
    public event EventHandler MyEvent;   
}

class C
{
    void M()
    {
        var t = new Test();
        /*<bind>*/t.MyEvent -= null/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IEventAssignmentExpression (EventRemove) ([0] OperationKind.EventAssignmentExpression, Type: System.Void) (Syntax: SubtractAssignmentExpression, 't.MyEvent -= null') (Parent: ExpressionStatement)
  Event Reference: 
    IEventReferenceExpression: event System.EventHandler Test.MyEvent ([0] OperationKind.EventReferenceExpression, Type: System.EventHandler) (Syntax: SimpleMemberAccessExpression, 't.MyEvent')
      Instance Receiver: 
        ILocalReferenceExpression: t ([0] OperationKind.LocalReferenceExpression, Type: Test) (Syntax: IdentifierName, 't')
  Handler: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.EventHandler, Constant: null, IsImplicit) (Syntax: NullLiteralExpression, 'null')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NullLiteralExpression, 'null')
";
            var expectedDiagnostics = new[] {
                // file.cs(6,31): warning CS0067: The event 'Test.MyEvent' is never used
                //     public event EventHandler MyEvent;
                Diagnostic(ErrorCode.WRN_UnreferencedEvent, "MyEvent").WithArguments("Test.MyEvent").WithLocation(6, 31)
            };

            VerifyOperationTreeAndDiagnosticsForTest<AssignmentExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void AddEventHandler_StaticEvent()
        {
            string source = @"
using System;

class Test
{
    public static event EventHandler MyEvent;    
}

class C
{
    void Handler(object sender, EventArgs e)
    {
    } 

    void M()
    {
        /*<bind>*/Test.MyEvent += Handler/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IEventAssignmentExpression (EventAdd) ([0] OperationKind.EventAssignmentExpression, Type: System.Void) (Syntax: AddAssignmentExpression, 'Test.MyEvent += Handler') (Parent: ExpressionStatement)
  Event Reference: 
    IEventReferenceExpression: event System.EventHandler Test.MyEvent (Static) ([0] OperationKind.EventReferenceExpression, Type: System.EventHandler) (Syntax: SimpleMemberAccessExpression, 'Test.MyEvent')
      Instance Receiver: 
        null
  Handler: 
    IDelegateCreationExpression ([1] OperationKind.DelegateCreationExpression, Type: System.EventHandler, IsImplicit) (Syntax: IdentifierName, 'Handler')
      Target: 
        IMethodReferenceExpression: void C.Handler(System.Object sender, System.EventArgs e) ([0] OperationKind.MethodReferenceExpression, Type: null) (Syntax: IdentifierName, 'Handler')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'Handler')
";
            var expectedDiagnostics = new[] {
                // file.cs(6,38): warning CS0067: The event 'Test.MyEvent' is never used
                //     public static event EventHandler MyEvent;
                Diagnostic(ErrorCode.WRN_UnreferencedEvent, "MyEvent").WithArguments("Test.MyEvent").WithLocation(6, 38)
            };

            VerifyOperationTreeAndDiagnosticsForTest<AssignmentExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void RemoveEventHandler_StaticEvent()
        {
            string source = @"
using System;

class Test
{
    public static event EventHandler MyEvent;    
}

class C
{
    void Handler(object sender, EventArgs e)
    {
    } 

    void M()
    {
        /*<bind>*/Test.MyEvent -= Handler/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IEventAssignmentExpression (EventRemove) ([0] OperationKind.EventAssignmentExpression, Type: System.Void) (Syntax: SubtractAssignmentExpression, 'Test.MyEvent -= Handler') (Parent: ExpressionStatement)
  Event Reference: 
    IEventReferenceExpression: event System.EventHandler Test.MyEvent (Static) ([0] OperationKind.EventReferenceExpression, Type: System.EventHandler) (Syntax: SimpleMemberAccessExpression, 'Test.MyEvent')
      Instance Receiver: 
        null
  Handler: 
    IDelegateCreationExpression ([1] OperationKind.DelegateCreationExpression, Type: System.EventHandler, IsImplicit) (Syntax: IdentifierName, 'Handler')
      Target: 
        IMethodReferenceExpression: void C.Handler(System.Object sender, System.EventArgs e) ([0] OperationKind.MethodReferenceExpression, Type: null) (Syntax: IdentifierName, 'Handler')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'Handler')
";
            var expectedDiagnostics = new[] {
                // file.cs(6,38): warning CS0067: The event 'Test.MyEvent' is never used
                //     public static event EventHandler MyEvent;
                Diagnostic(ErrorCode.WRN_UnreferencedEvent, "MyEvent").WithArguments("Test.MyEvent").WithLocation(6, 38)
            };

            VerifyOperationTreeAndDiagnosticsForTest<AssignmentExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void AddEventHandler_DelegateTypeMismatch()
        {
            string source = @"
using System;

class Test
{
    public event EventHandler MyEvent;    
}

class C
{
    void Handler(object sender)
    {
    } 

    void M()
    {
        var t = new Test();
        /*<bind>*/t.MyEvent += Handler/*<bind>*/;
    }
}
";
            string expectedOperationTree = @"
IEventAssignmentExpression (EventAdd) ([0] OperationKind.EventAssignmentExpression, Type: System.Void, IsInvalid) (Syntax: AddAssignmentExpression, 't.MyEvent += Handler') (Parent: ExpressionStatement)
  Event Reference: 
    IEventReferenceExpression: event System.EventHandler Test.MyEvent ([0] OperationKind.EventReferenceExpression, Type: System.EventHandler, IsInvalid) (Syntax: SimpleMemberAccessExpression, 't.MyEvent')
      Instance Receiver: 
        ILocalReferenceExpression: t ([0] OperationKind.LocalReferenceExpression, Type: Test, IsInvalid) (Syntax: IdentifierName, 't')
  Handler: 
    IDelegateCreationExpression ([1] OperationKind.DelegateCreationExpression, Type: System.EventHandler, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Handler')
      Target: 
        IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: IdentifierName, 'Handler')
          Children(1):
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Handler')
";
            var expectedDiagnostics = new[] {
                // file.cs(18,19): error CS0123: No overload for 'Handler' matches delegate 'EventHandler'
                //         /*<bind>*/t.MyEvent += Handler/*<bind>*/;
                Diagnostic(ErrorCode.ERR_MethDelegateMismatch, "t.MyEvent += Handler").WithArguments("Handler", "System.EventHandler").WithLocation(18, 19),
                // file.cs(6,31): warning CS0067: The event 'Test.MyEvent' is never used
                //     public event EventHandler MyEvent;
                Diagnostic(ErrorCode.WRN_UnreferencedEvent, "MyEvent").WithArguments("Test.MyEvent").WithLocation(6, 31)
            };

            VerifyOperationTreeAndDiagnosticsForTest<AssignmentExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void AddEventHandler_AssignToStaticEventOnInstance()
        {
            string source = @"
using System;

class Test
{
    public static event EventHandler MyEvent;    
}

class C
{
    void Handler(object sender, EventArgs e)
    {
    } 

    void M()
    {
        var t = new Test();
        /*<bind>*/t.MyEvent += Handler/*<bind>*/;
    }
}
";
            string expectedOperationTree = @"
IEventAssignmentExpression (EventAdd) ([0] OperationKind.EventAssignmentExpression, Type: System.Void, IsInvalid) (Syntax: AddAssignmentExpression, 't.MyEvent += Handler') (Parent: ExpressionStatement)
  Event Reference: 
    IEventReferenceExpression: event System.EventHandler Test.MyEvent (Static) ([0] OperationKind.EventReferenceExpression, Type: System.EventHandler, IsInvalid) (Syntax: SimpleMemberAccessExpression, 't.MyEvent')
      Instance Receiver: 
        null
  Handler: 
    IDelegateCreationExpression ([1] OperationKind.DelegateCreationExpression, Type: System.EventHandler, IsImplicit) (Syntax: IdentifierName, 'Handler')
      Target: 
        IMethodReferenceExpression: void C.Handler(System.Object sender, System.EventArgs e) ([0] OperationKind.MethodReferenceExpression, Type: null) (Syntax: IdentifierName, 'Handler')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'Handler')
";
            var expectedDiagnostics = new[] {
                // file.cs(18,19): error CS0176: Member 'Test.MyEvent' cannot be accessed with an instance reference; qualify it with a type name instead
                //         /*<bind>*/t.MyEvent += Handler/*<bind>*/;
                Diagnostic(ErrorCode.ERR_ObjectProhibited, "t.MyEvent").WithArguments("Test.MyEvent").WithLocation(18, 19),
                // file.cs(6,38): warning CS0067: The event 'Test.MyEvent' is never used
                //     public static event EventHandler MyEvent;
                Diagnostic(ErrorCode.WRN_UnreferencedEvent, "MyEvent").WithArguments("Test.MyEvent").WithLocation(6, 38)
            };

            VerifyOperationTreeAndDiagnosticsForTest<AssignmentExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        [WorkItem(8909, "https://github.com/dotnet/roslyn/issues/8909")]
        public void AddEventHandler_AssignToNonStaticEventOnType()
        {
            string source = @"
using System;

class Test
{
    public event EventHandler MyEvent;    
}

class C
{
    void Handler(object sender, EventArgs e)
    {
    } 

    void M()
    {
        /*<bind>*/Test.MyEvent += Handler/*<bind>*/;
    }
}
";
            string expectedOperationTree = @"
IEventAssignmentExpression (EventAdd) ([0] OperationKind.EventAssignmentExpression, Type: System.Void, IsInvalid) (Syntax: AddAssignmentExpression, 'Test.MyEvent += Handler') (Parent: ExpressionStatement)
  Event Reference: 
    IEventReferenceExpression: event System.EventHandler Test.MyEvent ([0] OperationKind.EventReferenceExpression, Type: System.EventHandler, IsInvalid) (Syntax: SimpleMemberAccessExpression, 'Test.MyEvent')
      Instance Receiver: 
        IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: IdentifierName, 'Test')
  Handler: 
    IDelegateCreationExpression ([1] OperationKind.DelegateCreationExpression, Type: System.EventHandler, IsImplicit) (Syntax: IdentifierName, 'Handler')
      Target: 
        IMethodReferenceExpression: void C.Handler(System.Object sender, System.EventArgs e) ([0] OperationKind.MethodReferenceExpression, Type: null) (Syntax: IdentifierName, 'Handler')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'Handler')
";
            var expectedDiagnostics = new[] {
                // file.cs(17,19): error CS0120: An object reference is required for the non-static field, method, or property 'Test.MyEvent'
                //         /*<bind>*/Test.MyEvent += Handler/*<bind>*/;
                Diagnostic(ErrorCode.ERR_ObjectRequired, "Test.MyEvent").WithArguments("Test.MyEvent").WithLocation(17, 19),
                // file.cs(6,31): warning CS0067: The event 'Test.MyEvent' is never used
                //     public event EventHandler MyEvent;
                Diagnostic(ErrorCode.WRN_UnreferencedEvent, "MyEvent").WithArguments("Test.MyEvent").WithLocation(6, 31)
            };

            VerifyOperationTreeAndDiagnosticsForTest<AssignmentExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void AddEventHandler_AssignToEventWithoutExplicitReceiver()
        {
            string source = @"
using System;

class Test
{
    public event EventHandler MyEvent;  

    void Handler(object sender, EventArgs e)
    {
    } 

    void M()
    {
        /*<bind>*/MyEvent += Handler/*<bind>*/;
    }  
}
";
            string expectedOperationTree = @"
IEventAssignmentExpression (EventAdd) ([0] OperationKind.EventAssignmentExpression, Type: System.Void) (Syntax: AddAssignmentExpression, 'MyEvent += Handler') (Parent: ExpressionStatement)
  Event Reference: 
    IEventReferenceExpression: event System.EventHandler Test.MyEvent ([0] OperationKind.EventReferenceExpression, Type: System.EventHandler) (Syntax: IdentifierName, 'MyEvent')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Test, IsImplicit) (Syntax: IdentifierName, 'MyEvent')
  Handler: 
    IDelegateCreationExpression ([1] OperationKind.DelegateCreationExpression, Type: System.EventHandler, IsImplicit) (Syntax: IdentifierName, 'Handler')
      Target: 
        IMethodReferenceExpression: void Test.Handler(System.Object sender, System.EventArgs e) ([0] OperationKind.MethodReferenceExpression, Type: null) (Syntax: IdentifierName, 'Handler')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Test, IsImplicit) (Syntax: IdentifierName, 'Handler')
";
            var expectedDiagnostics = new[] {
                      // file.cs(6,31): warning CS0067: The event 'Test.MyEvent' is never used
                      //     public event EventHandler MyEvent;
                      Diagnostic(ErrorCode.WRN_UnreferencedEvent, "MyEvent").WithArguments("Test.MyEvent").WithLocation(6, 31)
            };

            VerifyOperationTreeAndDiagnosticsForTest<AssignmentExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }
    }
}
