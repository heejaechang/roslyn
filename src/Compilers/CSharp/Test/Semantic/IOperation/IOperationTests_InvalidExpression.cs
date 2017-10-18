﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp.Test.Utilities;
using Microsoft.CodeAnalysis.Test.Utilities;
using Roslyn.Test.Utilities;
using System.Linq;
using Xunit;

namespace Microsoft.CodeAnalysis.CSharp.UnitTests
{
    public partial class IOperationTests : SemanticModelTestBase
    {
        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17598, "https://github.com/dotnet/roslyn/issues/17598")]
        public void InvalidInvocationExpression_BadReceiver()
        {
            string source = @"
using System;

class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/Console.WriteLine2()/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: InvocationExpression, 'Console.WriteLine2()') (Parent: ExpressionStatement)
  Children(1):
    IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: SimpleMemberAccessExpression, 'Console.WriteLine2')
      Children(1):
        IOperation:  ([0] OperationKind.None) (Syntax: IdentifierName, 'Console')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0117: 'Console' does not contain a definition for 'WriteLine2'
                //         /*<bind>*/Console.WriteLine2()/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NoSuchMember, "WriteLine2").WithArguments("System.Console", "WriteLine2").WithLocation(8, 27)
            };

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17598, "https://github.com/dotnet/roslyn/issues/17598")]
        public void InvalidInvocationExpression_OverloadResolutionFailureBadArgument()
        {
            string source = @"
using System;

class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/F(string.Empty)/*</bind>*/;
    }

    void F(int x)
    {
    }
}
";
            string expectedOperationTree = @"
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Void, IsInvalid) (Syntax: InvocationExpression, 'F(string.Empty)') (Parent: ExpressionStatement)
  Children(1):
    IFieldReferenceExpression: System.String System.String.Empty (Static) ([0] OperationKind.FieldReferenceExpression, Type: System.String, IsInvalid) (Syntax: SimpleMemberAccessExpression, 'string.Empty')
      Instance Receiver: 
        null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1503: Argument 1: cannot convert from 'string' to 'int'
                //         /*<bind>*/F(string.Empty)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_BadArgType, "string.Empty").WithArguments("1", "string", "int").WithLocation(8, 21)
            };

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17598, "https://github.com/dotnet/roslyn/issues/17598")]
        public void InvalidInvocationExpression_OverloadResolutionFailureExtraArgument()
        {
            string source = @"
using System;

class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/F(string.Empty)/*</bind>*/;
    }

    void F()
    {
    }
}
";
            string expectedOperationTree = @"
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Void, IsInvalid) (Syntax: InvocationExpression, 'F(string.Empty)') (Parent: ExpressionStatement)
  Children(1):
    IFieldReferenceExpression: System.String System.String.Empty (Static) ([0] OperationKind.FieldReferenceExpression, Type: System.String) (Syntax: SimpleMemberAccessExpression, 'string.Empty')
      Instance Receiver: 
        null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1501: No overload for method 'F' takes 1 arguments
                //         /*<bind>*/F(string.Empty)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_BadArgCount, "F").WithArguments("F", "1").WithLocation(8, 19)
            };

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17598, "https://github.com/dotnet/roslyn/issues/17598")]
        public void InvalidFieldReferenceExpression()
        {
            string source = @"
using System;

class Program
{
    static void Main(string[] args)
    {
        var x = new Program();
        var /*<bind>*/y = x.MissingField/*</bind>*/;
    }

    void F()
    {
    }
}
";
            string expectedOperationTree = @"
IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'y = x.MissingField') (Parent: VariableDeclarationStatement)
  Variables: Local_1: ? y
  Initializer: 
    IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= x.MissingField')
      IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: SimpleMemberAccessExpression, 'x.MissingField')
        Children(1):
          ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: Program) (Syntax: IdentifierName, 'x')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1061: 'Program' does not contain a definition for 'MissingField' and no extension method 'MissingField' accepting a first argument of type 'Program' could be found (are you missing a using directive or an assembly reference?)
                //         var y /*<bind>*/= x.MissingField/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NoSuchMemberOrExtension, "MissingField").WithArguments("Program", "MissingField").WithLocation(9, 29)
            };

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclaratorSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17598, "https://github.com/dotnet/roslyn/issues/17598")]
        public void InvalidConversionExpression_ImplicitCast()
        {
            string source = @"
using System;

class Program
{
    int i1;
    static void Main(string[] args)
    {
        var x = new Program();
        /*<bind>*/string y = x.i1;/*</bind>*/
    }

    void F()
    {
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([1] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'string y = x.i1;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'y = x.i1')
    Variables: Local_1: System.String y
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= x.i1')
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.String, IsInvalid, IsImplicit) (Syntax: SimpleMemberAccessExpression, 'x.i1')
          Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            IFieldReferenceExpression: System.Int32 Program.i1 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32, IsInvalid) (Syntax: SimpleMemberAccessExpression, 'x.i1')
              Instance Receiver: 
                ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: Program, IsInvalid) (Syntax: IdentifierName, 'x')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0029: Cannot implicitly convert type 'int' to 'string'
                //         string y /*<bind>*/= x.i1/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NoImplicitConv, "x.i1").WithArguments("int", "string").WithLocation(10, 30),
                // CS0649: Field 'Program.i1' is never assigned to, and will always have its default value 0
                //     int i1;
                Diagnostic(ErrorCode.WRN_UnassignedInternalField, "i1").WithArguments("Program.i1", "0").WithLocation(6, 9)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17598, "https://github.com/dotnet/roslyn/issues/17598")]
        public void InvalidConversionExpression_ExplicitCast()
        {
            string source = @"
using System;

class Program
{
    int i1;
    static void Main(string[] args)
    {
        var x = new Program();
        /*<bind>*/Program y = (Program)x.i1;/*</bind>*/
    }

    void F()
    {
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([1] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Program y = ... ogram)x.i1;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'y = (Program)x.i1')
    Variables: Local_1: Program y
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= (Program)x.i1')
        IConversionExpression (Explicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: Program, IsInvalid) (Syntax: CastExpression, '(Program)x.i1')
          Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            IFieldReferenceExpression: System.Int32 Program.i1 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32, IsInvalid) (Syntax: SimpleMemberAccessExpression, 'x.i1')
              Instance Receiver: 
                ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: Program, IsInvalid) (Syntax: IdentifierName, 'x')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0030: Cannot convert type 'int' to 'Program'
                //         Program y /*<bind>*/= (Program)x.i1/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NoExplicitConv, "(Program)x.i1").WithArguments("int", "Program").WithLocation(10, 31),
                // CS0649: Field 'Program.i1' is never assigned to, and will always have its default value 0
                //     int i1;
                Diagnostic(ErrorCode.WRN_UnassignedInternalField, "i1").WithArguments("Program.i1", "0").WithLocation(6, 9)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17598, "https://github.com/dotnet/roslyn/issues/17598")]
        public void InvalidUnaryExpression()
        {
            string source = @"
using System;

class Program
{
    static void Main(string[] args)
    {
        var x = new Program();
        Console.Write(/*<bind>*/++x/*</bind>*/);
    }

    void F()
    {
    }
}
";
            string expectedOperationTree = @"
IIncrementOrDecrementExpression (Prefix) ([0] OperationKind.IncrementExpression, Type: System.Object, IsInvalid) (Syntax: PreIncrementExpression, '++x') (Parent: Argument)
  Target: 
    ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: Program, IsInvalid) (Syntax: IdentifierName, 'x')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0023: Operator '++' cannot be applied to operand of type 'Program'
                //         Console.Write(/*<bind>*/++x/*</bind>*/);
                Diagnostic(ErrorCode.ERR_BadUnaryOp, "++x").WithArguments("++", "Program").WithLocation(9, 33)
            };

            VerifyOperationTreeAndDiagnosticsForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17598, "https://github.com/dotnet/roslyn/issues/17598")]
        public void InvalidBinaryExpression()
        {
            string source = @"
using System;

class Program
{
    static void Main(string[] args)
    {
        var x = new Program();
        Console.Write(/*<bind>*/x + (y * args.Length)/*</bind>*/);
    }

    void F()
    {
    }
}
";
            string expectedOperationTree = @"
IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: ?, IsInvalid) (Syntax: AddExpression, 'x + (y * args.Length)') (Parent: InvalidExpression)
  Left: 
    ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: Program) (Syntax: IdentifierName, 'x')
  Right: 
    IBinaryOperatorExpression (BinaryOperatorKind.Multiply) ([1] OperationKind.BinaryOperatorExpression, Type: ?, IsInvalid) (Syntax: MultiplyExpression, 'y * args.Length')
      Left: 
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: IdentifierName, 'y')
          Children(0)
      Right: 
        IPropertyReferenceExpression: System.Int32 System.Array.Length { get; } ([1] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: SimpleMemberAccessExpression, 'args.Length')
          Instance Receiver: 
            IParameterReferenceExpression: args ([0] OperationKind.ParameterReferenceExpression, Type: System.String[]) (Syntax: IdentifierName, 'args')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0103: The name 'y' does not exist in the current context
                //         Console.Write(/*<bind>*/x + (y * args.Length)/*</bind>*/);
                Diagnostic(ErrorCode.ERR_NameNotInContext, "y").WithArguments("y").WithLocation(9, 38)
            };

            VerifyOperationTreeAndDiagnosticsForTest<BinaryExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17598, "https://github.com/dotnet/roslyn/issues/17598")]
        public void InvalidLambdaBinding_UnboundLambda()
        {
            string source = @"
using System;

class Program
{
    static void Main(string[] args)
    {
        var /*<bind>*/x = () => F()/*</bind>*/;
    }

    static void F()
    {
    }
}
";
            string expectedOperationTree = @"
IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'x = () => F()') (Parent: VariableDeclarationStatement)
  Variables: Local_1: var x
  Initializer: 
    IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= () => F()')
      IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null, IsInvalid) (Syntax: ParenthesizedLambdaExpression, '() => F()')
        IBlockStatement (1 statements) ([0] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'F()')
          IExpressionStatement ([0] OperationKind.ExpressionStatement, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'F()')
            Expression: 
              IInvocationExpression (void Program.F()) ([0] OperationKind.InvocationExpression, Type: System.Void, IsInvalid) (Syntax: InvocationExpression, 'F()')
                Instance Receiver: 
                  null
                Arguments(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0815: Cannot assign lambda expression to an implicitly-typed variable
                //         var /*<bind>*/x = () => F()/*</bind>*/;
                Diagnostic(ErrorCode.ERR_ImplicitlyTypedVariableAssignedBadValue, "x = () => F()").WithArguments("lambda expression").WithLocation(8, 23)
            };

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclaratorSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17598, "https://github.com/dotnet/roslyn/issues/17598")]
        public void InvalidLambdaBinding_LambdaExpression()
        {
            string source = @"
using System;

class Program
{
    static void Main(string[] args)
    {
        var x = /*<bind>*/() => F()/*</bind>*/;
    }

    static void F()
    {
    }
}
";
            string expectedOperationTree = @"
IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null, IsInvalid) (Syntax: ParenthesizedLambdaExpression, '() => F()') (Parent: VariableInitializer)
  IBlockStatement (1 statements) ([0] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'F()')
    IExpressionStatement ([0] OperationKind.ExpressionStatement, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'F()')
      Expression: 
        IInvocationExpression (void Program.F()) ([0] OperationKind.InvocationExpression, Type: System.Void, IsInvalid) (Syntax: InvocationExpression, 'F()')
          Instance Receiver: 
            null
          Arguments(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0815: Cannot assign lambda expression to an implicitly-typed variable
                //         var x = /*<bind>*/() => F()/*</bind>*/;
                Diagnostic(ErrorCode.ERR_ImplicitlyTypedVariableAssignedBadValue, "x = /*<bind>*/() => F()").WithArguments("lambda expression").WithLocation(8, 13)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ParenthesizedLambdaExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17598, "https://github.com/dotnet/roslyn/issues/17598")]
        public void InvalidFieldInitializer()
        {
            string source = @"
class Program
{
    int x /*<bind>*/= Program/*</bind>*/;
    static void Main(string[] args)
    {
        var x = new Program() { x = Program };
    }
}
";
            string expectedOperationTree = @"
IFieldInitializer (Field: System.Int32 Program.x) ([Root] OperationKind.FieldInitializer, IsInvalid) (Syntax: EqualsValueClause, '= Program') (Parent: )
  IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Program')
    Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    Operand: 
      IInvalidExpression ([0] OperationKind.InvalidExpression, Type: Program, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Program')
        Children(1):
          IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: IdentifierName, 'Program')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0119: 'Program' is a type, which is not valid in the given context
                //     int x /*<bind>*/= Program/*</bind>*/;
                Diagnostic(ErrorCode.ERR_BadSKunknown, "Program").WithArguments("Program", "type").WithLocation(4, 23),
                // CS0119: 'Program' is a type, which is not valid in the given context
                //         var x = new Program() { x = Program };
                Diagnostic(ErrorCode.ERR_BadSKunknown, "Program").WithArguments("Program", "type").WithLocation(7, 37)
            };

            VerifyOperationTreeAndDiagnosticsForTest<EqualsValueClauseSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17598, "https://github.com/dotnet/roslyn/issues/17598")]
        public void InvalidArrayInitializer()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        var x = new int[2, 2] /*<bind>*/{ { { 1, 1 } }, { 2, 2 } }/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IArrayInitializer (2 elements) ([2] OperationKind.ArrayInitializer, IsInvalid) (Syntax: ArrayInitializerExpression, '{ { { 1, 1  ...  { 2, 2 } }') (Parent: ArrayCreationExpression)
  Element Values(2):
    IArrayInitializer (1 elements) ([0] OperationKind.ArrayInitializer, IsInvalid) (Syntax: ArrayInitializerExpression, '{ { 1, 1 } }')
      Element Values(1):
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: ArrayInitializerExpression, '{ 1, 1 }')
          Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid, IsImplicit) (Syntax: ArrayInitializerExpression, '{ 1, 1 }')
              Children(1):
                IArrayInitializer (2 elements) ([0] OperationKind.ArrayInitializer, IsInvalid) (Syntax: ArrayInitializerExpression, '{ 1, 1 }')
                  Element Values(2):
                    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsInvalid, IsImplicit) (Syntax: NumericLiteralExpression, '1')
                      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      Operand: 
                        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid) (Syntax: NumericLiteralExpression, '1')
                    IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Object, IsInvalid, IsImplicit) (Syntax: NumericLiteralExpression, '1')
                      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      Operand: 
                        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid) (Syntax: NumericLiteralExpression, '1')
    IArrayInitializer (2 elements) ([1] OperationKind.ArrayInitializer) (Syntax: ArrayInitializerExpression, '{ 2, 2 }')
      Element Values(2):
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0623: Array initializers can only be used in a variable or field initializer. Try using a new expression instead.
                //         var x = new int[2, 2] /*<bind>*/{ { { 1, 1 } }, { 2, 2 } }/*</bind>*/;
                Diagnostic(ErrorCode.ERR_ArrayInitInBadPlace, "{ 1, 1 }").WithLocation(6, 45),
                // CS0847: An array initializer of length '2' is expected
                //         var x = new int[2, 2] /*<bind>*/{ { { 1, 1 } }, { 2, 2 } }/*</bind>*/;
                Diagnostic(ErrorCode.ERR_ArrayInitializerIncorrectLength, "{ { 1, 1 } }").WithArguments("2").WithLocation(6, 43)
            };

            VerifyOperationTreeAndDiagnosticsForTest<InitializerExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17598, "https://github.com/dotnet/roslyn/issues/17598")]
        public void InvalidArrayCreation()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        var x = /*<bind>*/new X[Program] { { 1 } }/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: X[], IsInvalid) (Syntax: ArrayCreationExpression, 'new X[Program] { { 1 } }') (Parent: VariableInitializer)
  Dimension Sizes(1):
    IInvalidExpression ([0] OperationKind.InvalidExpression, Type: Program, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Program')
      Children(1):
        IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: IdentifierName, 'Program')
  Initializer: 
    IArrayInitializer (1 elements) ([1] OperationKind.ArrayInitializer, IsInvalid) (Syntax: ArrayInitializerExpression, '{ { 1 } }')
      Element Values(1):
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: X, IsInvalid, IsImplicit) (Syntax: ArrayInitializerExpression, '{ 1 }')
          Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid, IsImplicit) (Syntax: ArrayInitializerExpression, '{ 1 }')
              Children(1):
                IArrayInitializer (1 elements) ([0] OperationKind.ArrayInitializer, IsInvalid) (Syntax: ArrayInitializerExpression, '{ 1 }')
                  Element Values(1):
                    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsInvalid, IsImplicit) (Syntax: NumericLiteralExpression, '1')
                      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      Operand: 
                        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid) (Syntax: NumericLiteralExpression, '1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0246: The type or namespace name 'X' could not be found (are you missing a using directive or an assembly reference?)
                //         var x = /*<bind>*/new X[Program] { { 1 } }/*</bind>*/;
                Diagnostic(ErrorCode.ERR_SingleTypeNameNotFound, "X").WithArguments("X").WithLocation(6, 31),
                // CS0119: 'Program' is a type, which is not valid in the given context
                //         var x = /*<bind>*/new X[Program] { { 1 } }/*</bind>*/;
                Diagnostic(ErrorCode.ERR_BadSKunknown, "Program").WithArguments("Program", "type").WithLocation(6, 33),
                // CS0623: Array initializers can only be used in a variable or field initializer. Try using a new expression instead.
                //         var x = /*<bind>*/new X[Program] { { 1 } }/*</bind>*/;
                Diagnostic(ErrorCode.ERR_ArrayInitInBadPlace, "{ 1 }").WithLocation(6, 44)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17598, "https://github.com/dotnet/roslyn/issues/17598")]
        public void InvalidParameterDefaultValueInitializer()
        {
            string source = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static int M() { return 0; }
    void F(int p /*<bind>*/= M()/*</bind>*/)
    {
    }
}
";
            string expectedOperationTree = @"
IParameterInitializer (Parameter: [System.Int32 p = default(System.Int32)]) ([Root] OperationKind.ParameterInitializer, IsInvalid) (Syntax: EqualsValueClause, '= M()') (Parent: )
  IInvocationExpression (System.Int32 Program.M()) ([0] OperationKind.InvocationExpression, Type: System.Int32, IsInvalid) (Syntax: InvocationExpression, 'M()')
    Instance Receiver: 
      null
    Arguments(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1736: Default parameter value for 'p' must be a compile-time constant
                //     void F(int p /*<bind>*/= M()/*</bind>*/)
                Diagnostic(ErrorCode.ERR_DefaultValueMustBeConstant, "M()").WithArguments("p").WithLocation(10, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<EqualsValueClauseSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }
    }
}
