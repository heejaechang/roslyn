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
        public void DelegateCreationExpression_ImplicitLambdaConversion()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        /*<bind>*/Action a = () => { };/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Action a = () => { };') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'a = () => { }')
    Variables: Local_1: System.Action a
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= () => { }')
        IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsImplicit) (Syntax: ParenthesizedLambdaExpression, '() => { }')
          Target: 
            IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null) (Syntax: ParenthesizedLambdaExpression, '() => { }')
              IBlockStatement (1 statements) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ }')
                IReturnStatement ([0] OperationKind.ReturnStatement, IsImplicit) (Syntax: Block, '{ }')
                  ReturnedValue: 
                    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ImplicitLambdaConversion_InitializerBindingReturnsJustAnonymousFunction()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/() => { }/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null) (Syntax: ParenthesizedLambdaExpression, '() => { }') (Parent: DelegateCreationExpression)
  IBlockStatement (1 statements) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ }')
    IReturnStatement ([0] OperationKind.ReturnStatement, IsImplicit) (Syntax: Block, '{ }')
      ReturnedValue: 
        null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LambdaExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ImplicitLambdaConversion_InvalidReturnType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        /*<bind>*/Action a = () => 1;/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Action a = () => 1;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'a = () => 1')
    Variables: Local_1: System.Action a
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= () => 1')
        IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsInvalid, IsImplicit) (Syntax: ParenthesizedLambdaExpression, '() => 1')
          Target: 
            IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null, IsInvalid) (Syntax: ParenthesizedLambdaExpression, '() => 1')
              IBlockStatement (2 statements) ([0] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: NumericLiteralExpression, '1')
                IExpressionStatement ([0] OperationKind.ExpressionStatement, IsInvalid, IsImplicit) (Syntax: NumericLiteralExpression, '1')
                  Expression: 
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid) (Syntax: NumericLiteralExpression, '1')
                IReturnStatement ([1] OperationKind.ReturnStatement, IsInvalid, IsImplicit) (Syntax: NumericLiteralExpression, '1')
                  ReturnedValue: 
                    null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0201: Only assignment, call, increment, decrement, and new object expressions can be used as a statement
                //         /*<bind>*/Action a = () => 1;/*</bind>*/
                Diagnostic(ErrorCode.ERR_IllegalStatement, "1").WithLocation(7, 36)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ImplicitLambdaConversion_InvalidArgumentType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        /*<bind>*/Action a = (int i) => { };/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Action a =  ...  i) => { };') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'a = (int i) => { }')
    Variables: Local_1: System.Action a
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= (int i) => { }')
        IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsInvalid, IsImplicit) (Syntax: ParenthesizedLambdaExpression, '(int i) => { }')
          Target: 
            IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null, IsInvalid) (Syntax: ParenthesizedLambdaExpression, '(int i) => { }')
              IBlockStatement (0 statements) ([0] OperationKind.BlockStatement, IsInvalid) (Syntax: Block, '{ }')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1593: Delegate 'Action' does not take 1 arguments
                //         /*<bind>*/Action a = (int i) => { };/*</bind>*/
                Diagnostic(ErrorCode.ERR_BadDelArgCount, "(int i) => { }").WithArguments("System.Action", "1").WithLocation(7, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitLambdaConversion()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/(Action)(() => { })/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action) (Syntax: CastExpression, '(Action)(() => { })') (Parent: VariableInitializer)
  Target: 
    IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null) (Syntax: ParenthesizedLambdaExpression, '() => { }')
      IBlockStatement (1 statements) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ }')
        IReturnStatement ([0] OperationKind.ReturnStatement, IsImplicit) (Syntax: Block, '{ }')
          ReturnedValue: 
            null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<CastExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitLambdaConversion_InvalidReturnType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/(Action)(() => 1)/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsInvalid) (Syntax: CastExpression, '(Action)(() => 1)') (Parent: VariableInitializer)
  Target: 
    IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null, IsInvalid) (Syntax: ParenthesizedLambdaExpression, '() => 1')
      IBlockStatement (2 statements) ([0] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: NumericLiteralExpression, '1')
        IExpressionStatement ([0] OperationKind.ExpressionStatement, IsInvalid, IsImplicit) (Syntax: NumericLiteralExpression, '1')
          Expression: 
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid) (Syntax: NumericLiteralExpression, '1')
        IReturnStatement ([1] OperationKind.ReturnStatement, IsInvalid, IsImplicit) (Syntax: NumericLiteralExpression, '1')
          ReturnedValue: 
            null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0201: Only assignment, call, increment, decrement, and new object expressions can be used as a statement
                //         Action a = /*<bind>*/(Action)(() => 1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_IllegalStatement, "1").WithLocation(7, 45)
            };

            VerifyOperationTreeAndDiagnosticsForTest<CastExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitLambdaConversion_InvalidArgumentType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/(Action)((int i) => { })/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsInvalid) (Syntax: CastExpression, '(Action)((int i) => { })') (Parent: VariableInitializer)
  Target: 
    IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null, IsInvalid) (Syntax: ParenthesizedLambdaExpression, '(int i) => { }')
      IBlockStatement (0 statements) ([0] OperationKind.BlockStatement, IsInvalid) (Syntax: Block, '{ }')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1593: Delegate 'Action' does not take 1 arguments
                //         Action a = /*<bind>*/(Action)((int i) => { })/*</bind>*/;
                Diagnostic(ErrorCode.ERR_BadDelArgCount, "(int i) => { }").WithArguments("System.Action", "1").WithLocation(7, 39)
            };

            VerifyOperationTreeAndDiagnosticsForTest<CastExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_DelegateExpression()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        /*<bind>*/Action a = delegate() { };/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Action a =  ... gate() { };') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'a = delegate() { }')
    Variables: Local_1: System.Action a
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= delegate() { }')
        IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsImplicit) (Syntax: AnonymousMethodExpression, 'delegate() { }')
          Target: 
            IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null) (Syntax: AnonymousMethodExpression, 'delegate() { }')
              IBlockStatement (1 statements) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ }')
                IReturnStatement ([0] OperationKind.ReturnStatement, IsImplicit) (Syntax: Block, '{ }')
                  ReturnedValue: 
                    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_DelegateExpression_InvalidReturnType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        /*<bind>*/Action a = delegate() { return 1; };/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Action a =  ... eturn 1; };') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'a = delegat ... return 1; }')
    Variables: Local_1: System.Action a
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= delegate( ... return 1; }')
        IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsInvalid, IsImplicit) (Syntax: AnonymousMethodExpression, 'delegate() { return 1; }')
          Target: 
            IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null, IsInvalid) (Syntax: AnonymousMethodExpression, 'delegate() { return 1; }')
              IBlockStatement (1 statements) ([0] OperationKind.BlockStatement, IsInvalid) (Syntax: Block, '{ return 1; }')
                IReturnStatement ([0] OperationKind.ReturnStatement, IsInvalid) (Syntax: ReturnStatement, 'return 1;')
                  ReturnedValue: 
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS8030: Anonymous function converted to a void returning delegate cannot return a value
                //         /*<bind>*/Action a = delegate() { return 1; };/*</bind>*/
                Diagnostic(ErrorCode.ERR_RetNoObjectRequiredLambda, "return").WithLocation(7, 43)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_DelegateExpression_InvalidArgumentType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        /*<bind>*/Action a = delegate(int i) { };/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Action a =  ... int i) { };') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'a = delegate(int i) { }')
    Variables: Local_1: System.Action a
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= delegate(int i) { }')
        IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsInvalid, IsImplicit) (Syntax: AnonymousMethodExpression, 'delegate(int i) { }')
          Target: 
            IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null, IsInvalid) (Syntax: AnonymousMethodExpression, 'delegate(int i) { }')
              IBlockStatement (0 statements) ([0] OperationKind.BlockStatement, IsInvalid) (Syntax: Block, '{ }')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1593: Delegate 'Action' does not take 1 arguments
                //         /*<bind>*/Action a = delegate(int i) { };/*</bind>*/
                Diagnostic(ErrorCode.ERR_BadDelArgCount, "delegate(int i) { }").WithArguments("System.Action", "1").WithLocation(7, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ImplicitMethodBinding()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        /*<bind>*/Action a = M1;/*</bind>*/
    }
    void M1() { }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Action a = M1;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'a = M1')
    Variables: Local_1: System.Action a
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= M1')
        IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsImplicit) (Syntax: IdentifierName, 'M1')
          Target: 
            IMethodReferenceExpression: void Program.M1() ([0] OperationKind.MethodReferenceExpression, Type: null) (Syntax: IdentifierName, 'M1')
              Instance Receiver: 
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Program, IsImplicit) (Syntax: IdentifierName, 'M1')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        [WorkItem(15513, "https://github.com/dotnet/roslyn/issues/15513")]
        public void DelegateCreationExpression_ImplicitMethodBinding_InitializerBindingReturnsJustMethodReference()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/M1/*</bind>*/;
    }
    static void M1() { }
}
";

            string expectedOperationTree = @"
IMethodReferenceExpression: void Program.M1() (Static) ([0] OperationKind.MethodReferenceExpression, Type: null) (Syntax: IdentifierName, 'M1') (Parent: DelegateCreationExpression)
  Instance Receiver: 
    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<IdentifierNameSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ImplicitMethodBinding_InvalidIdentifier()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        /*<bind>*/Action a = M1;/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Action a = M1;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'a = M1')
    Variables: Local_1: System.Action a
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= M1')
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Action, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M1')
          Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: IdentifierName, 'M1')
              Children(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0103: The name 'M1' does not exist in the current context
                //         /*<bind>*/Action a = M1;/*</bind>*/
                Diagnostic(ErrorCode.ERR_NameNotInContext, "M1").WithArguments("M1").WithLocation(7, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ImplicitMethodBinding_InvalidIdentifier_InitializerBindingReturnsJustInvalidExpression()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/M1/*</bind>*/;
    }
}
";

            string expectedOperationTree = @"
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: IdentifierName, 'M1') (Parent: ConversionExpression)
  Children(0)
";
            var expectedDiagnostics = new DiagnosticDescription[]
            {
                // CS0103: The name 'M1' does not exist in the current context
                //         Action a = /*<bind>*/M1/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NameNotInContext, "M1").WithArguments("M1").WithLocation(7, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<IdentifierNameSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ImplicitMethodBinding_InvalidReturnType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        /*<bind>*/Action a = M1;/*</bind>*/
    }
    int M1() => 1;
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Action a = M1;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'a = M1')
    Variables: Local_1: System.Action a
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= M1')
        IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M1')
          Target: 
            IMethodReferenceExpression: System.Int32 Program.M1() ([0] OperationKind.MethodReferenceExpression, Type: null, IsInvalid) (Syntax: IdentifierName, 'M1')
              Instance Receiver: 
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Program, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0407: 'int Program.M1()' has the wrong return type
                //         /*<bind>*/Action a = M1;/*</bind>*/
                Diagnostic(ErrorCode.ERR_BadRetType, "M1").WithArguments("Program.M1()", "int").WithLocation(7, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ImplicitMethodBinding_InvalidReturnType_InitializerBindingReturnsJustMethodReference()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/M1/*</bind>*/;
    }
    int M1() => 1;
}
";

            string expectedOperationTree = @"
IMethodReferenceExpression: System.Int32 Program.M1() ([0] OperationKind.MethodReferenceExpression, Type: null, IsInvalid) (Syntax: IdentifierName, 'M1') (Parent: DelegateCreationExpression)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Program, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[]
            {
                // CS0407: 'int Program.M1()' has the wrong return type
                //         Action a = /*<bind>*/M1/*</bind>*/;
                Diagnostic(ErrorCode.ERR_BadRetType, "M1").WithArguments("Program.M1()", "int").WithLocation(7, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<IdentifierNameSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ImplicitMethodBinding_InvalidArgumentType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        /*<bind>*/Action a = M1;/*</bind>*/
    }
    void M1(object o) { }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Action a = M1;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'a = M1')
    Variables: Local_1: System.Action a
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= M1')
        IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M1')
          Target: 
            IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: IdentifierName, 'M1')
              Children(1):
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Program, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0123: No overload for 'M1' matches delegate 'Action'
                //         /*<bind>*/Action a = M1;/*</bind>*/
                Diagnostic(ErrorCode.ERR_MethDelegateMismatch, "M1").WithArguments("M1", "System.Action").WithLocation(7, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ImplicitMethodBinding_InvalidArgumentType_InitializerBindingReturnsJustNoneOperation()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/M1/*</bind>*/;
    }
    void M1(object o) { }
}
";

            string expectedOperationTree = @"
IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: IdentifierName, 'M1') (Parent: DelegateCreationExpression)
  Children(1):
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Program, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[]
            {
                // CS0123: No overload for 'M1' matches delegate 'Action'
                //         Action a = /*<bind>*/M1/*</bind>*/;
                Diagnostic(ErrorCode.ERR_MethDelegateMismatch, "M1").WithArguments("M1", "System.Action").WithLocation(7, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<IdentifierNameSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitMethodBinding()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/(Action)M1/*</bind>*/;
    }
    void M1() { }
}
";
            string expectedOperationTree = @"
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action) (Syntax: CastExpression, '(Action)M1') (Parent: VariableInitializer)
  Target: 
    IMethodReferenceExpression: void Program.M1() ([0] OperationKind.MethodReferenceExpression, Type: null) (Syntax: IdentifierName, 'M1')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Program, IsImplicit) (Syntax: IdentifierName, 'M1')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<CastExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitMethodBinding_InvalidIdentifier()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/(Action)M1/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IConversionExpression (Explicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Action, IsInvalid) (Syntax: CastExpression, '(Action)M1') (Parent: VariableInitializer)
  Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  Operand: 
    IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: IdentifierName, 'M1')
      Children(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0103: The name 'M1' does not exist in the current context
                //         Action a = /*<bind>*/(Action)M1/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NameNotInContext, "M1").WithArguments("M1").WithLocation(7, 38)
            };

            VerifyOperationTreeAndDiagnosticsForTest<CastExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact]
        public void DelegateCreationExpression_ExplicitMethodBinding_InvalidIdentifierWithReceiver()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        object o = new object();
        Action a = /*<bind>*/(Action)o.M1/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IConversionExpression (Explicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Action, IsInvalid) (Syntax: CastExpression, '(Action)o.M1') (Parent: VariableInitializer)
  Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  Operand: 
    IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: SimpleMemberAccessExpression, 'o.M1')
      Children(1):
        ILocalReferenceExpression: o ([0] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'o')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1061: 'object' does not contain a definition for 'M1' and no extension method 'M1' accepting a first argument of type 'object' could be found (are you missing a using directive or an assembly reference?)
                //         Action a = /*<bind>*/(Action)o.M1/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NoSuchMemberOrExtension, "M1").WithArguments("object", "M1").WithLocation(8, 40)
            };

            VerifyOperationTreeAndDiagnosticsForTest<CastExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitMethodBinding_InvalidReturnType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/(Action)M1/*</bind>*/;
    }
    int M1() => 1;
}
";
            string expectedOperationTree = @"
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsInvalid) (Syntax: CastExpression, '(Action)M1') (Parent: VariableInitializer)
  Target: 
    IMethodReferenceExpression: System.Int32 Program.M1() ([0] OperationKind.MethodReferenceExpression, Type: null, IsInvalid) (Syntax: IdentifierName, 'M1')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Program, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[]
            {
                // CS0407: 'int Program.M1()' has the wrong return type
                //         Action a = /*<bind>*/(Action)M1/*</bind>*/;
                Diagnostic(ErrorCode.ERR_BadRetType, "(Action)M1").WithArguments("Program.M1()", "int").WithLocation(7, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<CastExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact]
        public void DelegateCreationExpression_ExplicitMethodBinding_InvalidReturnTypeWithReceiver()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Program p = new Program();
        Action a = /*<bind>*/(Action)p.M1/*</bind>*/;
    }
    int M1() => 1;
}
";
            string expectedOperationTree = @"
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsInvalid) (Syntax: CastExpression, '(Action)p.M1') (Parent: VariableInitializer)
  Target: 
    IMethodReferenceExpression: System.Int32 Program.M1() ([0] OperationKind.MethodReferenceExpression, Type: null, IsInvalid) (Syntax: SimpleMemberAccessExpression, 'p.M1')
      Instance Receiver: 
        ILocalReferenceExpression: p ([0] OperationKind.LocalReferenceExpression, Type: Program, IsInvalid) (Syntax: IdentifierName, 'p')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0407: 'int Program.M1()' has the wrong return type
                //         Action a = /*<bind>*/(Action)p.M1/*</bind>*/;
                Diagnostic(ErrorCode.ERR_BadRetType, "(Action)p.M1").WithArguments("Program.M1()", "int").WithLocation(8, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<CastExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitMethodBinding_InvalidArgumentType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/(Action)M1/*</bind>*/;
    }
    void M1(object o) { }
}
";
            string expectedOperationTree = @"
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsInvalid) (Syntax: CastExpression, '(Action)M1') (Parent: VariableInitializer)
  Target: 
    IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: IdentifierName, 'M1')
      Children(1):
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Program, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[]
            {
                // CS0030: Cannot convert type 'method' to 'Action'
                //         Action a = /*<bind>*/(Action)M1/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NoExplicitConv, "(Action)M1").WithArguments("method", "System.Action").WithLocation(7, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<CastExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitMethodBinding_InvalidArgumentTypeWithReceiver()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Program p = new Program();
        Action a = /*<bind>*/(Action)p.M1/*</bind>*/;
    }
    void M1(object o) { }
}
";
            string expectedOperationTree = @"
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsInvalid) (Syntax: CastExpression, '(Action)p.M1') (Parent: VariableInitializer)
  Target: 
    IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: SimpleMemberAccessExpression, 'p.M1')
      Children(1):
        ILocalReferenceExpression: p ([0] OperationKind.LocalReferenceExpression, Type: Program, IsInvalid) (Syntax: IdentifierName, 'p')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0030: Cannot convert type 'method' to 'Action'
                //         Action a = /*<bind>*/(Action)p.M1/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NoExplicitConv, "(Action)p.M1").WithArguments("method", "System.Action").WithLocation(8, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<CastExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndImplicitLambdaConversion()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action(() => { })/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action) (Syntax: ObjectCreationExpression, 'new Action(() => { })') (Parent: VariableInitializer)
  Target: 
    IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null) (Syntax: ParenthesizedLambdaExpression, '() => { }')
      IBlockStatement (1 statements) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ }')
        IReturnStatement ([0] OperationKind.ReturnStatement, IsImplicit) (Syntax: Block, '{ }')
          ReturnedValue: 
            null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndImplicitLambdaConversion_InvalidReturnType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action(() => 1)/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsInvalid) (Syntax: ObjectCreationExpression, 'new Action(() => 1)') (Parent: VariableInitializer)
  Target: 
    IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null, IsInvalid) (Syntax: ParenthesizedLambdaExpression, '() => 1')
      IBlockStatement (2 statements) ([0] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: NumericLiteralExpression, '1')
        IExpressionStatement ([0] OperationKind.ExpressionStatement, IsInvalid, IsImplicit) (Syntax: NumericLiteralExpression, '1')
          Expression: 
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid) (Syntax: NumericLiteralExpression, '1')
        IReturnStatement ([1] OperationKind.ReturnStatement, IsInvalid, IsImplicit) (Syntax: NumericLiteralExpression, '1')
          ReturnedValue: 
            null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0201: Only assignment, call, increment, decrement, and new object expressions can be used as a statement
                //         Action a = /*<bind>*/new Action(() => 1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_IllegalStatement, "1").WithLocation(7, 47)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndImplicitLambdaConversion_InvalidArgumentType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action((int i) => { })/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsInvalid) (Syntax: ObjectCreationExpression, 'new Action( ...  i) => { })') (Parent: VariableInitializer)
  Target: 
    IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null, IsInvalid) (Syntax: ParenthesizedLambdaExpression, '(int i) => { }')
      IBlockStatement (1 statements) ([0] OperationKind.BlockStatement, IsInvalid) (Syntax: Block, '{ }')
        IReturnStatement ([0] OperationKind.ReturnStatement, IsInvalid, IsImplicit) (Syntax: Block, '{ }')
          ReturnedValue: 
            null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1593: Delegate 'Action' does not take 1 arguments
                //         Action a = /*<bind>*/new Action((int i) => { })/*</bind>*/;
                Diagnostic(ErrorCode.ERR_BadDelArgCount, "(int i) => { }").WithArguments("System.Action", "1").WithLocation(7, 41)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndImplicitLambdaConversion_InvalidMultipleParameters()
        {
            string source = @"
using System;

class C
{
    void M1()
    {
        Action action = /*<bind>*/new Action((o) => { }, new object())/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Action, IsInvalid) (Syntax: ObjectCreationExpression, 'new Action( ... w object())') (Parent: VariableInitializer)
  Children(2):
    IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null, IsInvalid) (Syntax: ParenthesizedLambdaExpression, '(o) => { }')
      IBlockStatement (0 statements) ([0] OperationKind.BlockStatement, IsInvalid) (Syntax: Block, '{ }')
    IObjectCreationExpression (Constructor: System.Object..ctor()) ([1] OperationKind.ObjectCreationExpression, Type: System.Object, IsInvalid) (Syntax: ObjectCreationExpression, 'new object()')
      Arguments(0)
      Initializer: 
        null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0149: Method name expected
                //         Action action = /*<bind>*/new Action((o) => { }, new object())/*</bind>*/;
                Diagnostic(ErrorCode.ERR_MethodNameExpected, "(o) => { }, new object()").WithLocation(8, 46)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndImplicitMethodBindingConversion()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action(M1)/*</bind>*/;
    }

    void M1()
    { }
}
";
            string expectedOperationTree = @"
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action) (Syntax: ObjectCreationExpression, 'new Action(M1)') (Parent: VariableInitializer)
  Target: 
    IMethodReferenceExpression: void Program.M1() ([0] OperationKind.MethodReferenceExpression, Type: null) (Syntax: IdentifierName, 'M1')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Program, IsImplicit) (Syntax: IdentifierName, 'M1')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        [WorkItem(15513, "https://github.com/dotnet/roslyn/issues/15513")]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndImplicitStaticMethodBindingConversion_01()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action(M1)/*</bind>*/;
    }

    static void M1()
    { }
}
";
            string expectedOperationTree = @"
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action) (Syntax: ObjectCreationExpression, 'new Action(M1)') (Parent: VariableInitializer)
  Target: 
    IMethodReferenceExpression: void Program.M1() (Static) ([0] OperationKind.MethodReferenceExpression, Type: null) (Syntax: IdentifierName, 'M1')
      Instance Receiver: 
        null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        [WorkItem(15513, "https://github.com/dotnet/roslyn/issues/15513")]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndImplicitStaticMethodBindingConversion_02()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action(this.M1)/*</bind>*/;
    }

    static void M1()
    { }
}
";
            string expectedOperationTree = @"
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsInvalid) (Syntax: ObjectCreationExpression, 'new Action(this.M1)') (Parent: VariableInitializer)
  Target: 
    IMethodReferenceExpression: void Program.M1() (Static) ([0] OperationKind.MethodReferenceExpression, Type: null, IsInvalid) (Syntax: SimpleMemberAccessExpression, 'this.M1')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Program, IsInvalid) (Syntax: ThisExpression, 'this')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // (7,41): error CS0176: Member 'Program.M1()' cannot be accessed with an instance reference; qualify it with a type name instead
                //         Action a = /*<bind>*/new Action(this.M1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_ObjectProhibited, "this.M1").WithArguments("Program.M1()").WithLocation(7, 41)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndImplicitMethodBindingConversionWithReceiver()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        var p = new Program();
        Action a = /*<bind>*/new Action(p.M1)/*</bind>*/;
    }

    void M1() { }
}
";
            string expectedOperationTree = @"
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action) (Syntax: ObjectCreationExpression, 'new Action(p.M1)') (Parent: VariableInitializer)
  Target: 
    IMethodReferenceExpression: void Program.M1() ([0] OperationKind.MethodReferenceExpression, Type: null) (Syntax: SimpleMemberAccessExpression, 'p.M1')
      Instance Receiver: 
        ILocalReferenceExpression: p ([0] OperationKind.LocalReferenceExpression, Type: Program) (Syntax: IdentifierName, 'p')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndImplicitMethodBindingConversion_InvalidMissingIdentifier()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action(M1)/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Action, IsInvalid) (Syntax: ObjectCreationExpression, 'new Action(M1)') (Parent: VariableInitializer)
  Children(1):
    IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: IdentifierName, 'M1')
      Children(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0103: The name 'M1' does not exist in the current context
                //         Action a = /*<bind>*/new Action(M1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NameNotInContext, "M1").WithArguments("M1").WithLocation(7, 41)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndImplicitMethodBindingConversion_InvalidReturnType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action(M1)/*</bind>*/;
    }

    int M1() => 1;
}
";
            string expectedOperationTree = @"
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsInvalid) (Syntax: ObjectCreationExpression, 'new Action(M1)') (Parent: VariableInitializer)
  Target: 
    IMethodReferenceExpression: System.Int32 Program.M1() ([0] OperationKind.MethodReferenceExpression, Type: null, IsInvalid) (Syntax: IdentifierName, 'M1')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Program, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0407: 'int Program.M1()' has the wrong return type
                //         Action a = /*<bind>*/new Action(M1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_BadRetType, "M1").WithArguments("Program.M1()", "int").WithLocation(7, 41)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndImplicitMethodBindingConversion_InvalidArgumentType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action(M1)/*</bind>*/;
    }

    void M1(object o)
    { }
}
";
            string expectedOperationTree = @"
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsInvalid) (Syntax: ObjectCreationExpression, 'new Action(M1)') (Parent: VariableInitializer)
  Target: 
    IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: IdentifierName, 'M1')
      Children(1):
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Program, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0123: No overload for 'M1' matches delegate 'Action'
                //         Action a = /*<bind>*/new Action(M1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_MethDelegateMismatch, "new Action(M1)").WithArguments("M1", "System.Action").WithLocation(7, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateCreation_InvalidMultipleParameters()
        {
            string source = @"
using System;

class C
{
    void M1()
    {
        Action action = /*<bind>*/new Action(M2, M3)/*</bind>*/;
    }

    void M2()
    {
    }
    void M3()
    {
    }
}
";
            string expectedOperationTree = @"
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Action, IsInvalid) (Syntax: ObjectCreationExpression, 'new Action(M2, M3)') (Parent: VariableInitializer)
  Children(2):
    IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: IdentifierName, 'M2')
      Children(1):
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M2')
    IOperation:  ([1] OperationKind.None, IsInvalid) (Syntax: IdentifierName, 'M3')
      Children(1):
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M3')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0149: Method name expected
                //         Action action = /*<bind>*/new Action(M2, M3)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_MethodNameExpected, "M2, M3").WithLocation(8, 46)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorImplicitMethodBinding_InvalidTargetArguments()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action<string> a = /*<bind>*/new Action(M1)/*</bind>*/;
    }

    void M1() { }
}
";
            string expectedOperationTree = @"
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsInvalid) (Syntax: ObjectCreationExpression, 'new Action(M1)') (Parent: ConversionExpression)
  Target: 
    IMethodReferenceExpression: void Program.M1() ([0] OperationKind.MethodReferenceExpression, Type: null, IsInvalid) (Syntax: IdentifierName, 'M1')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Program, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0029: Cannot implicitly convert type 'System.Action' to 'System.Action<string>'
                //         Action<string> a = /*<bind>*/new Action(M1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NoImplicitConv, "new Action(M1)").WithArguments("System.Action", "System.Action<string>").WithLocation(7, 38)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorImplicitMethodBinding_InvalidTargetReturn()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Func<string> a = /*<bind>*/new Action(M1)/*</bind>*/;
    }

    void M1() { }
}
";
            string expectedOperationTree = @"
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsInvalid) (Syntax: ObjectCreationExpression, 'new Action(M1)') (Parent: ConversionExpression)
  Target: 
    IMethodReferenceExpression: void Program.M1() ([0] OperationKind.MethodReferenceExpression, Type: null, IsInvalid) (Syntax: IdentifierName, 'M1')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Program, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0029: Cannot implicitly convert type 'System.Action' to 'System.Func<string>'
                //         Func<string> a = /*<bind>*/new Action(M1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NoImplicitConv, "new Action(M1)").WithArguments("System.Action", "System.Func<string>").WithLocation(7, 36)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndExplicitLambdaConversion()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action((Action)(() => { }))/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action) (Syntax: ObjectCreationExpression, 'new Action( ... () => { }))') (Parent: VariableInitializer)
  Target: 
    IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action) (Syntax: CastExpression, '(Action)(() => { })')
      Target: 
        IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null) (Syntax: ParenthesizedLambdaExpression, '() => { }')
          IBlockStatement (1 statements) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ }')
            IReturnStatement ([0] OperationKind.ReturnStatement, IsImplicit) (Syntax: Block, '{ }')
              ReturnedValue: 
                null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndExplicitLambdaConversion_InvalidReturnType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action((Action)(() => 1))/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Action, IsInvalid) (Syntax: ObjectCreationExpression, 'new Action( ... )(() => 1))') (Parent: VariableInitializer)
  Children(1):
    IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsInvalid) (Syntax: CastExpression, '(Action)(() => 1)')
      Target: 
        IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null, IsInvalid) (Syntax: ParenthesizedLambdaExpression, '() => 1')
          IBlockStatement (2 statements) ([0] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: NumericLiteralExpression, '1')
            IExpressionStatement ([0] OperationKind.ExpressionStatement, IsInvalid, IsImplicit) (Syntax: NumericLiteralExpression, '1')
              Expression: 
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid) (Syntax: NumericLiteralExpression, '1')
            IReturnStatement ([1] OperationKind.ReturnStatement, IsInvalid, IsImplicit) (Syntax: NumericLiteralExpression, '1')
              ReturnedValue: 
                null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0201: Only assignment, call, increment, decrement, and new object expressions can be used as a statement
                //         Action a = /*<bind>*/new Action((Action)(() => 1))/*</bind>*/;
                Diagnostic(ErrorCode.ERR_IllegalStatement, "1").WithLocation(7, 56)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndExplicitLambdaConversion_InvalidArgumentType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action((Action)((int i) => { }))/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Action, IsInvalid) (Syntax: ObjectCreationExpression, 'new Action( ... i) => { }))') (Parent: VariableInitializer)
  Children(1):
    IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsInvalid) (Syntax: CastExpression, '(Action)((int i) => { })')
      Target: 
        IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null, IsInvalid) (Syntax: ParenthesizedLambdaExpression, '(int i) => { }')
          IBlockStatement (0 statements) ([0] OperationKind.BlockStatement, IsInvalid) (Syntax: Block, '{ }')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1593: Delegate 'Action' does not take 1 arguments
                //         Action a = /*<bind>*/new Action((Action)((int i) => { }))/*</bind>*/;
                Diagnostic(ErrorCode.ERR_BadDelArgCount, "(int i) => { }").WithArguments("System.Action", "1").WithLocation(7, 50)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndExplicitMethodBindingConversion()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action((Action)M1)/*</bind>*/;
    }
    void M1() {}
}
";
            string expectedOperationTree = @"
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action) (Syntax: ObjectCreationExpression, 'new Action((Action)M1)') (Parent: VariableInitializer)
  Target: 
    IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action) (Syntax: CastExpression, '(Action)M1')
      Target: 
        IMethodReferenceExpression: void Program.M1() ([0] OperationKind.MethodReferenceExpression, Type: null) (Syntax: IdentifierName, 'M1')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Program, IsImplicit) (Syntax: IdentifierName, 'M1')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndExplicitMethodBindingConversion_InvalidMissingIdentifier()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action((Action)M1)/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Action, IsInvalid) (Syntax: ObjectCreationExpression, 'new Action((Action)M1)') (Parent: VariableInitializer)
  Children(1):
    IConversionExpression (Explicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Action, IsInvalid) (Syntax: CastExpression, '(Action)M1')
      Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: IdentifierName, 'M1')
          Children(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0103: The name 'M1' does not exist in the current context
                //         Action a = /*<bind>*/new Action((Action)M1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NameNotInContext, "M1").WithArguments("M1").WithLocation(7, 49)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndExplicitMethodBindingConversion_InvalidReturnType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action((Action)M1)/*</bind>*/;
    }
    int M1() => 1;
}
";
            string expectedOperationTree = @"
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Action, IsInvalid) (Syntax: ObjectCreationExpression, 'new Action((Action)M1)') (Parent: VariableInitializer)
  Children(1):
    IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsInvalid) (Syntax: CastExpression, '(Action)M1')
      Target: 
        IMethodReferenceExpression: System.Int32 Program.M1() ([0] OperationKind.MethodReferenceExpression, Type: null, IsInvalid) (Syntax: IdentifierName, 'M1')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Program, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0407: 'int Program.M1()' has the wrong return type
                //         Action a = /*<bind>*/new Action((Action)M1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_BadRetType, "(Action)M1").WithArguments("Program.M1()", "int").WithLocation(7, 41)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndExplicitMethodBindingConversion_InvalidArgumentType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action((Action)M1)/*</bind>*/;
    }
    void M1(int i) { }
}
";
            string expectedOperationTree = @"
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Action, IsInvalid) (Syntax: ObjectCreationExpression, 'new Action((Action)M1)') (Parent: VariableInitializer)
  Children(1):
    IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsInvalid) (Syntax: CastExpression, '(Action)M1')
      Target: 
        IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: IdentifierName, 'M1')
          Children(1):
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Program, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0030: Cannot convert type 'method' to 'Action'
                //         Action a = /*<bind>*/new Action((Action)M1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NoExplicitConv, "(Action)M1").WithArguments("method", "System.Action").WithLocation(7, 41)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndExplicitMethodBindingConversion_InvalidTargetArgument()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action<string> a = /*<bind>*/new Action((Action)M1)/*</bind>*/;
    }

    void M1() { }
}
";
            string expectedOperationTree = @"
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsInvalid) (Syntax: ObjectCreationExpression, 'new Action((Action)M1)') (Parent: ConversionExpression)
  Target: 
    IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsInvalid) (Syntax: CastExpression, '(Action)M1')
      Target: 
        IMethodReferenceExpression: void Program.M1() ([0] OperationKind.MethodReferenceExpression, Type: null, IsInvalid) (Syntax: IdentifierName, 'M1')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Program, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0029: Cannot implicitly convert type 'System.Action' to 'System.Action<string>'
                //         Action<string> a = /*<bind>*/new Action((Action)M1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NoImplicitConv, "new Action((Action)M1)").WithArguments("System.Action", "System.Action<string>").WithLocation(7, 38)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndExplicitMethodBindingConversion_InvalidTargetReturn()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Func<string> a = /*<bind>*/new Action((Action)M1)/*</bind>*/;
    }

    void M1() { }
}
";
            string expectedOperationTree = @"
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsInvalid) (Syntax: ObjectCreationExpression, 'new Action((Action)M1)') (Parent: ConversionExpression)
  Target: 
    IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsInvalid) (Syntax: CastExpression, '(Action)M1')
      Target: 
        IMethodReferenceExpression: void Program.M1() ([0] OperationKind.MethodReferenceExpression, Type: null, IsInvalid) (Syntax: IdentifierName, 'M1')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Program, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0029: Cannot implicitly convert type 'System.Action' to 'System.Func<string>'
                //         Func<string> a = /*<bind>*/new Action((Action)M1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NoImplicitConv, "new Action((Action)M1)").WithArguments("System.Action", "System.Func<string>").WithLocation(7, 36)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndExplicitMethodBindingConversion_InvalidConstructorArgument()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action<int> a = /*<bind>*/new Action<int>((Action)M1)/*</bind>*/;
    }

    void M1() { }
}
";
            string expectedOperationTree = @"
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Action<System.Int32>, IsInvalid) (Syntax: ObjectCreationExpression, 'new Action< ... (Action)M1)') (Parent: VariableInitializer)
  Children(1):
    IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action, IsInvalid) (Syntax: CastExpression, '(Action)M1')
      Target: 
        IMethodReferenceExpression: void Program.M1() ([0] OperationKind.MethodReferenceExpression, Type: null, IsInvalid) (Syntax: IdentifierName, 'M1')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Program, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0123: No overload for 'Action' matches delegate 'Action<int>'
                //         Action<int> a = /*<bind>*/new Action<int>((Action)M1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_MethDelegateMismatch, "new Action<int>((Action)M1)").WithArguments("Action", "System.Action<int>").WithLocation(7, 35)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void ConversionExpression_Implicit_ReferenceLambdaToDelegateConversion_InvalidSyntax()
        {
            string source = @"
class Program
{
    delegate void DType();
    void Main()
    {
        DType /*<bind>*/d1 = () =>/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'd1 = () =>/*</bind>*/') (Parent: VariableDeclarationStatement)
  Variables: Local_1: Program.DType d1
  Initializer: 
    IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= () =>/*</bind>*/')
      IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: Program.DType, IsInvalid, IsImplicit) (Syntax: ParenthesizedLambdaExpression, '() =>/*</bind>*/')
        Target: 
          IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null, IsInvalid) (Syntax: ParenthesizedLambdaExpression, '() =>/*</bind>*/')
            IBlockStatement (2 statements) ([0] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: IdentifierName, '')
              IExpressionStatement ([0] OperationKind.ExpressionStatement, IsInvalid, IsImplicit) (Syntax: IdentifierName, '')
                Expression: 
                  IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
                    Children(0)
              IReturnStatement ([1] OperationKind.ReturnStatement, IsInvalid, IsImplicit) (Syntax: IdentifierName, '')
                ReturnedValue: 
                  null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1525: Invalid expression term ';'
                //         DType /*<bind>*/d1 = () =>/*</bind>*/;
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, ";").WithArguments(";").WithLocation(7, 46)
            };

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclaratorSyntax>(source, expectedOperationTree, expectedDiagnostics,
                additionalOperationTreeVerifier: new ExpectedSymbolVerifier().Verify);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ImplicitMethodBinding_MultipleCandidates_InvalidNoMatch()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        /*<bind>*/Action<int> a = M1;/*</bind>*/
    }
    void M1(Program o) { }
    void M1(string s) { }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Action<int> a = M1;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'a = M1')
    Variables: Local_1: System.Action<System.Int32> a
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= M1')
        IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action<System.Int32>, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M1')
          Target: 
            IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: IdentifierName, 'M1')
              Children(1):
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Program, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0123: No overload for 'Program.M1(object)' matches delegate 'Action<int>'
                //         /*<bind>*/Action<int> a = M1;/*</bind>*/
                Diagnostic(ErrorCode.ERR_MethDelegateMismatch, "M1").WithArguments("M1", "System.Action<int>").WithLocation(7, 35)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ImplicitMethodBinding_MultipleCandidates()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        /*<bind>*/Action<int> a = M1;/*</bind>*/
    }
    void M1(object o) { }
    void M1(int i) { }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Action<int> a = M1;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'a = M1')
    Variables: Local_1: System.Action<System.Int32> a
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= M1')
        IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action<System.Int32>, IsImplicit) (Syntax: IdentifierName, 'M1')
          Target: 
            IMethodReferenceExpression: void Program.M1(System.Int32 i) ([0] OperationKind.MethodReferenceExpression, Type: null) (Syntax: IdentifierName, 'M1')
              Instance Receiver: 
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Program, IsImplicit) (Syntax: IdentifierName, 'M1')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorImplicitMethodBinding_MultipleCandidates()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action<string> a = /*<bind>*/new Action<string>(M1)/*</bind>*/;
    }

    void M1(object o) { }

    void M1(string s) { }
}
";
            string expectedOperationTree = @"
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action<System.String>) (Syntax: ObjectCreationExpression, 'new Action<string>(M1)') (Parent: VariableInitializer)
  Target: 
    IMethodReferenceExpression: void Program.M1(System.String s) ([0] OperationKind.MethodReferenceExpression, Type: null) (Syntax: IdentifierName, 'M1')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Program, IsImplicit) (Syntax: IdentifierName, 'M1')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorImplicitMethodBinding_MultipleCandidates_InvalidNoMatch()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action<int> a = /*<bind>*/new Action<int>(M1)/*</bind>*/;
    }

    void M1(Program o) { }

    void M1(string s) { }
}
";
            string expectedOperationTree = @"
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Action<System.Int32>, IsInvalid) (Syntax: ObjectCreationExpression, 'new Action<int>(M1)') (Parent: VariableInitializer)
  Target: 
    IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: IdentifierName, 'M1')
      Children(1):
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Program, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0123: No overload for 'Program.M1(object)' matches delegate 'Action<int>'
                //         Action<int> a = /*<bind>*/new Action<int>(M1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_MethDelegateMismatch, "new Action<int>(M1)").WithArguments("M1", "System.Action<int>").WithLocation(7, 35)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }
    }
}
