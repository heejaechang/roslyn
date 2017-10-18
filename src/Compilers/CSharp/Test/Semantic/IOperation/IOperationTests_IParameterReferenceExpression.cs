// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp.Test.Utilities;
using Microsoft.CodeAnalysis.Test.Utilities;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.CSharp.UnitTests
{
    public partial class IOperationTests : SemanticModelTestBase
    {
        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")]
        public void ParameterReference_TupleExpression()
        {
            string source = @"
class Class1
{
    public void M(int x, int y)
    {
        var tuple = /*<bind>*/(x, x + y)/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int32 x, System.Int32)) (Syntax: TupleExpression, '(x, x + y)') (Parent: VariableInitializer)
  Elements(2):
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
    IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'x + y')
      Left: 
        IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
      Right: 
        IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
";
            var expectedDiagnostics = new[] {
                // file.cs(6,13): warning CS0219: The variable 'tuple' is assigned but its value is never used
                //         var tuple = /*<bind>*/(x, x + y)/*</bind>*/;
                Diagnostic(ErrorCode.WRN_UnreferencedVarAssg, "tuple").WithArguments("tuple").WithLocation(6, 13)
            };

            VerifyOperationTreeAndDiagnosticsForTest<TupleExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")]
        public void ParameterReference_TupleDeconstruction()
        {
            string source = @"
class Point
{
    public int X { get; }
    public int Y { get; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void Deconstruct(out int x, out int y)
    {
        x = X;
        y = Y;
    }
}

class Class1
{
    public void M(Point point)
    {
        /*<bind>*/var (x, y) = point/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IDeconstructionAssignmentExpression ([0] OperationKind.DeconstructionAssignmentExpression, Type: (System.Int32 x, System.Int32 y)) (Syntax: SimpleAssignmentExpression, 'var (x, y) = point') (Parent: ExpressionStatement)
  Left: 
    IDeclarationExpression ([0] OperationKind.DeclarationExpression, Type: (System.Int32 x, System.Int32 y)) (Syntax: DeclarationExpression, 'var (x, y)')
      ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int32 x, System.Int32 y)) (Syntax: ParenthesizedVariableDesignation, '(x, y)')
        Elements(2):
          ILocalReferenceExpression: x (IsDeclaration: True) ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: SingleVariableDesignation, 'x')
          ILocalReferenceExpression: y (IsDeclaration: True) ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: SingleVariableDesignation, 'y')
  Right: 
    IParameterReferenceExpression: point ([1] OperationKind.ParameterReferenceExpression, Type: Point) (Syntax: IdentifierName, 'point')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<AssignmentExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")]
        public void ParameterReference_AnonymousObjectCreation()
        {
            string source = @"
class Class1
{
    public void M(int x, string y)
    {
        var v = /*<bind>*/new { Amount = x, Message = ""Hello"" + y }/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IAnonymousObjectCreationExpression ([0] OperationKind.AnonymousObjectCreationExpression, Type: <anonymous type: System.Int32 Amount, System.String Message>) (Syntax: AnonymousObjectCreationExpression, 'new { Amoun ... ello"" + y }') (Parent: VariableInitializer)
  Initializers(2):
    ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: AnonymousObjectMemberDeclarator, 'Amount = x')
      Left: 
        IPropertyReferenceExpression: System.Int32 <anonymous type: System.Int32 Amount, System.String Message>.Amount { get; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'Amount')
          Instance Receiver: 
            null
      Right: 
        IParameterReferenceExpression: x ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
    ISimpleAssignmentExpression ([1] OperationKind.SimpleAssignmentExpression, Type: System.String) (Syntax: AnonymousObjectMemberDeclarator, 'Message = ""Hello"" + y')
      Left: 
        IPropertyReferenceExpression: System.String <anonymous type: System.Int32 Amount, System.String Message>.Message { get; } ([0] OperationKind.PropertyReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'Message')
          Instance Receiver: 
            null
      Right: 
        IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.String) (Syntax: AddExpression, '""Hello"" + y')
          Left: 
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: ""Hello"") (Syntax: StringLiteralExpression, '""Hello""')
          Right: 
            IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'y')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<AnonymousObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")]
        public void ParameterReference_QueryExpression()
        {
            string source = @"
using System.Linq;
using System.Collections.Generic;

struct Customer
{
    public string Name { get; set; }
    public string Address { get; set; }
}

class Class1
{
    public void M(List<Customer> customers)
    {
        var result = /*<bind>*/from cust in customers
                     select cust.Name/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
ITranslatedQueryExpression ([0] OperationKind.TranslatedQueryExpression, Type: System.Collections.Generic.IEnumerable<System.String>) (Syntax: QueryExpression, 'from cust i ... t cust.Name') (Parent: VariableInitializer)
  Expression: 
    IInvocationExpression (System.Collections.Generic.IEnumerable<System.String> System.Linq.Enumerable.Select<Customer, System.String>(this System.Collections.Generic.IEnumerable<Customer> source, System.Func<Customer, System.String> selector)) ([0] OperationKind.InvocationExpression, Type: System.Collections.Generic.IEnumerable<System.String>, IsImplicit) (Syntax: SelectClause, 'select cust.Name')
      Instance Receiver: 
        null
      Arguments(2):
        IArgument (ArgumentKind.Explicit, Matching Parameter: source) ([0] OperationKind.Argument, IsImplicit) (Syntax: FromClause, 'from cust in customers')
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.Generic.IEnumerable<Customer>, IsImplicit) (Syntax: FromClause, 'from cust in customers')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IParameterReferenceExpression: customers ([0] OperationKind.ParameterReferenceExpression, Type: System.Collections.Generic.List<Customer>) (Syntax: IdentifierName, 'customers')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        IArgument (ArgumentKind.Explicit, Matching Parameter: selector) ([1] OperationKind.Argument, IsImplicit) (Syntax: SimpleMemberAccessExpression, 'cust.Name')
          IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Func<Customer, System.String>, IsImplicit) (Syntax: SimpleMemberAccessExpression, 'cust.Name')
            Target: 
              IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null, IsImplicit) (Syntax: SimpleMemberAccessExpression, 'cust.Name')
                IBlockStatement (1 statements) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: SimpleMemberAccessExpression, 'cust.Name')
                  IReturnStatement ([0] OperationKind.ReturnStatement, IsImplicit) (Syntax: SimpleMemberAccessExpression, 'cust.Name')
                    ReturnedValue: 
                      IPropertyReferenceExpression: System.String Customer.Name { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: System.String) (Syntax: SimpleMemberAccessExpression, 'cust.Name')
                        Instance Receiver: 
                          IOperation:  ([0] OperationKind.None) (Syntax: IdentifierName, 'cust')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<QueryExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")]
        public void ParameterReference_ObjectAndCollectionInitializer()
        {
            string source = @"
using System.Collections.Generic;

internal class Class
{
    public int X { get; set; }
    public List<int> Y { get; set; }
    public Dictionary<int, int> Z { get; set; }
    public Class C { get; set; }

    public void M(int x, int y, int z)
    {
        var c = /*<bind>*/new Class() { X = x, Y = { x, y, 3 }, Z = { { x, y } }, C = { X = z } }/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IObjectCreationExpression (Constructor: Class..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: Class) (Syntax: ObjectCreationExpression, 'new Class() ... { X = z } }') (Parent: VariableInitializer)
  Arguments(0)
  Initializer: 
    IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: Class) (Syntax: ObjectInitializerExpression, '{ X = x, Y  ... { X = z } }')
      Initializers(4):
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'X = x')
          Left: 
            IPropertyReferenceExpression: System.Int32 Class.X { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'X')
              Instance Receiver: 
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Class, IsImplicit) (Syntax: IdentifierName, 'X')
          Right: 
            IParameterReferenceExpression: x ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
        IMemberInitializerExpression ([1] OperationKind.MemberInitializerExpression, Type: System.Collections.Generic.List<System.Int32>) (Syntax: SimpleAssignmentExpression, 'Y = { x, y, 3 }')
          InitializedMember: 
            IPropertyReferenceExpression: System.Collections.Generic.List<System.Int32> Class.Y { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Collections.Generic.List<System.Int32>) (Syntax: IdentifierName, 'Y')
              Instance Receiver: 
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Class, IsImplicit) (Syntax: IdentifierName, 'Y')
          Initializer: 
            IObjectOrCollectionInitializerExpression ([1] OperationKind.ObjectOrCollectionInitializerExpression, Type: System.Collections.Generic.List<System.Int32>) (Syntax: CollectionInitializerExpression, '{ x, y, 3 }')
              Initializers(3):
                ICollectionElementInitializerExpression (AddMethod: void System.Collections.Generic.List<System.Int32>.Add(System.Int32 item)) (IsDynamic: False) ([0] OperationKind.CollectionElementInitializerExpression, Type: System.Void, IsImplicit) (Syntax: IdentifierName, 'x')
                  Arguments(1):
                    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                ICollectionElementInitializerExpression (AddMethod: void System.Collections.Generic.List<System.Int32>.Add(System.Int32 item)) (IsDynamic: False) ([1] OperationKind.CollectionElementInitializerExpression, Type: System.Void, IsImplicit) (Syntax: IdentifierName, 'y')
                  Arguments(1):
                    IParameterReferenceExpression: y ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
                ICollectionElementInitializerExpression (AddMethod: void System.Collections.Generic.List<System.Int32>.Add(System.Int32 item)) (IsDynamic: False) ([2] OperationKind.CollectionElementInitializerExpression, Type: System.Void, IsImplicit) (Syntax: NumericLiteralExpression, '3')
                  Arguments(1):
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3) (Syntax: NumericLiteralExpression, '3')
        IMemberInitializerExpression ([2] OperationKind.MemberInitializerExpression, Type: System.Collections.Generic.Dictionary<System.Int32, System.Int32>) (Syntax: SimpleAssignmentExpression, 'Z = { { x, y } }')
          InitializedMember: 
            IPropertyReferenceExpression: System.Collections.Generic.Dictionary<System.Int32, System.Int32> Class.Z { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Collections.Generic.Dictionary<System.Int32, System.Int32>) (Syntax: IdentifierName, 'Z')
              Instance Receiver: 
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Class, IsImplicit) (Syntax: IdentifierName, 'Z')
          Initializer: 
            IObjectOrCollectionInitializerExpression ([1] OperationKind.ObjectOrCollectionInitializerExpression, Type: System.Collections.Generic.Dictionary<System.Int32, System.Int32>) (Syntax: CollectionInitializerExpression, '{ { x, y } }')
              Initializers(1):
                ICollectionElementInitializerExpression (AddMethod: void System.Collections.Generic.Dictionary<System.Int32, System.Int32>.Add(System.Int32 key, System.Int32 value)) (IsDynamic: False) ([0] OperationKind.CollectionElementInitializerExpression, Type: System.Void, IsImplicit) (Syntax: ComplexElementInitializerExpression, '{ x, y }')
                  Arguments(2):
                    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                    IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
        IMemberInitializerExpression ([3] OperationKind.MemberInitializerExpression, Type: Class) (Syntax: SimpleAssignmentExpression, 'C = { X = z }')
          InitializedMember: 
            IPropertyReferenceExpression: Class Class.C { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: Class) (Syntax: IdentifierName, 'C')
              Instance Receiver: 
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Class, IsImplicit) (Syntax: IdentifierName, 'C')
          Initializer: 
            IObjectOrCollectionInitializerExpression ([1] OperationKind.ObjectOrCollectionInitializerExpression, Type: Class) (Syntax: ObjectInitializerExpression, '{ X = z }')
              Initializers(1):
                ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'X = z')
                  Left: 
                    IPropertyReferenceExpression: System.Int32 Class.X { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'X')
                      Instance Receiver: 
                        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Class, IsImplicit) (Syntax: IdentifierName, 'X')
                  Right: 
                    IParameterReferenceExpression: z ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'z')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")]
        public void ParameterReference_DelegateCreationExpressionWithLambdaArgument()
        {
            string source = @"
using System;

class Class
{
    // Used parameter methods
    public void UsedParameterMethod1(Action a)
    {
        Action a2 = /*<bind>*/new Action(() =>
        {
            a();
        })/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action) (Syntax: ObjectCreationExpression, 'new Action( ... })') (Parent: VariableInitializer)
  Target: 
    IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null) (Syntax: ParenthesizedLambdaExpression, '() => ... }')
      IBlockStatement (2 statements) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
        IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'a();')
          Expression: 
            IInvocationExpression (virtual void System.Action.Invoke()) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'a()')
              Instance Receiver: 
                IParameterReferenceExpression: a ([0] OperationKind.ParameterReferenceExpression, Type: System.Action) (Syntax: IdentifierName, 'a')
              Arguments(0)
        IReturnStatement ([1] OperationKind.ReturnStatement, IsImplicit) (Syntax: Block, '{ ... }')
          ReturnedValue: 
            null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")]
        public void ParameterReference_DelegateCreationExpressionWithMethodArgument()
        {
            string source = @"
using System;

class Class
{
    public delegate void Delegate(int x, int y);

    public void Method(Delegate d)
    {
        var a = /*<bind>*/new Delegate(Method2)/*</bind>*/;
    }

    public void Method2(int x, int y)
    {
    }
}
";
            string expectedOperationTree = @"
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: Class.Delegate) (Syntax: ObjectCreationExpression, 'new Delegate(Method2)') (Parent: VariableInitializer)
  Target: 
    IMethodReferenceExpression: void Class.Method2(System.Int32 x, System.Int32 y) ([0] OperationKind.MethodReferenceExpression, Type: null) (Syntax: IdentifierName, 'Method2')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Class, IsImplicit) (Syntax: IdentifierName, 'Method2')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")]
        public void ParameterReference_DelegateCreationExpressionWithInvalidArgument()
        {
            string source = @"
using System;

class Class
{
    public delegate void Delegate(int x, int y);

    public void Method(int x)
    {
        var a = /*<bind>*/new Delegate(x)/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: Class.Delegate, IsInvalid) (Syntax: ObjectCreationExpression, 'new Delegate(x)') (Parent: VariableInitializer)
  Children(1):
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32, IsInvalid) (Syntax: IdentifierName, 'x')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0149: Method name expected
                //         var a = /*<bind>*/new Delegate(x)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_MethodNameExpected, "x").WithLocation(10, 40)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")]
        public void ParameterReference_DynamicCollectionInitializer()
        {
            string source = @"
using System.Collections.Generic;

internal class Class
{
    public dynamic X { get; set; }

    public void M(int x, int y)
    {
        var c = new Class() /*<bind>*/{ X = { { x, y } } }/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: Class) (Syntax: ObjectInitializerExpression, '{ X = { { x, y } } }') (Parent: ObjectCreationExpression)
  Initializers(1):
    IMemberInitializerExpression ([0] OperationKind.MemberInitializerExpression, Type: dynamic) (Syntax: SimpleAssignmentExpression, 'X = { { x, y } }')
      InitializedMember: 
        IPropertyReferenceExpression: dynamic Class.X { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'X')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Class, IsImplicit) (Syntax: IdentifierName, 'X')
      Initializer: 
        IObjectOrCollectionInitializerExpression ([1] OperationKind.ObjectOrCollectionInitializerExpression, Type: dynamic) (Syntax: CollectionInitializerExpression, '{ { x, y } }')
          Initializers(1):
            ICollectionElementInitializerExpression (IsDynamic: True) ([0] OperationKind.CollectionElementInitializerExpression, Type: System.Void) (Syntax: ComplexElementInitializerExpression, '{ x, y }')
              Arguments(2):
                IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InitializerExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")]
        public void ParameterReference_NameOfExpression()
        {
            string source = @"
class Class1
{
    public string M(int x)
    {
        return /*<bind>*/nameof(x)/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
INameOfExpression ([0] OperationKind.NameOfExpression, Type: System.String, Constant: ""x"") (Syntax: InvocationExpression, 'nameof(x)') (Parent: ReturnStatement)
  IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")]
        public void ParameterReference_PointerIndirectionExpression()
        {
            string source = @"
class Class1
{
    public unsafe int M(int *x)
    {
        return /*<bind>*/*x/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IOperation:  ([0] OperationKind.None) (Syntax: PointerIndirectionExpression, '*x') (Parent: ReturnStatement)
  Children(1):
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32*) (Syntax: IdentifierName, 'x')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0227: Unsafe code may only appear if compiling with /unsafe
                //     public unsafe int M(int *x)
                Diagnostic(ErrorCode.ERR_IllegalUnsafe, "M").WithLocation(4, 23)
            };

            VerifyOperationTreeAndDiagnosticsForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")]
        public void ParameterReference_FixedLocalInitializer()
        {
            string source = @"
using System.Collections.Generic;

internal class Class
{
    public unsafe void M(int[] array)
    {
        fixed (int* /*<bind>*/p = array/*</bind>*/)
        {
            *p = 1;
        }
    }
}
";
            string expectedOperationTree = @"
IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'p = array') (Parent: VariableDeclarationStatement)
  Variables: Local_1: System.Int32* p
  Initializer: 
    IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= array')
      IOperation:  ([0] OperationKind.None) (Syntax: IdentifierName, 'array')
        Children(1):
          IParameterReferenceExpression: array ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32[]) (Syntax: IdentifierName, 'array')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0227: Unsafe code may only appear if compiling with /unsafe
                //     public unsafe void M(int[] array)
                Diagnostic(ErrorCode.ERR_IllegalUnsafe, "M").WithLocation(6, 24)
            };

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclaratorSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")]
        public void ParameterReference_RefTypeOperator()
        {
            string source = @"
class Class1
{
    public System.Type M(System.TypedReference x)
    {
        return /*<bind>*/__reftype(x)/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IOperation:  ([0] OperationKind.None) (Syntax: RefTypeExpression, '__reftype(x)') (Parent: ReturnStatement)
  Children(1):
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.TypedReference) (Syntax: IdentifierName, 'x')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<RefTypeExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")]
        public void ParameterReference_MakeRefOperator()
        {
            string source = @"
class Class1
{
    public void M(System.Type x)
    {
        var y = /*<bind>*/__makeref(x)/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IOperation:  ([0] OperationKind.None) (Syntax: MakeRefExpression, '__makeref(x)') (Parent: VariableInitializer)
  Children(1):
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Type) (Syntax: IdentifierName, 'x')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<MakeRefExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")]
        public void ParameterReference_RefValueOperator()
        {
            string source = @"
class Class1
{
    public void M(System.TypedReference x)
    {
        var y = /*<bind>*/__refvalue(x, int)/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IOperation:  ([0] OperationKind.None) (Syntax: RefValueExpression, '__refvalue(x, int)') (Parent: VariableInitializer)
  Children(1):
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.TypedReference) (Syntax: IdentifierName, 'x')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<RefValueExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")]
        public void ParameterReference_DynamicIndexerAccess()
        {
            string source = @"
class Class1
{
    public void M(dynamic d, int x)
    {
        var /*<bind>*/y = d[x]/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'y = d[x]') (Parent: VariableDeclarationStatement)
  Variables: Local_1: dynamic y
  Initializer: 
    IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= d[x]')
      IDynamicIndexerAccessExpression ([0] OperationKind.DynamicIndexerAccessExpression, Type: dynamic) (Syntax: ElementAccessExpression, 'd[x]')
        Expression: 
          IParameterReferenceExpression: d ([0] OperationKind.ParameterReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'd')
        Arguments(1):
          IParameterReferenceExpression: x ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
        ArgumentNames(0)
        ArgumentRefKinds(0)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclaratorSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")]
        public void ParameterReference_DynamicMemberAccess()
        {
            string source = @"
class Class1
{
    public void M(dynamic x, int y)
    {
        var z = /*<bind>*/x.M(y)/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IDynamicInvocationExpression ([0] OperationKind.DynamicInvocationExpression, Type: dynamic) (Syntax: InvocationExpression, 'x.M(y)') (Parent: VariableInitializer)
  Expression: 
    IDynamicMemberReferenceExpression (Member Name: ""M"", Containing Type: null) ([0] OperationKind.DynamicMemberReferenceExpression, Type: dynamic) (Syntax: SimpleMemberAccessExpression, 'x.M')
      Type Arguments(0)
      Instance Receiver: 
        IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'x')
  Arguments(1):
    IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  ArgumentNames(0)
  ArgumentRefKinds(0)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")]
        public void ParameterReference_DynamicInvocation()
        {
            string source = @"
class Class1
{
    public void M(dynamic x, int y)
    {
        var z = /*<bind>*/x(y)/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IDynamicInvocationExpression ([0] OperationKind.DynamicInvocationExpression, Type: dynamic) (Syntax: InvocationExpression, 'x(y)') (Parent: VariableInitializer)
  Expression: 
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'x')
  Arguments(1):
    IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  ArgumentNames(0)
  ArgumentRefKinds(0)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")]
        public void ParameterReference_DynamicObjectCreation()
        {
            string source = @"
internal class Class
{
    public Class(Class x) { }
    public Class(string x) { }

    public void M(dynamic x)
    {
        var c = /*<bind>*/new Class(x)/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IDynamicObjectCreationExpression ([0] OperationKind.DynamicObjectCreationExpression, Type: Class) (Syntax: ObjectCreationExpression, 'new Class(x)') (Parent: VariableInitializer)
  Arguments(1):
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'x')
  ArgumentNames(0)
  ArgumentRefKinds(0)
  Initializer: 
    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")]
        public void ParameterReference_StackAllocArrayCreation()
        {
            string source = @"
using System.Collections.Generic;

internal class Class
{
    public unsafe void M(int x)
    {
        int* block = /*<bind>*/stackalloc int[x]/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IOperation:  ([0] OperationKind.None) (Syntax: StackAllocArrayCreationExpression, 'stackalloc int[x]') (Parent: VariableInitializer)
  Children(1):
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0227: Unsafe code may only appear if compiling with /unsafe
                //     public unsafe void M(int x)
                Diagnostic(ErrorCode.ERR_IllegalUnsafe, "M").WithLocation(6, 24)
            };

            VerifyOperationTreeAndDiagnosticsForTest<StackAllocArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")]
        public void ParameterReference_InterpolatedStringExpression()
        {
            string source = @"
using System;

internal class Class
{
    public void M(string x, int y)
    {
        Console.WriteLine(/*<bind>*/$""String {x,20} and {y:D3} and constant {1}""/*</bind>*/);
    }
}
";
            string expectedOperationTree = @"
IInterpolatedStringExpression ([0] OperationKind.InterpolatedStringExpression, Type: System.String) (Syntax: InterpolatedStringExpression, '$""String {x ... nstant {1}""') (Parent: Argument)
  Parts(6):
    IInterpolatedStringText ([0] OperationKind.InterpolatedStringText) (Syntax: InterpolatedStringText, 'String ')
      Text: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: ""String "") (Syntax: InterpolatedStringText, 'String ')
    IInterpolation ([1] OperationKind.Interpolation) (Syntax: Interpolation, '{x,20}')
      Expression: 
        IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'x')
      Alignment: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 20) (Syntax: NumericLiteralExpression, '20')
      FormatString: 
        null
    IInterpolatedStringText ([2] OperationKind.InterpolatedStringText) (Syntax: InterpolatedStringText, ' and ')
      Text: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "" and "") (Syntax: InterpolatedStringText, ' and ')
    IInterpolation ([3] OperationKind.Interpolation) (Syntax: Interpolation, '{y:D3}')
      Expression: 
        IParameterReferenceExpression: y ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
      Alignment: 
        null
      FormatString: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.String, Constant: ""D3"") (Syntax: InterpolationFormatClause, ':D3')
    IInterpolatedStringText ([4] OperationKind.InterpolatedStringText) (Syntax: InterpolatedStringText, ' and constant ')
      Text: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "" and constant "") (Syntax: InterpolatedStringText, ' and constant ')
    IInterpolation ([5] OperationKind.Interpolation) (Syntax: Interpolation, '{1}')
      Expression: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      Alignment: 
        null
      FormatString: 
        null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InterpolatedStringExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")]
        public void ParameterReference_ThrowExpression()
        {
            string source = @"
using System;

internal class Class
{
    public void M(string x)
    {
        var y = x ?? /*<bind>*/throw new ArgumentNullException(nameof(x))/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IThrowExpression ([0] OperationKind.ThrowExpression, Type: null) (Syntax: ThrowExpression, 'throw new A ... (nameof(x))') (Parent: ConversionExpression)
  IObjectCreationExpression (Constructor: System.ArgumentNullException..ctor(System.String paramName)) ([0] OperationKind.ObjectCreationExpression, Type: System.ArgumentNullException) (Syntax: ObjectCreationExpression, 'new Argumen ... (nameof(x))')
    Arguments(1):
      IArgument (ArgumentKind.Explicit, Matching Parameter: paramName) ([0] OperationKind.Argument) (Syntax: Argument, 'nameof(x)')
        INameOfExpression ([0] OperationKind.NameOfExpression, Type: System.String, Constant: ""x"") (Syntax: InvocationExpression, 'nameof(x)')
          IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'x')
        InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    Initializer: 
      null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ThrowExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")]
        public void ParameterReference_PatternSwitchStatement()
        {
            string source = @"
internal class Class
{
    public void M(int x)
    {
        switch (x)
        {
            /*<bind>*/case var y when (x >= 10):
                break;/*</bind>*/
        }
    }
}
";
            string expectedOperationTree = @"
ISwitchCase (1 case clauses, 1 statements) ([1] OperationKind.SwitchCase) (Syntax: SwitchSection, 'case var y  ... break;') (Parent: SwitchStatement)
    Clauses:
      IPatternCaseClause (Label Symbol: case var y when (x >= 10):) (CaseKind.Pattern) ([0] OperationKind.CaseClause) (Syntax: CasePatternSwitchLabel, 'case var y  ...  (x >= 10):')
        Pattern: 
          IDeclarationPattern (Declared Symbol: System.Int32 y) ([0] OperationKind.DeclarationPattern) (Syntax: DeclarationPattern, 'var y')
        Guard Expression: 
          IBinaryOperatorExpression (BinaryOperatorKind.GreaterThanOrEqual) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanOrEqualExpression, 'x >= 10')
            Left: 
              IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
    Body:
      IBranchStatement (BranchKind.Break) ([1] OperationKind.BranchStatement) (Syntax: BreakStatement, 'break;')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<SwitchSectionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")]
        public void ParameterReference_DefaultPatternSwitchStatement()
        {
            string source = @"
internal class Class
{
    public void M(int x)
    {
        switch (x)
        {
            case var y when (x >= 10):
                break;

            /*<bind>*/default:/*</bind>*/
                break;
        }
    }
}
";
            string expectedOperationTree = @"
IDefaultCaseClause (CaseKind.Default) ([0] OperationKind.CaseClause) (Syntax: DefaultSwitchLabel, 'default:') (Parent: SwitchCase)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<DefaultSwitchLabelSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")]
        public void ParameterReference_UserDefinedLogicalConditionalOperator()
        {
            string source = @"
class A<T>
{
    public static bool operator true(A<T> o) { return true; }
    public static bool operator false(A<T> o) { return false; }
}
class B : A<object>
{
    public static B operator &(B x, B y) { return x; }
}
class C : B
{
    public static C operator |(C x, C y) { return x; }
}
class P
{
    static void M(C x, C y)
    {
        if (/*<bind>*/x && y/*</bind>*/)
        {
        }
    }
}
";
            string expectedOperationTree = @"
IOperation:  ([0] OperationKind.None) (Syntax: LogicalAndExpression, 'x && y') (Parent: ConversionExpression)
  Children(2):
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: B, IsImplicit) (Syntax: IdentifierName, 'x')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'x')
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: B, IsImplicit) (Syntax: IdentifierName, 'y')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IParameterReferenceExpression: y ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'y')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<BinaryExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")]
        public void ParameterReference_NoPiaObjectCreation()
        {
            var sources0 = @"
using System;
using System.Runtime.InteropServices;

[assembly: ImportedFromTypeLib(""_.dll"")]
[assembly: Guid(""f9c2d51d-4f44-45f0-9eda-c9d599b58257"")]
[ComImport()]
[Guid(""f9c2d51d-4f44-45f0-9eda-c9d599b58277"")]
[CoClass(typeof(C))]
public interface I
        {
            int P { get; set; }
        }
[Guid(""f9c2d51d-4f44-45f0-9eda-c9d599b58278"")]
public class C
{
    public C(object o)
    {
    }
}
";
            var sources1 = @"
struct S
{
	public I F(object x)
	{
		return /*<bind>*/new I(x)/*</bind>*/;
    }
}
";
            var compilation0 = CreateStandardCompilation(sources0);
            compilation0.VerifyDiagnostics();

            var compilation1 = CreateStandardCompilation(
                sources1,
                references: new[] { MscorlibRef, SystemRef, compilation0.EmitToImageReference(embedInteropTypes: true) });

            string expectedOperationTree = @"
IOperation:  ([1] OperationKind.None, IsInvalid) (Syntax: ObjectCreationExpression, 'new I(x)') (Parent: InvalidExpression)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                    // (6,25): error CS1729: 'I' does not contain a constructor that takes 1 arguments
                    // 		return /*<bind>*/new I(x)/*</bind>*/;
                    Diagnostic(ErrorCode.ERR_BadCtorArgCount, "(x)").WithArguments("I", "1").WithLocation(6, 25)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(compilation1, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")]
        public void ParameterReference_ArgListOperator()
        {
            string source = @"
using System;
class C
{
    static void Method(int x, bool y)
    {
        M(1, /*<bind>*/__arglist(x, y)/*</bind>*/);
    }
    
    static void M(int x, __arglist)
    {
    } 
}
";
            string expectedOperationTree = @"
IOperation:  ([0] OperationKind.None) (Syntax: InvocationExpression, '__arglist(x, y)') (Parent: Argument)
  Children(2):
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
    IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.Boolean) (Syntax: IdentifierName, 'y')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(19790, "https://github.com/dotnet/roslyn/issues/19790")]
        public void ParameterReference_IsPatternExpression()
        {
            string source = @"
class Class1
{
    public void Method1(object x)
    {
        if (/*<bind>*/x is int y/*</bind>*/) { }
    }
}
";
            string expectedOperationTree = @"
IIsPatternExpression ([0] OperationKind.IsPatternExpression, Type: System.Boolean) (Syntax: IsPatternExpression, 'x is int y') (Parent: IfStatement)
  Expression: 
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'x')
  Pattern: 
    IDeclarationPattern (Declared Symbol: System.Int32 y) ([1] OperationKind.DeclarationPattern) (Syntax: DeclarationPattern, 'int y')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<IsPatternExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(19902, "https://github.com/dotnet/roslyn/issues/19902")]
        public void ParameterReference_LocalFunctionStatement()
        {
            string source = @"
using System;
using System.Collections.Generic;

class Class
{
    static IEnumerable<T> MyIterator<T>(IEnumerable<T> source, Func<T, bool> predicate)
    {
        /*<bind>*/IEnumerable<T> Iterator()
        {
            foreach (var element in source)
                if (predicate(element))
                    yield return element;
        }/*</bind>*/

        return Iterator();
    }
}

";
            string expectedOperationTree = @"
ILocalFunctionStatement (Symbol: System.Collections.Generic.IEnumerable<T> Iterator()) ([0] OperationKind.LocalFunctionStatement) (Syntax: LocalFunctionStatement, 'IEnumerable ... }') (Parent: BlockStatement)
  IBlockStatement (2 statements) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
    IForEachLoopStatement (LoopKind.ForEach) ([0] OperationKind.LoopStatement) (Syntax: ForEachStatement, 'foreach (va ... rn element;')
      Locals: Local_1: T element
      LoopControlVariable: 
        ILocalReferenceExpression: element (IsDeclaration: True) ([1] OperationKind.LocalReferenceExpression, Type: T, Constant: null) (Syntax: ForEachStatement, 'foreach (va ... rn element;')
      Collection: 
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.Generic.IEnumerable<T>, IsImplicit) (Syntax: IdentifierName, 'source')
          Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            IParameterReferenceExpression: source ([0] OperationKind.ParameterReferenceExpression, Type: System.Collections.Generic.IEnumerable<T>) (Syntax: IdentifierName, 'source')
      Body: 
        IIfStatement ([2] OperationKind.IfStatement) (Syntax: IfStatement, 'if (predica ... rn element;')
          Condition: 
            IInvocationExpression (virtual System.Boolean System.Func<T, System.Boolean>.Invoke(T arg)) ([0] OperationKind.InvocationExpression, Type: System.Boolean) (Syntax: InvocationExpression, 'predicate(element)')
              Instance Receiver: 
                IParameterReferenceExpression: predicate ([0] OperationKind.ParameterReferenceExpression, Type: System.Func<T, System.Boolean>) (Syntax: IdentifierName, 'predicate')
              Arguments(1):
                IArgument (ArgumentKind.Explicit, Matching Parameter: arg) ([1] OperationKind.Argument) (Syntax: Argument, 'element')
                  ILocalReferenceExpression: element ([0] OperationKind.LocalReferenceExpression, Type: T) (Syntax: IdentifierName, 'element')
                  InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          IfTrue: 
            IReturnStatement ([1] OperationKind.YieldReturnStatement) (Syntax: YieldReturnStatement, 'yield return element;')
              ReturnedValue: 
                ILocalReferenceExpression: element ([0] OperationKind.LocalReferenceExpression, Type: T) (Syntax: IdentifierName, 'element')
          IfFalse: 
            null
      NextVariables(0)
    IReturnStatement ([1] OperationKind.YieldBreakStatement, IsImplicit) (Syntax: Block, '{ ... }')
      ReturnedValue: 
        null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LocalFunctionStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }
    }
}
