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
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_ForSimpleLoop()
        {
            string source = @"
class C
{
    static void Main()
    {
        int x = 3;
        /*<bind>*/for (int i = 0; i < 3; i = i + 1)
        {
            x = x * 3;
        }/*</bind>*/
        System.Console.Write(""{0}"", x);
    }
}
";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([1] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (int i  ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 i
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < 3')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3) (Syntax: NumericLiteralExpression, '3')
  Before:
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int i = 0')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i = 0')
        Variables: Local_1: System.Int32 i
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  AtLoopBottom:
    IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
          Left: 
            ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
          Right: 
            IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'i + 1')
              Left: 
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'x = x * 3;')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'x = x * 3')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
            Right: 
              IBinaryOperatorExpression (BinaryOperatorKind.Multiply) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: MultiplyExpression, 'x * 3')
                Left: 
                  ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                Right: 
                  ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3) (Syntax: NumericLiteralExpression, '3')
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_TrueCondition()
        {
            string source = @"
class C
{
    static void Main()
    {
        int i = 0;
        int j;
        /*<bind>*/for (j = 0; true; j = j + 1)
        {
            i = i + 1;
            break;
        }/*</bind>*/
        System.Console.Write(""{0},{1}"", i, j);
    }
}
";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([2] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (j = 0; ... }') (Parent: BlockStatement)
  Condition: 
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Boolean, Constant: True) (Syntax: TrueLiteralExpression, 'true')
  Before:
    IExpressionStatement ([0] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'j = 0')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'j = 0')
          Left: 
            ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
          Right: 
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  AtLoopBottom:
    IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'j = j + 1')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'j = j + 1')
          Left: 
            ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
          Right: 
            IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'j + 1')
              Left: 
                ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  Body: 
    IBlockStatement (2 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'i = i + 1;')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
            Left: 
              ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
            Right: 
              IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'i + 1')
                Left: 
                  ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                Right: 
                  ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      IBranchStatement (BranchKind.Break) ([1] OperationKind.BranchStatement) (Syntax: BreakStatement, 'break;')
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_FalseCondition()
        {
            string source = @"
class C
{
    static void Main()
    {
        int i = 0;
        int j;
        /*<bind>*/for (j = 0; false; j = j + 1)
        {
            i = i + 1;
            break;
        }/*</bind>*/
        System.Console.Write(""{0},{1}"", i, j);
    }
}
";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([2] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (j = 0; ... }') (Parent: BlockStatement)
  Condition: 
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Boolean, Constant: False) (Syntax: FalseLiteralExpression, 'false')
  Before:
    IExpressionStatement ([0] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'j = 0')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'j = 0')
          Left: 
            ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
          Right: 
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  AtLoopBottom:
    IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'j = j + 1')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'j = j + 1')
          Left: 
            ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
          Right: 
            IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'j + 1')
              Left: 
                ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  Body: 
    IBlockStatement (2 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'i = i + 1;')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
            Left: 
              ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
            Right: 
              IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'i + 1')
                Left: 
                  ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                Right: 
                  ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      IBranchStatement (BranchKind.Break) ([1] OperationKind.BranchStatement) (Syntax: BreakStatement, 'break;')
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_WithContinue()
        {
            string source = @"
class C
{
    static void Main()
    {
        int i;
        int j;
        /*<bind>*/for (i = 0, j = 0; i < 5; i = i + 1)
        {
            if (i > 2) continue;
            j = j + 1;
        }/*</bind>*/
        System.Console.Write(""{0},{1}, "", i, j);
    }
}
";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([2] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (i = 0, ... }') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([2] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < 5')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
  Before:
    IExpressionStatement ([0] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'i = 0')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = 0')
          Left: 
            ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
          Right: 
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
    IExpressionStatement ([1] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'j = 0')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'j = 0')
          Left: 
            ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
          Right: 
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  AtLoopBottom:
    IExpressionStatement ([4] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
          Left: 
            ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
          Right: 
            IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'i + 1')
              Left: 
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  Body: 
    IBlockStatement (2 statements) ([3] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IIfStatement ([0] OperationKind.IfStatement) (Syntax: IfStatement, 'if (i > 2) continue;')
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'i > 2')
            Left: 
              ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
        IfTrue: 
          IBranchStatement (BranchKind.Continue) ([1] OperationKind.BranchStatement) (Syntax: ContinueStatement, 'continue;')
        IfFalse: 
          null
      IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'j = j + 1;')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'j = j + 1')
            Left: 
              ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
            Right: 
              IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'j + 1')
                Left: 
                  ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
                Right: 
                  ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_WithBreak()
        {
            string source = @"
class C
{
    static void Main()
    {
        int i;
        int j;
        /*<bind>*/for (i = 0, j = 0; i < 5; i = i + 1)
        {
            if (i > 3) break;
            j = j + 1;
        }/*</bind>*/
        System.Console.Write(""{0}, {1}"", i, j);
    }
}
";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([2] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (i = 0, ... }') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([2] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < 5')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
  Before:
    IExpressionStatement ([0] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'i = 0')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = 0')
          Left: 
            ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
          Right: 
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
    IExpressionStatement ([1] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'j = 0')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'j = 0')
          Left: 
            ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
          Right: 
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  AtLoopBottom:
    IExpressionStatement ([4] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
          Left: 
            ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
          Right: 
            IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'i + 1')
              Left: 
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  Body: 
    IBlockStatement (2 statements) ([3] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IIfStatement ([0] OperationKind.IfStatement) (Syntax: IfStatement, 'if (i > 3) break;')
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'i > 3')
            Left: 
              ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3) (Syntax: NumericLiteralExpression, '3')
        IfTrue: 
          IBranchStatement (BranchKind.Break) ([1] OperationKind.BranchStatement) (Syntax: BreakStatement, 'break;')
        IfFalse: 
          null
      IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'j = j + 1;')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'j = j + 1')
            Left: 
              ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
            Right: 
              IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'j + 1')
                Left: 
                  ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
                Right: 
                  ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_WithNoStatement()
        {
            string source = @"
class C
{
    static void Main()
    {
        int i = 0;
        /*<bind>*/for (;;)
        {
            if (i > 4) break;
            i = i + 2;
        }/*</bind>*/
        System.Console.Write(""{0}"", i);
    }
}
";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([1] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (;;) ... }') (Parent: BlockStatement)
  Condition: 
    null
  Before(0)
  AtLoopBottom(0)
  Body: 
    IBlockStatement (2 statements) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IIfStatement ([0] OperationKind.IfStatement) (Syntax: IfStatement, 'if (i > 4) break;')
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'i > 4')
            Left: 
              ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 4) (Syntax: NumericLiteralExpression, '4')
        IfTrue: 
          IBranchStatement (BranchKind.Break) ([1] OperationKind.BranchStatement) (Syntax: BreakStatement, 'break;')
        IfFalse: 
          null
      IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'i = i + 2;')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = i + 2')
            Left: 
              ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
            Right: 
              IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'i + 2')
                Left: 
                  ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                Right: 
                  ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_MultipleInitializer()
        {
            string source = @"
class C
{
    static void Main()
    {
        int i = 0;
        int j = 0;
        /*<bind>*/for (i = i + 1, i = i + 1; j < 2; i = i + 2, j = j + 1)
        {
        }/*</bind>*/
        System.Console.Write(""{0}"", i);
    }
}
";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([2] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (i = i  ... }') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([2] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'j < 2')
      Left: 
        ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
  Before:
    IExpressionStatement ([0] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
          Left: 
            ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
          Right: 
            IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'i + 1')
              Left: 
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
    IExpressionStatement ([1] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
          Left: 
            ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
          Right: 
            IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'i + 1')
              Left: 
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  AtLoopBottom:
    IExpressionStatement ([4] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'i = i + 2')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = i + 2')
          Left: 
            ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
          Right: 
            IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'i + 2')
              Left: 
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
    IExpressionStatement ([5] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'j = j + 1')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'j = j + 1')
          Left: 
            ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
          Right: 
            IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'j + 1')
              Left: 
                ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  Body: 
    IBlockStatement (0 statements) ([3] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_InitializerMissing()
        {
            string source = @"
class C
{
    static void Main(string[] args)
    {
        int i = 1;
        /*<bind>*/for (; i < 10; i = i + 1)
        {
        }/*</bind>*/
    }
}

";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([1] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (; i <  ... }') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < 10')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
  Before(0)
  AtLoopBottom:
    IExpressionStatement ([2] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
          Left: 
            ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
          Right: 
            IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'i + 1')
              Left: 
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  Body: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_DecreasingIterator()
        {
            string source = @"
class C
{
    static void Main(string[] args)
    {
        /*<bind>*/for (int k = 200; k > 100; k = k - 1)
        {
        }/*</bind>*/
    }
}

";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (int k  ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 k
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'k > 100')
      Left: 
        ILocalReferenceExpression: k ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'k')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 100) (Syntax: NumericLiteralExpression, '100')
  Before:
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int k = 200')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'k = 200')
        Variables: Local_1: System.Int32 k
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 200')
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 200) (Syntax: NumericLiteralExpression, '200')
  AtLoopBottom:
    IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'k = k - 1')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'k = k - 1')
          Left: 
            ILocalReferenceExpression: k ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'k')
          Right: 
            IBinaryOperatorExpression (BinaryOperatorKind.Subtract) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: SubtractExpression, 'k - 1')
              Left: 
                ILocalReferenceExpression: k ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'k')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  Body: 
    IBlockStatement (0 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_MethodCall()
        {
            string source = @"
class C
{
    static void Main(string[] args)
    {
        /*<bind>*/for (Initializer(); Conditional(); Iterator())
        {
        }/*</bind>*/
    }
    public static int Initializer() { return 1; }
    public static bool Conditional()
    { return true; }
    public static int Iterator() { return 1; }
}

";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (Initia ... }') (Parent: BlockStatement)
  Condition: 
    IInvocationExpression (System.Boolean C.Conditional()) ([1] OperationKind.InvocationExpression, Type: System.Boolean) (Syntax: InvocationExpression, 'Conditional()')
      Instance Receiver: 
        null
      Arguments(0)
  Before:
    IExpressionStatement ([0] OperationKind.ExpressionStatement, IsImplicit) (Syntax: InvocationExpression, 'Initializer()')
      Expression: 
        IInvocationExpression (System.Int32 C.Initializer()) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'Initializer()')
          Instance Receiver: 
            null
          Arguments(0)
  AtLoopBottom:
    IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: InvocationExpression, 'Iterator()')
      Expression: 
        IInvocationExpression (System.Int32 C.Iterator()) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'Iterator()')
          Instance Receiver: 
            null
          Arguments(0)
  Body: 
    IBlockStatement (0 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_MissingForBody()
        {
            string source = @"
class C
{
    static void Main(string[] args)
    {
        /*<bind>*/for (int i = 10; i < 100; i = i + 1) ;/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (int i  ...  = i + 1) ;') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 i
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < 100')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 100) (Syntax: NumericLiteralExpression, '100')
  Before:
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int i = 10')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i = 10')
        Variables: Local_1: System.Int32 i
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 10')
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
  AtLoopBottom:
    IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
          Left: 
            ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
          Right: 
            IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'i + 1')
              Left: 
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  Body: 
    IEmptyStatement ([2] OperationKind.EmptyStatement) (Syntax: EmptyStatement, ';')
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_Nested()
        {
            string source = @"
class C
{
    static void Main(string[] args)
    {
        /*<bind>*/for (int i = 0; i < 100; i = i + 1)
        {
            for (int j = 0; j < 10; j = j + 1)
            {
            }
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (int i  ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 i
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < 100')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 100) (Syntax: NumericLiteralExpression, '100')
  Before:
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int i = 0')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i = 0')
        Variables: Local_1: System.Int32 i
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  AtLoopBottom:
    IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
          Left: 
            ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
          Right: 
            IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'i + 1')
              Left: 
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (int j  ... }')
        Locals: Local_1: System.Int32 j
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'j < 10')
            Left: 
              ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
        Before:
          IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int j = 0')
            IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'j = 0')
              Variables: Local_1: System.Int32 j
              Initializer: 
                IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
                  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
        AtLoopBottom:
          IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'j = j + 1')
            Expression: 
              ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'j = j + 1')
                Left: 
                  ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
                Right: 
                  IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'j + 1')
                    Left: 
                      ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
                    Right: 
                      ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
        Body: 
          IBlockStatement (0 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_ChangeOuterVariableInInnerLoop()
        {
            string source = @"
class C
{
    static void Main(string[] args)
    {
        /*<bind>*/for (int i = 0; i < 10; i = i + 1)
        {
            for (int j = 0; j < 10; j = j + 1)
            {
                i = 1;
            }
        }/*</bind>*/
    }
}

";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (int i  ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 i
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < 10')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
  Before:
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int i = 0')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i = 0')
        Variables: Local_1: System.Int32 i
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  AtLoopBottom:
    IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
          Left: 
            ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
          Right: 
            IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'i + 1')
              Left: 
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (int j  ... }')
        Locals: Local_1: System.Int32 j
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'j < 10')
            Left: 
              ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
        Before:
          IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int j = 0')
            IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'j = 0')
              Variables: Local_1: System.Int32 j
              Initializer: 
                IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
                  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
        AtLoopBottom:
          IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'j = j + 1')
            Expression: 
              ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'j = j + 1')
                Left: 
                  ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
                Right: 
                  IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'j + 1')
                    Left: 
                      ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
                    Right: 
                      ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
        Body: 
          IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'i = 1;')
              Expression: 
                ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = 1')
                  Left: 
                    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                  Right: 
                    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_InnerLoopRefOuterIteration()
        {
            string source = @"
class C
{
    static void Main(string[] args)
    {
        /*<bind>*/for (int i = 0; i < 5; i = i + 1)
        {
            for (int j = i + 1; i < j; j = j - 1)
            {
            }
        }/*</bind>*/
    }
}

";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (int i  ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 i
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < 5')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
  Before:
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int i = 0')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i = 0')
        Variables: Local_1: System.Int32 i
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  AtLoopBottom:
    IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
          Left: 
            ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
          Right: 
            IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'i + 1')
              Left: 
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (int j  ... }')
        Locals: Local_1: System.Int32 j
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < j')
            Left: 
              ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
            Right: 
              ILocalReferenceExpression: j ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
        Before:
          IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int j = i + 1')
            IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'j = i + 1')
              Variables: Local_1: System.Int32 j
              Initializer: 
                IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= i + 1')
                  IBinaryOperatorExpression (BinaryOperatorKind.Add) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'i + 1')
                    Left: 
                      ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                    Right: 
                      ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
        AtLoopBottom:
          IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'j = j - 1')
            Expression: 
              ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'j = j - 1')
                Left: 
                  ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
                Right: 
                  IBinaryOperatorExpression (BinaryOperatorKind.Subtract) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: SubtractExpression, 'j - 1')
                    Left: 
                      ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
                    Right: 
                      ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
        Body: 
          IBlockStatement (0 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_BreakFromNestedLoop()
        {
            string source = @"
class C
{
    static void Main(string[] args)
    {
        /*<bind>*/for (int i = 0; i < 5; i = i + 1)
        {
            for (int j = 0; j < 10; j = j + 1)
            {
                if (j == 5)
                    break;
            }
        }/*</bind>*/
    }
}

";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (int i  ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 i
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < 5')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
  Before:
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int i = 0')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i = 0')
        Variables: Local_1: System.Int32 i
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  AtLoopBottom:
    IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
          Left: 
            ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
          Right: 
            IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'i + 1')
              Left: 
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (int j  ... }')
        Locals: Local_1: System.Int32 j
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'j < 10')
            Left: 
              ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
        Before:
          IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int j = 0')
            IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'j = 0')
              Variables: Local_1: System.Int32 j
              Initializer: 
                IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
                  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
        AtLoopBottom:
          IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'j = j + 1')
            Expression: 
              ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'j = j + 1')
                Left: 
                  ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
                Right: 
                  IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'j + 1')
                    Left: 
                      ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
                    Right: 
                      ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
        Body: 
          IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
            IIfStatement ([0] OperationKind.IfStatement) (Syntax: IfStatement, 'if (j == 5) ... break;')
              Condition: 
                IBinaryOperatorExpression (BinaryOperatorKind.Equals) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: EqualsExpression, 'j == 5')
                  Left: 
                    ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
                  Right: 
                    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
              IfTrue: 
                IBranchStatement (BranchKind.Break) ([1] OperationKind.BranchStatement) (Syntax: BreakStatement, 'break;')
              IfFalse: 
                null
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_ContinueForNestedLoop()
        {
            string source = @"
class C
{
    static void Main(string[] args)
    {
        /*<bind>*/for (int i = 0; i < 5; i = i + 1)
        {
            for (int j = 1; j < 10; j = j + 1)
            {
                if ((j % 2) != 0)
                    continue;
                i = i + 1;
                System.Console.Write(i);
            }
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (int i  ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 i
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < 5')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
  Before:
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int i = 0')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i = 0')
        Variables: Local_1: System.Int32 i
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  AtLoopBottom:
    IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
          Left: 
            ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
          Right: 
            IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'i + 1')
              Left: 
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (int j  ... }')
        Locals: Local_1: System.Int32 j
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'j < 10')
            Left: 
              ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
        Before:
          IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int j = 1')
            IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'j = 1')
              Variables: Local_1: System.Int32 j
              Initializer: 
                IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 1')
                  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
        AtLoopBottom:
          IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'j = j + 1')
            Expression: 
              ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'j = j + 1')
                Left: 
                  ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
                Right: 
                  IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'j + 1')
                    Left: 
                      ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
                    Right: 
                      ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
        Body: 
          IBlockStatement (3 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
            IIfStatement ([0] OperationKind.IfStatement) (Syntax: IfStatement, 'if ((j % 2) ... continue;')
              Condition: 
                IBinaryOperatorExpression (BinaryOperatorKind.NotEquals) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: NotEqualsExpression, '(j % 2) != 0')
                  Left: 
                    IBinaryOperatorExpression (BinaryOperatorKind.Remainder) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: ModuloExpression, 'j % 2')
                      Left: 
                        ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
                      Right: 
                        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
                  Right: 
                    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
              IfTrue: 
                IBranchStatement (BranchKind.Continue) ([1] OperationKind.BranchStatement) (Syntax: ContinueStatement, 'continue;')
              IfFalse: 
                null
            IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'i = i + 1;')
              Expression: 
                ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
                  Left: 
                    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                  Right: 
                    IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'i + 1')
                      Left: 
                        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                      Right: 
                        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
            IExpressionStatement ([2] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Console.Write(i);')
              Expression: 
                IInvocationExpression (void System.Console.Write(System.Int32 value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Console.Write(i)')
                  Instance Receiver: 
                    null
                  Arguments(1):
                    IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'i')
                      ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_GotoForNestedLoop_1()
        {
            string source = @"
class C
{
    static void Main(string[] args)
    {
        /*<bind>*/for (int i = 0; i < 5; i = i + 1)
        {
            for (int j = 0; j < 10; j = j + 1)
            {
                goto stop;
            stop:
                j = j + 1;
            }
        }/*</bind>*/
    }
}

";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (int i  ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 i
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < 5')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
  Before:
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int i = 0')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i = 0')
        Variables: Local_1: System.Int32 i
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  AtLoopBottom:
    IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
          Left: 
            ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
          Right: 
            IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'i + 1')
              Left: 
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (int j  ... }')
        Locals: Local_1: System.Int32 j
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'j < 10')
            Left: 
              ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
        Before:
          IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int j = 0')
            IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'j = 0')
              Variables: Local_1: System.Int32 j
              Initializer: 
                IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
                  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
        AtLoopBottom:
          IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'j = j + 1')
            Expression: 
              ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'j = j + 1')
                Left: 
                  ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
                Right: 
                  IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'j + 1')
                    Left: 
                      ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
                    Right: 
                      ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
        Body: 
          IBlockStatement (2 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
            IBranchStatement (BranchKind.GoTo, Label: stop) ([0] OperationKind.BranchStatement) (Syntax: GotoStatement, 'goto stop;')
            ILabeledStatement (Label: stop) ([1] OperationKind.LabeledStatement) (Syntax: LabeledStatement, 'stop: ... j = j + 1;')
              Statement: 
                IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'j = j + 1;')
                  Expression: 
                    ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'j = j + 1')
                      Left: 
                        ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
                      Right: 
                        IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'j + 1')
                          Left: 
                            ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
                          Right: 
                            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_ThrowException()
        {
            string source = @"
class C
{
    static void Main(string[] args)
    {
        /*<bind>*/for (int i = 0; i < 10; i = i + 1)
        {
            for (int j = 0; j < 10;)
            {
                throw new System.Exception();
            }
        }/*</bind>*/
    }
}

";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (int i  ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 i
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < 10')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
  Before:
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int i = 0')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i = 0')
        Variables: Local_1: System.Int32 i
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  AtLoopBottom:
    IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
          Left: 
            ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
          Right: 
            IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'i + 1')
              Left: 
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (int j  ... }')
        Locals: Local_1: System.Int32 j
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'j < 10')
            Left: 
              ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
        Before:
          IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int j = 0')
            IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'j = 0')
              Variables: Local_1: System.Int32 j
              Initializer: 
                IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
                  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
        AtLoopBottom(0)
        Body: 
          IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ThrowStatement, 'throw new S ... xception();')
              Expression: 
                IThrowExpression ([0] OperationKind.ThrowExpression, Type: System.Exception) (Syntax: ThrowStatement, 'throw new S ... xception();')
                  IObjectCreationExpression (Constructor: System.Exception..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: System.Exception) (Syntax: ObjectCreationExpression, 'new System.Exception()')
                    Arguments(0)
                    Initializer: 
                      null
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_ReturnInFor()
        {
            string source = @"
class C
{
    static void Main(string[] args)
    {
        /*<bind>*/for (int i = 0; i < 10;)
        {
            for (int j = 0; j < 5;)
            {
                return;
            }
        }/*</bind>*/
    }
}

";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (int i  ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 i
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < 10')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
  Before:
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int i = 0')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i = 0')
        Variables: Local_1: System.Int32 i
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  AtLoopBottom(0)
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (int j  ... }')
        Locals: Local_1: System.Int32 j
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'j < 5')
            Left: 
              ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
        Before:
          IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int j = 0')
            IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'j = 0')
              Variables: Local_1: System.Int32 j
              Initializer: 
                IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
                  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
        AtLoopBottom(0)
        Body: 
          IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
            IReturnStatement ([0] OperationKind.ReturnStatement) (Syntax: ReturnStatement, 'return;')
              ReturnedValue: 
                null
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_ChangeValueOfInit()
        {
            string source = @"
class C
{
    static void Main(string[] args)
    {
        /*<bind>*/for (int i = 0, j = 1; i < 5; i = i + 1)
        {
            j = 2;
        }/*</bind>*/
    }
}

";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (int i  ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 i
    Local_2: System.Int32 j
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < 5')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
  Before:
    IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int i = 0, j = 1')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i = 0')
        Variables: Local_1: System.Int32 i
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
      IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'j = 1')
        Variables: Local_1: System.Int32 j
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 1')
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  AtLoopBottom:
    IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
          Left: 
            ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
          Right: 
            IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'i + 1')
              Left: 
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'j = 2;')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'j = 2')
            Left: 
              ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_ChangeValueOfCondition()
        {
            string source = @"
class C
{
    static void Main(string[] args)
    {
        int c = 0, x = 0;
        /*<bind>*/for (int i = 0; i < 50 - x; i = i + 1)
        {
            x = x + 1;
            c = c + 1;
        }/*</bind>*/
    }
}

";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([1] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (int i  ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 i
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < 50 - x')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        IBinaryOperatorExpression (BinaryOperatorKind.Subtract) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: SubtractExpression, '50 - x')
          Left: 
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 50) (Syntax: NumericLiteralExpression, '50')
          Right: 
            ILocalReferenceExpression: x ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
  Before:
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int i = 0')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i = 0')
        Variables: Local_1: System.Int32 i
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  AtLoopBottom:
    IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
          Left: 
            ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
          Right: 
            IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'i + 1')
              Left: 
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  Body: 
    IBlockStatement (2 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'x = x + 1;')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'x = x + 1')
            Left: 
              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
            Right: 
              IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'x + 1')
                Left: 
                  ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                Right: 
                  ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'c = c + 1;')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'c = c + 1')
            Left: 
              ILocalReferenceExpression: c ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'c')
            Right: 
              IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'c + 1')
                Left: 
                  ILocalReferenceExpression: c ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'c')
                Right: 
                  ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_UnreachableCode1()
        {
            string source = @"
class C
{
    static void Main(string[] args)
    {
        /*<bind>*/for (; false;)
        {
            System.Console.WriteLine(""hello"");        //unreachable
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (; fals ... }') (Parent: BlockStatement)
  Condition: 
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Boolean, Constant: False) (Syntax: FalseLiteralExpression, 'false')
  Before(0)
  AtLoopBottom(0)
  Body: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... e(""hello"");')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.String value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... ne(""hello"")')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, '""hello""')
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: ""hello"") (Syntax: StringLiteralExpression, '""hello""')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_ObjectInitAsInitializer()
        {
            string source = @"
class C
{
    static void Main(string[] args)
    {
        /*<bind>*/for (F f = new F { i = 0, s = ""abc"" }; f.i < 5; f.i = f.i + 1)
        {
        }/*</bind>*/
    }
}
public class F
{
    public int i;
    public string s;
}

";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (F f =  ... }') (Parent: BlockStatement)
  Locals: Local_1: F f
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'f.i < 5')
      Left: 
        IFieldReferenceExpression: System.Int32 F.i ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: SimpleMemberAccessExpression, 'f.i')
          Instance Receiver: 
            ILocalReferenceExpression: f ([0] OperationKind.LocalReferenceExpression, Type: F) (Syntax: IdentifierName, 'f')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
  Before:
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'F f = new F ... s = ""abc"" }')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'f = new F { ... s = ""abc"" }')
        Variables: Local_1: F f
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= new F { i ... s = ""abc"" }')
            IObjectCreationExpression (Constructor: F..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: F) (Syntax: ObjectCreationExpression, 'new F { i = ... s = ""abc"" }')
              Arguments(0)
              Initializer: 
                IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: F) (Syntax: ObjectInitializerExpression, '{ i = 0, s = ""abc"" }')
                  Initializers(2):
                    ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = 0')
                      Left: 
                        IFieldReferenceExpression: System.Int32 F.i ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                          Instance Receiver: 
                            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: F, IsImplicit) (Syntax: IdentifierName, 'i')
                      Right: 
                        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
                    ISimpleAssignmentExpression ([1] OperationKind.SimpleAssignmentExpression, Type: System.String) (Syntax: SimpleAssignmentExpression, 's = ""abc""')
                      Left: 
                        IFieldReferenceExpression: System.String F.s ([0] OperationKind.FieldReferenceExpression, Type: System.String) (Syntax: IdentifierName, 's')
                          Instance Receiver: 
                            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: F, IsImplicit) (Syntax: IdentifierName, 's')
                      Right: 
                        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.String, Constant: ""abc"") (Syntax: StringLiteralExpression, '""abc""')
  AtLoopBottom:
    IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'f.i = f.i + 1')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'f.i = f.i + 1')
          Left: 
            IFieldReferenceExpression: System.Int32 F.i ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: SimpleMemberAccessExpression, 'f.i')
              Instance Receiver: 
                ILocalReferenceExpression: f ([0] OperationKind.LocalReferenceExpression, Type: F) (Syntax: IdentifierName, 'f')
          Right: 
            IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'f.i + 1')
              Left: 
                IFieldReferenceExpression: System.Int32 F.i ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: SimpleMemberAccessExpression, 'f.i')
                  Instance Receiver: 
                    ILocalReferenceExpression: f ([0] OperationKind.LocalReferenceExpression, Type: F) (Syntax: IdentifierName, 'f')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  Body: 
    IBlockStatement (0 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_DynamicInFor()
        {
            string source = @"
class C
{
    static void Main(string[] args)
    {
        dynamic d = new myFor();
        /*<bind>*/for (d.Initialize(5); d.Done; d.Next())
        {
        }/*</bind>*/
    }
}

public class myFor
{
    int index;
    int max;
    public void Initialize(int max)
    {
        index = 0;
        this.max = max;
        System.Console.WriteLine(""Initialize"");
    }
    public bool Done
    {
        get
        {
            System.Console.WriteLine(""Done"");
            return index < max;
        }
    }
    public void Next()
    {
        index = index + 1;
        System.Console.WriteLine(""Next"");
    }
}
";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([1] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (d.Init ... }') (Parent: BlockStatement)
  Condition: 
    IUnaryOperatorExpression (UnaryOperatorKind.True) ([1] OperationKind.UnaryOperatorExpression, Type: System.Boolean, IsImplicit) (Syntax: SimpleMemberAccessExpression, 'd.Done')
      Operand: 
        IDynamicMemberReferenceExpression (Member Name: ""Done"", Containing Type: null) ([0] OperationKind.DynamicMemberReferenceExpression, Type: dynamic) (Syntax: SimpleMemberAccessExpression, 'd.Done')
          Type Arguments(0)
          Instance Receiver: 
            ILocalReferenceExpression: d ([0] OperationKind.LocalReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'd')
  Before:
    IExpressionStatement ([0] OperationKind.ExpressionStatement, IsImplicit) (Syntax: InvocationExpression, 'd.Initialize(5)')
      Expression: 
        IDynamicInvocationExpression ([0] OperationKind.DynamicInvocationExpression, Type: dynamic) (Syntax: InvocationExpression, 'd.Initialize(5)')
          Expression: 
            IDynamicMemberReferenceExpression (Member Name: ""Initialize"", Containing Type: null) ([0] OperationKind.DynamicMemberReferenceExpression, Type: dynamic) (Syntax: SimpleMemberAccessExpression, 'd.Initialize')
              Type Arguments(0)
              Instance Receiver: 
                ILocalReferenceExpression: d ([0] OperationKind.LocalReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'd')
          Arguments(1):
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
          ArgumentNames(0)
          ArgumentRefKinds(0)
  AtLoopBottom:
    IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: InvocationExpression, 'd.Next()')
      Expression: 
        IDynamicInvocationExpression ([0] OperationKind.DynamicInvocationExpression, Type: dynamic) (Syntax: InvocationExpression, 'd.Next()')
          Expression: 
            IDynamicMemberReferenceExpression (Member Name: ""Next"", Containing Type: null) ([0] OperationKind.DynamicMemberReferenceExpression, Type: dynamic) (Syntax: SimpleMemberAccessExpression, 'd.Next')
              Type Arguments(0)
              Instance Receiver: 
                ILocalReferenceExpression: d ([0] OperationKind.LocalReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'd')
          Arguments(0)
          ArgumentNames(0)
          ArgumentRefKinds(0)
  Body: 
    IBlockStatement (0 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
";

            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_VarInFor()
        {
            string source = @"
class C
{
    static void Main(string[] args)
    {
        /*<bind>*/for (var i = 1; i < 5; i = i + 1) ;/*</bind>*/
    }
}

";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (var i  ...  = i + 1) ;') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 i
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < 5')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
  Before:
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'var i = 1')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i = 1')
        Variables: Local_1: System.Int32 i
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 1')
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  AtLoopBottom:
    IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
          Left: 
            ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
          Right: 
            IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'i + 1')
              Left: 
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  Body: 
    IEmptyStatement ([2] OperationKind.EmptyStatement) (Syntax: EmptyStatement, ';')
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_QueryInInit()
        {
            string source = @"
using System.Linq;
using System.Collections.Generic;
class C
{
    static void Main(string[] args)
    {
        /*<bind>*/for (IEnumerable<string> str = from x in ""123""
                                       let z = x.ToString()
                                       select z into w
                                       select w; ; )
        {
            foreach (var item in str)
            {
                System.Console.WriteLine(item);
            }
            return;
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (IEnume ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Collections.Generic.IEnumerable<System.String> str
  Condition: 
    null
  Before:
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'IEnumerable ... select w')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'str = from  ... select w')
        Variables: Local_1: System.Collections.Generic.IEnumerable<System.String> str
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= from x in ... select w')
            ITranslatedQueryExpression ([0] OperationKind.TranslatedQueryExpression, Type: System.Collections.Generic.IEnumerable<System.String>) (Syntax: QueryExpression, 'from x in "" ... select w')
              Expression: 
                IInvocationExpression (System.Collections.Generic.IEnumerable<System.String> System.Linq.Enumerable.Select<System.String, System.String>(this System.Collections.Generic.IEnumerable<System.String> source, System.Func<System.String, System.String> selector)) ([0] OperationKind.InvocationExpression, Type: System.Collections.Generic.IEnumerable<System.String>, IsImplicit) (Syntax: SelectClause, 'select w')
                  Instance Receiver: 
                    null
                  Arguments(2):
                    IArgument (ArgumentKind.Explicit, Matching Parameter: source) ([0] OperationKind.Argument, IsImplicit) (Syntax: SelectClause, 'select z')
                      IInvocationExpression (System.Collections.Generic.IEnumerable<System.String> System.Linq.Enumerable.Select<<anonymous type: System.Char x, System.String z>, System.String>(this System.Collections.Generic.IEnumerable<<anonymous type: System.Char x, System.String z>> source, System.Func<<anonymous type: System.Char x, System.String z>, System.String> selector)) ([0] OperationKind.InvocationExpression, Type: System.Collections.Generic.IEnumerable<System.String>, IsImplicit) (Syntax: SelectClause, 'select z')
                        Instance Receiver: 
                          null
                        Arguments(2):
                          IArgument (ArgumentKind.Explicit, Matching Parameter: source) ([0] OperationKind.Argument, IsImplicit) (Syntax: LetClause, 'let z = x.ToString()')
                            IInvocationExpression (System.Collections.Generic.IEnumerable<<anonymous type: System.Char x, System.String z>> System.Linq.Enumerable.Select<System.Char, <anonymous type: System.Char x, System.String z>>(this System.Collections.Generic.IEnumerable<System.Char> source, System.Func<System.Char, <anonymous type: System.Char x, System.String z>> selector)) ([0] OperationKind.InvocationExpression, Type: System.Collections.Generic.IEnumerable<<anonymous type: System.Char x, System.String z>>, IsImplicit) (Syntax: LetClause, 'let z = x.ToString()')
                              Instance Receiver: 
                                null
                              Arguments(2):
                                IArgument (ArgumentKind.Explicit, Matching Parameter: source) ([0] OperationKind.Argument, IsImplicit) (Syntax: FromClause, 'from x in ""123""')
                                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.Generic.IEnumerable<System.Char>, IsImplicit) (Syntax: FromClause, 'from x in ""123""')
                                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
                                    Operand: 
                                      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: ""123"") (Syntax: StringLiteralExpression, '""123""')
                                  InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                                  OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                                IArgument (ArgumentKind.Explicit, Matching Parameter: selector) ([1] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'x.ToString()')
                                  IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Func<System.Char, <anonymous type: System.Char x, System.String z>>, IsImplicit) (Syntax: InvocationExpression, 'x.ToString()')
                                    Target: 
                                      IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null, IsImplicit) (Syntax: InvocationExpression, 'x.ToString()')
                                        IBlockStatement (1 statements) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: InvocationExpression, 'x.ToString()')
                                          IReturnStatement ([0] OperationKind.ReturnStatement, IsImplicit) (Syntax: InvocationExpression, 'x.ToString()')
                                            ReturnedValue: 
                                              IObjectCreationExpression (Constructor: <anonymous type: System.Char x, System.String z>..ctor(System.Char x, System.String z)) ([0] OperationKind.ObjectCreationExpression, Type: <anonymous type: System.Char x, System.String z>, IsImplicit) (Syntax: LetClause, 'let z = x.ToString()')
                                                Arguments(2):
                                                  IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([0] OperationKind.Argument, IsImplicit) (Syntax: LetClause, 'let z = x.ToString()')
                                                    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Char, IsImplicit) (Syntax: LetClause, 'let z = x.ToString()')
                                                    InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                                                    OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                                                  IArgument (ArgumentKind.Explicit, Matching Parameter: z) ([1] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'x.ToString()')
                                                    IInvocationExpression (virtual System.String System.Char.ToString()) ([0] OperationKind.InvocationExpression, Type: System.String) (Syntax: InvocationExpression, 'x.ToString()')
                                                      Instance Receiver: 
                                                        IOperation:  ([0] OperationKind.None) (Syntax: IdentifierName, 'x')
                                                      Arguments(0)
                                                    InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                                                    OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                                                Initializer: 
                                                  null
                                  InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                                  OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                            InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                            OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                          IArgument (ArgumentKind.Explicit, Matching Parameter: selector) ([1] OperationKind.Argument, IsImplicit) (Syntax: IdentifierName, 'z')
                            IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Func<<anonymous type: System.Char x, System.String z>, System.String>, IsImplicit) (Syntax: IdentifierName, 'z')
                              Target: 
                                IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null, IsImplicit) (Syntax: IdentifierName, 'z')
                                  IBlockStatement (1 statements) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: IdentifierName, 'z')
                                    IReturnStatement ([0] OperationKind.ReturnStatement, IsImplicit) (Syntax: IdentifierName, 'z')
                                      ReturnedValue: 
                                        IOperation:  ([0] OperationKind.None) (Syntax: IdentifierName, 'z')
                            InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                            OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    IArgument (ArgumentKind.Explicit, Matching Parameter: selector) ([1] OperationKind.Argument, IsImplicit) (Syntax: IdentifierName, 'w')
                      IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Func<System.String, System.String>, IsImplicit) (Syntax: IdentifierName, 'w')
                        Target: 
                          IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null, IsImplicit) (Syntax: IdentifierName, 'w')
                            IBlockStatement (1 statements) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: IdentifierName, 'w')
                              IReturnStatement ([0] OperationKind.ReturnStatement, IsImplicit) (Syntax: IdentifierName, 'w')
                                ReturnedValue: 
                                  IOperation:  ([0] OperationKind.None) (Syntax: IdentifierName, 'w')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  AtLoopBottom(0)
  Body: 
    IBlockStatement (2 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IForEachLoopStatement (LoopKind.ForEach) ([0] OperationKind.LoopStatement) (Syntax: ForEachStatement, 'foreach (va ... }')
        Locals: Local_1: System.String item
        LoopControlVariable: 
          ILocalReferenceExpression: item (IsDeclaration: True) ([1] OperationKind.LocalReferenceExpression, Type: System.String, Constant: null) (Syntax: ForEachStatement, 'foreach (va ... }')
        Collection: 
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.Generic.IEnumerable<System.String>, IsImplicit) (Syntax: IdentifierName, 'str')
            Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              ILocalReferenceExpression: str ([0] OperationKind.LocalReferenceExpression, Type: System.Collections.Generic.IEnumerable<System.String>) (Syntax: IdentifierName, 'str')
        Body: 
          IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... Line(item);')
              Expression: 
                IInvocationExpression (void System.Console.WriteLine(System.String value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... eLine(item)')
                  Instance Receiver: 
                    null
                  Arguments(1):
                    IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'item')
                      ILocalReferenceExpression: item ([0] OperationKind.LocalReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'item')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        NextVariables(0)
      IReturnStatement ([1] OperationKind.ReturnStatement) (Syntax: ReturnStatement, 'return;')
        ReturnedValue: 
          null
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_QueryInBody()
        {
            string source = @"
using System.Linq;
using System.Collections.Generic;
class C
{
    static void Main(string[] args)
    {
        foreach (var item in fun())
        {
            System.Console.WriteLine(item);
        }
    }

    private static IEnumerable<string> fun()
    {
        /*<bind>*/for (int i = 0; i < 5;)
        {
            return from x in ""123""
                   let z = x.ToString()
                   select z into w
                   select w;
        }/*</bind>*/
        return null;
    }
}

";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (int i  ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 i
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < 5')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
  Before:
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int i = 0')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i = 0')
        Variables: Local_1: System.Int32 i
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  AtLoopBottom(0)
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IReturnStatement ([0] OperationKind.ReturnStatement) (Syntax: ReturnStatement, 'return from ... select w;')
        ReturnedValue: 
          ITranslatedQueryExpression ([0] OperationKind.TranslatedQueryExpression, Type: System.Collections.Generic.IEnumerable<System.String>) (Syntax: QueryExpression, 'from x in "" ... select w')
            Expression: 
              IInvocationExpression (System.Collections.Generic.IEnumerable<System.String> System.Linq.Enumerable.Select<System.String, System.String>(this System.Collections.Generic.IEnumerable<System.String> source, System.Func<System.String, System.String> selector)) ([0] OperationKind.InvocationExpression, Type: System.Collections.Generic.IEnumerable<System.String>, IsImplicit) (Syntax: SelectClause, 'select w')
                Instance Receiver: 
                  null
                Arguments(2):
                  IArgument (ArgumentKind.Explicit, Matching Parameter: source) ([0] OperationKind.Argument, IsImplicit) (Syntax: SelectClause, 'select z')
                    IInvocationExpression (System.Collections.Generic.IEnumerable<System.String> System.Linq.Enumerable.Select<<anonymous type: System.Char x, System.String z>, System.String>(this System.Collections.Generic.IEnumerable<<anonymous type: System.Char x, System.String z>> source, System.Func<<anonymous type: System.Char x, System.String z>, System.String> selector)) ([0] OperationKind.InvocationExpression, Type: System.Collections.Generic.IEnumerable<System.String>, IsImplicit) (Syntax: SelectClause, 'select z')
                      Instance Receiver: 
                        null
                      Arguments(2):
                        IArgument (ArgumentKind.Explicit, Matching Parameter: source) ([0] OperationKind.Argument, IsImplicit) (Syntax: LetClause, 'let z = x.ToString()')
                          IInvocationExpression (System.Collections.Generic.IEnumerable<<anonymous type: System.Char x, System.String z>> System.Linq.Enumerable.Select<System.Char, <anonymous type: System.Char x, System.String z>>(this System.Collections.Generic.IEnumerable<System.Char> source, System.Func<System.Char, <anonymous type: System.Char x, System.String z>> selector)) ([0] OperationKind.InvocationExpression, Type: System.Collections.Generic.IEnumerable<<anonymous type: System.Char x, System.String z>>, IsImplicit) (Syntax: LetClause, 'let z = x.ToString()')
                            Instance Receiver: 
                              null
                            Arguments(2):
                              IArgument (ArgumentKind.Explicit, Matching Parameter: source) ([0] OperationKind.Argument, IsImplicit) (Syntax: FromClause, 'from x in ""123""')
                                IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.Generic.IEnumerable<System.Char>, IsImplicit) (Syntax: FromClause, 'from x in ""123""')
                                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
                                  Operand: 
                                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: ""123"") (Syntax: StringLiteralExpression, '""123""')
                                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                              IArgument (ArgumentKind.Explicit, Matching Parameter: selector) ([1] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'x.ToString()')
                                IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Func<System.Char, <anonymous type: System.Char x, System.String z>>, IsImplicit) (Syntax: InvocationExpression, 'x.ToString()')
                                  Target: 
                                    IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null, IsImplicit) (Syntax: InvocationExpression, 'x.ToString()')
                                      IBlockStatement (1 statements) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: InvocationExpression, 'x.ToString()')
                                        IReturnStatement ([0] OperationKind.ReturnStatement, IsImplicit) (Syntax: InvocationExpression, 'x.ToString()')
                                          ReturnedValue: 
                                            IObjectCreationExpression (Constructor: <anonymous type: System.Char x, System.String z>..ctor(System.Char x, System.String z)) ([0] OperationKind.ObjectCreationExpression, Type: <anonymous type: System.Char x, System.String z>, IsImplicit) (Syntax: LetClause, 'let z = x.ToString()')
                                              Arguments(2):
                                                IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([0] OperationKind.Argument, IsImplicit) (Syntax: LetClause, 'let z = x.ToString()')
                                                  IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Char, IsImplicit) (Syntax: LetClause, 'let z = x.ToString()')
                                                  InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                                                  OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                                                IArgument (ArgumentKind.Explicit, Matching Parameter: z) ([1] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'x.ToString()')
                                                  IInvocationExpression (virtual System.String System.Char.ToString()) ([0] OperationKind.InvocationExpression, Type: System.String) (Syntax: InvocationExpression, 'x.ToString()')
                                                    Instance Receiver: 
                                                      IOperation:  ([0] OperationKind.None) (Syntax: IdentifierName, 'x')
                                                    Arguments(0)
                                                  InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                                                  OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                                              Initializer: 
                                                null
                                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                        IArgument (ArgumentKind.Explicit, Matching Parameter: selector) ([1] OperationKind.Argument, IsImplicit) (Syntax: IdentifierName, 'z')
                          IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Func<<anonymous type: System.Char x, System.String z>, System.String>, IsImplicit) (Syntax: IdentifierName, 'z')
                            Target: 
                              IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null, IsImplicit) (Syntax: IdentifierName, 'z')
                                IBlockStatement (1 statements) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: IdentifierName, 'z')
                                  IReturnStatement ([0] OperationKind.ReturnStatement, IsImplicit) (Syntax: IdentifierName, 'z')
                                    ReturnedValue: 
                                      IOperation:  ([0] OperationKind.None) (Syntax: IdentifierName, 'z')
                          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  IArgument (ArgumentKind.Explicit, Matching Parameter: selector) ([1] OperationKind.Argument, IsImplicit) (Syntax: IdentifierName, 'w')
                    IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Func<System.String, System.String>, IsImplicit) (Syntax: IdentifierName, 'w')
                      Target: 
                        IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null, IsImplicit) (Syntax: IdentifierName, 'w')
                          IBlockStatement (1 statements) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: IdentifierName, 'w')
                            IReturnStatement ([0] OperationKind.ReturnStatement, IsImplicit) (Syntax: IdentifierName, 'w')
                              ReturnedValue: 
                                IOperation:  ([0] OperationKind.None) (Syntax: IdentifierName, 'w')
                    InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_ExpressiontreeInInit()
        {
            string source = @"
class C
{
    static void Main(string[] args)
    {
        System.Linq.Expressions.Expression<System.Func<int, int>> e = x => x % 6;
        int i = 1;
        /*<bind>*/for (e = x => x * x; i < 5; i++)
        {
            var lambda = e.Compile();
            System.Console.WriteLine(lambda(i));
        }/*</bind>*/
    }
}

";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([2] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (e = x  ... }') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < 5')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
  Before:
    IExpressionStatement ([0] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'e = x => x * x')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Linq.Expressions.Expression<System.Func<System.Int32, System.Int32>>) (Syntax: SimpleAssignmentExpression, 'e = x => x * x')
          Left: 
            ILocalReferenceExpression: e ([0] OperationKind.LocalReferenceExpression, Type: System.Linq.Expressions.Expression<System.Func<System.Int32, System.Int32>>) (Syntax: IdentifierName, 'e')
          Right: 
            IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Linq.Expressions.Expression<System.Func<System.Int32, System.Int32>>, IsImplicit) (Syntax: SimpleLambdaExpression, 'x => x * x')
              Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              Operand: 
                IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null) (Syntax: SimpleLambdaExpression, 'x => x * x')
                  IBlockStatement (1 statements) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiplyExpression, 'x * x')
                    IReturnStatement ([0] OperationKind.ReturnStatement, IsImplicit) (Syntax: MultiplyExpression, 'x * x')
                      ReturnedValue: 
                        IBinaryOperatorExpression (BinaryOperatorKind.Multiply) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: MultiplyExpression, 'x * x')
                          Left: 
                            IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                          Right: 
                            IParameterReferenceExpression: x ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
  AtLoopBottom:
    IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: PostIncrementExpression, 'i++')
      Expression: 
        IIncrementOrDecrementExpression (Postfix) ([0] OperationKind.IncrementExpression, Type: System.Int32) (Syntax: PostIncrementExpression, 'i++')
          Target: 
            ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
  Body: 
    IBlockStatement (2 statements, 1 locals) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      Locals: Local_1: System.Func<System.Int32, System.Int32> lambda
      IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'var lambda  ... .Compile();')
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'lambda = e.Compile()')
          Variables: Local_1: System.Func<System.Int32, System.Int32> lambda
          Initializer: 
            IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= e.Compile()')
              IInvocationExpression ( System.Func<System.Int32, System.Int32> System.Linq.Expressions.Expression<System.Func<System.Int32, System.Int32>>.Compile()) ([0] OperationKind.InvocationExpression, Type: System.Func<System.Int32, System.Int32>) (Syntax: InvocationExpression, 'e.Compile()')
                Instance Receiver: 
                  ILocalReferenceExpression: e ([0] OperationKind.LocalReferenceExpression, Type: System.Linq.Expressions.Expression<System.Func<System.Int32, System.Int32>>) (Syntax: IdentifierName, 'e')
                Arguments(0)
      IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... lambda(i));')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.Int32 value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... (lambda(i))')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'lambda(i)')
                IInvocationExpression (virtual System.Int32 System.Func<System.Int32, System.Int32>.Invoke(System.Int32 arg)) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'lambda(i)')
                  Instance Receiver: 
                    ILocalReferenceExpression: lambda ([0] OperationKind.LocalReferenceExpression, Type: System.Func<System.Int32, System.Int32>) (Syntax: IdentifierName, 'lambda')
                  Arguments(1):
                    IArgument (ArgumentKind.Explicit, Matching Parameter: arg) ([1] OperationKind.Argument) (Syntax: Argument, 'i')
                      ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_ExpressiontreeInIterator()
        {
            string source = @"
class C
{
    static void Main(string[] args)
    {
        System.Linq.Expressions.Expression<System.Func<int, int>> e = x => x % 6;
        /*<bind>*/for (int i = 1; i < 5; e = x => x * x, i = i + 1)
        {
            var lambda = e.Compile();
            System.Console.WriteLine(lambda(i));
        }/*</bind>*/
    }
}

";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([1] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (int i  ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 i
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < 5')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
  Before:
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int i = 1')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i = 1')
        Variables: Local_1: System.Int32 i
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 1')
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  AtLoopBottom:
    IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'e = x => x * x')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Linq.Expressions.Expression<System.Func<System.Int32, System.Int32>>) (Syntax: SimpleAssignmentExpression, 'e = x => x * x')
          Left: 
            ILocalReferenceExpression: e ([0] OperationKind.LocalReferenceExpression, Type: System.Linq.Expressions.Expression<System.Func<System.Int32, System.Int32>>) (Syntax: IdentifierName, 'e')
          Right: 
            IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Linq.Expressions.Expression<System.Func<System.Int32, System.Int32>>, IsImplicit) (Syntax: SimpleLambdaExpression, 'x => x * x')
              Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              Operand: 
                IAnonymousFunctionExpression (Symbol: lambda expression) ([0] OperationKind.AnonymousFunctionExpression, Type: null) (Syntax: SimpleLambdaExpression, 'x => x * x')
                  IBlockStatement (1 statements) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiplyExpression, 'x * x')
                    IReturnStatement ([0] OperationKind.ReturnStatement, IsImplicit) (Syntax: MultiplyExpression, 'x * x')
                      ReturnedValue: 
                        IBinaryOperatorExpression (BinaryOperatorKind.Multiply) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: MultiplyExpression, 'x * x')
                          Left: 
                            IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                          Right: 
                            IParameterReferenceExpression: x ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
    IExpressionStatement ([4] OperationKind.ExpressionStatement, IsImplicit) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
      Expression: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = i + 1')
          Left: 
            ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
          Right: 
            IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'i + 1')
              Left: 
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  Body: 
    IBlockStatement (2 statements, 1 locals) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      Locals: Local_1: System.Func<System.Int32, System.Int32> lambda
      IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'var lambda  ... .Compile();')
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'lambda = e.Compile()')
          Variables: Local_1: System.Func<System.Int32, System.Int32> lambda
          Initializer: 
            IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= e.Compile()')
              IInvocationExpression ( System.Func<System.Int32, System.Int32> System.Linq.Expressions.Expression<System.Func<System.Int32, System.Int32>>.Compile()) ([0] OperationKind.InvocationExpression, Type: System.Func<System.Int32, System.Int32>) (Syntax: InvocationExpression, 'e.Compile()')
                Instance Receiver: 
                  ILocalReferenceExpression: e ([0] OperationKind.LocalReferenceExpression, Type: System.Linq.Expressions.Expression<System.Func<System.Int32, System.Int32>>) (Syntax: IdentifierName, 'e')
                Arguments(0)
      IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... lambda(i));')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.Int32 value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... (lambda(i))')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'lambda(i)')
                IInvocationExpression (virtual System.Int32 System.Func<System.Int32, System.Int32>.Invoke(System.Int32 arg)) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'lambda(i)')
                  Instance Receiver: 
                    ILocalReferenceExpression: lambda ([0] OperationKind.LocalReferenceExpression, Type: System.Func<System.Int32, System.Int32>) (Syntax: IdentifierName, 'lambda')
                  Arguments(1):
                    IArgument (ArgumentKind.Explicit, Matching Parameter: arg) ([1] OperationKind.Argument) (Syntax: Argument, 'i')
                      ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_CustomerTypeInFor()
        {
            string source = @"
class C
{
    static void Main(string[] args)
    {
        /*<bind>*/for (C1 i = new C1(); i == null; i++) { }/*</bind>*/
    }
}
public class C1
{
    public static C1 operator ++(C1 obj)
    {
        return obj;
    }
}
";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (C1 i = ... l; i++) { }') (Parent: BlockStatement)
  Locals: Local_1: C1 i
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.Equals) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: EqualsExpression, 'i == null')
      Left: 
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: IdentifierName, 'i')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: C1) (Syntax: IdentifierName, 'i')
      Right: 
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Object, Constant: null, IsImplicit) (Syntax: NullLiteralExpression, 'null')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NullLiteralExpression, 'null')
  Before:
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'C1 i = new C1()')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i = new C1()')
        Variables: Local_1: C1 i
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= new C1()')
            IObjectCreationExpression (Constructor: C1..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: C1) (Syntax: ObjectCreationExpression, 'new C1()')
              Arguments(0)
              Initializer: 
                null
  AtLoopBottom:
    IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: PostIncrementExpression, 'i++')
      Expression: 
        IIncrementOrDecrementExpression (Postfix) (OperatorMethod: C1 C1.op_Increment(C1 obj)) ([0] OperationKind.IncrementExpression, Type: C1) (Syntax: PostIncrementExpression, 'i++')
          Target: 
            ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: C1) (Syntax: IdentifierName, 'i')
  Body: 
    IBlockStatement (0 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ }')
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_PostFixIncrementInFor()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        int i = 0;
        /*<bind>*/for (int j = i++; j < 5; ++j)
        {
            System.Console.WriteLine(j);
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([1] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (int j  ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 j
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'j < 5')
      Left: 
        ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
  Before:
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int j = i++')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'j = i++')
        Variables: Local_1: System.Int32 j
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= i++')
            IIncrementOrDecrementExpression (Postfix) ([0] OperationKind.IncrementExpression, Type: System.Int32) (Syntax: PostIncrementExpression, 'i++')
              Target: 
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
  AtLoopBottom:
    IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: PreIncrementExpression, '++j')
      Expression: 
        IIncrementOrDecrementExpression (Prefix) ([0] OperationKind.IncrementExpression, Type: System.Int32) (Syntax: PreIncrementExpression, '++j')
          Target: 
            ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... iteLine(j);')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.Int32 value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(j)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'j')
                ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_PreFixIncrementInFor()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        int i = 0;
        /*<bind>*/for (int j = ++i; j < 5; ++j)
        {
            System.Console.WriteLine(j);
        }/*</bind>*/
    }
}

";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([1] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (int j  ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 j
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'j < 5')
      Left: 
        ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
  Before:
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int j = ++i')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'j = ++i')
        Variables: Local_1: System.Int32 j
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= ++i')
            IIncrementOrDecrementExpression (Prefix) ([0] OperationKind.IncrementExpression, Type: System.Int32) (Syntax: PreIncrementExpression, '++i')
              Target: 
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
  AtLoopBottom:
    IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: PreIncrementExpression, '++j')
      Expression: 
        IIncrementOrDecrementExpression (Prefix) ([0] OperationKind.IncrementExpression, Type: System.Int32) (Syntax: PreIncrementExpression, '++j')
          Target: 
            ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... iteLine(j);')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.Int32 value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(j)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'j')
                ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_PreFixIncrementInCondition()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/for (int i = 0; ++i < 5;)
        {
            System.Console.WriteLine(i);
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (int i  ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 i
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, '++i < 5')
      Left: 
        IIncrementOrDecrementExpression (Prefix) ([0] OperationKind.IncrementExpression, Type: System.Int32) (Syntax: PreIncrementExpression, '++i')
          Target: 
            ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
  Before:
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int i = 0')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i = 0')
        Variables: Local_1: System.Int32 i
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  AtLoopBottom(0)
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... iteLine(i);')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.Int32 value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(i)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'i')
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_PostFixDecrementInCondition()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/for (int i = 0; foo(i--) > -5;)
        {
            System.Console.WriteLine(i);
        }/*</bind>*/
    }
    static int foo(int x)
    {
        return x;
    }
}

";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (int i  ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 i
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'foo(i--) > -5')
      Left: 
        IInvocationExpression (System.Int32 Program.foo(System.Int32 x)) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'foo(i--)')
          Instance Receiver: 
            null
          Arguments(1):
            IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([0] OperationKind.Argument) (Syntax: Argument, 'i--')
              IIncrementOrDecrementExpression (Postfix) ([0] OperationKind.DecrementExpression, Type: System.Int32) (Syntax: PostDecrementExpression, 'i--')
                Target: 
                  ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
              InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Right: 
        IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([1] OperationKind.UnaryOperatorExpression, Type: System.Int32, Constant: -5) (Syntax: UnaryMinusExpression, '-5')
          Operand: 
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
  Before:
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int i = 0')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i = 0')
        Variables: Local_1: System.Int32 i
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  AtLoopBottom(0)
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... iteLine(i);')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.Int32 value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(i)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'i')
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_InfiniteLoopVerify()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/for (; true;)
        {
            System.Console.WriteLine(""z"");
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (; true ... }') (Parent: BlockStatement)
  Condition: 
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Boolean, Constant: True) (Syntax: TrueLiteralExpression, 'true')
  Before(0)
  AtLoopBottom(0)
  Body: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... eLine(""z"");')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.String value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... teLine(""z"")')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, '""z""')
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: ""z"") (Syntax: StringLiteralExpression, '""z""')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_InvalidExpression()
        {
            string source = @"
class C
{
    static void Main(string[] args)
    {
        /*<bind>*/for (int k = 0, j = 0; k < 100, j > 5;/*</bind>*/ k++)
        {
        }
    }
}
";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([0] OperationKind.LoopStatement, IsInvalid) (Syntax: ForStatement, 'for (int k  ... 100, j > 5;') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 k
    Local_2: System.Int32 j
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean, IsInvalid) (Syntax: LessThanExpression, 'k < 100')
      Left: 
        ILocalReferenceExpression: k ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'k')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 100, IsInvalid) (Syntax: NumericLiteralExpression, '100')
  Before:
    IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int k = 0, j = 0')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'k = 0')
        Variables: Local_1: System.Int32 k
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
      IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'j = 0')
        Variables: Local_1: System.Int32 j
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  AtLoopBottom:
    IExpressionStatement ([3] OperationKind.ExpressionStatement, IsInvalid, IsImplicit) (Syntax: IdentifierName, '')
      Expression: 
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
          Children(0)
    IExpressionStatement ([4] OperationKind.ExpressionStatement, IsInvalid, IsImplicit) (Syntax: GreaterThanExpression, 'j > 5')
      Expression: 
        IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean, IsInvalid) (Syntax: GreaterThanExpression, 'j > 5')
          Left: 
            ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32, IsInvalid) (Syntax: IdentifierName, 'j')
          Right: 
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5, IsInvalid) (Syntax: NumericLiteralExpression, '5')
  Body: 
    IEmptyStatement ([2] OperationKind.EmptyStatement, IsInvalid) (Syntax: EmptyStatement, ';')
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForLoopStatement_ConditionOutVar()
        {
            string source = @"
class P
{
    private void M()
    {
        var s = """";
        /*<bind>*/for (var j = int.TryParse(s, out var i) ? i : 0; i < 10; i++)
        {
            System.Console.WriteLine($""i={i}, s={s}"");
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([1] OperationKind.LoopStatement) (Syntax: ForStatement, 'for (var j  ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 j
    Local_2: System.Int32 i
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < 10')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
  Before:
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'var j = int ...  i) ? i : 0')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'j = int.Try ...  i) ? i : 0')
        Variables: Local_1: System.Int32 j
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= int.TryPa ...  i) ? i : 0')
            IConditionalExpression ([0] OperationKind.ConditionalExpression, Type: System.Int32) (Syntax: ConditionalExpression, 'int.TryPars ...  i) ? i : 0')
              Condition: 
                IInvocationExpression (System.Boolean System.Int32.TryParse(System.String s, out System.Int32 result)) ([0] OperationKind.InvocationExpression, Type: System.Boolean) (Syntax: InvocationExpression, 'int.TryPars ...  out var i)')
                  Instance Receiver: 
                    null
                  Arguments(2):
                    IArgument (ArgumentKind.Explicit, Matching Parameter: s) ([0] OperationKind.Argument) (Syntax: Argument, 's')
                      ILocalReferenceExpression: s ([0] OperationKind.LocalReferenceExpression, Type: System.String) (Syntax: IdentifierName, 's')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    IArgument (ArgumentKind.Explicit, Matching Parameter: result) ([1] OperationKind.Argument) (Syntax: Argument, 'out var i')
                      IDeclarationExpression ([0] OperationKind.DeclarationExpression, Type: System.Int32) (Syntax: DeclarationExpression, 'var i')
                        ILocalReferenceExpression: i (IsDeclaration: True) ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: SingleVariableDesignation, 'i')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              WhenTrue: 
                ILocalReferenceExpression: i ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
              WhenFalse: 
                ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  AtLoopBottom:
    IExpressionStatement ([3] OperationKind.ExpressionStatement, IsImplicit) (Syntax: PostIncrementExpression, 'i++')
      Expression: 
        IIncrementOrDecrementExpression (Postfix) ([0] OperationKind.IncrementExpression, Type: System.Int32) (Syntax: PostIncrementExpression, 'i++')
          Target: 
            ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... }, s={s}"");')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.String value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... i}, s={s}"")')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, '$""i={i}, s={s}""')
                IInterpolatedStringExpression ([0] OperationKind.InterpolatedStringExpression, Type: System.String) (Syntax: InterpolatedStringExpression, '$""i={i}, s={s}""')
                  Parts(4):
                    IInterpolatedStringText ([0] OperationKind.InterpolatedStringText) (Syntax: InterpolatedStringText, 'i=')
                      Text: 
                        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: ""i="") (Syntax: InterpolatedStringText, 'i=')
                    IInterpolation ([1] OperationKind.Interpolation) (Syntax: Interpolation, '{i}')
                      Expression: 
                        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                      Alignment: 
                        null
                      FormatString: 
                        null
                    IInterpolatedStringText ([2] OperationKind.InterpolatedStringText) (Syntax: InterpolatedStringText, ', s=')
                      Text: 
                        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "", s="") (Syntax: InterpolatedStringText, ', s=')
                    IInterpolation ([3] OperationKind.Interpolation) (Syntax: Interpolation, '{s}')
                      Expression: 
                        ILocalReferenceExpression: s ([0] OperationKind.LocalReferenceExpression, Type: System.String) (Syntax: IdentifierName, 's')
                      Alignment: 
                        null
                      FormatString: 
                        null
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            VerifyOperationTreeForTest<ForStatementSyntax>(source, expectedOperationTree);
        }

    }
}
